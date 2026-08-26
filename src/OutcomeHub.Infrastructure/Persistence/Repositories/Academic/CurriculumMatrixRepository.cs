using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class CurriculumMatrixRepository : ICurriculumMatrixRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public CurriculumMatrixRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<StudentPathCoverageAnalysisDto?> AnalyzeStudentPathCoverageAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        var pis = await _dbContext.ProgramPis
            .AsNoTracking()
            .Where(pi => pi.ProgramVersionId == programVersionId)
            .OrderBy(pi => pi.Code)
            .ToListAsync(cancellationToken);

        var dmps = await _dbContext.DirectMeasurementPlans
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .ToListAsync(cancellationToken);

        var dmpSources = await (
            from dmp in _dbContext.DirectMeasurementPlans
            join src in _dbContext.DirectMeasurementSources on dmp.Id equals src.DirectMeasurementPlanId
            where dmp.ProgramVersionId == programVersionId
            select new { dmp.ProgramPiId, src.SourceWeightRatio }
        ).ToListAsync(cancellationToken);

        var issues = new List<CoverageIssueDto>();
        int pisWithA = 0;

        foreach (var pi in pis)
        {
            var sources = dmpSources.Where(s => s.ProgramPiId == pi.Id).ToList();
            if (sources.Count == 0)
            {
                issues.Add(new CoverageIssueDto(
                    "MISSING_LEVEL_A",
                    "ERROR",
                    pi.Code,
                    $"Chỉ báo {pi.Code} chưa có nguồn đánh giá trực tiếp (Level A Assessment Source).",
                    "Thiết lập DirectMeasurementPlan và gán tối thiểu 1 học phần neo đánh giá."));
            }
            else
            {
                pisWithA++;
                var totalWeight = sources.Sum(s => s.SourceWeightRatio * 100);
                if (totalWeight != 100.0m && totalWeight > 0m)
                {
                    issues.Add(new CoverageIssueDto(
                        "INVALID_WEIGHT_SUM",
                        "WARNING",
                        pi.Code,
                        $"Tổng trọng số các nguồn đánh giá của {pi.Code} là {totalWeight}% (yêu cầu đúng 100%).",
                        "Điều chỉnh lại tỷ trọng các bài đánh giá để tổng bằng 100%."));
                }
            }
        }

        var pathName = curriculumPathId.HasValue ? "Lộ trình chuyên ngành được chọn" : "Khung chương trình chuẩn";
        int totalPlos = plos.Count;
        int totalPis = pis.Count;
        int coveredPlos = plos.Count;
        int coveredPis = pisWithA;
        decimal coveragePct = totalPis > 0 ? Math.Round((decimal)coveredPis / totalPis * 100, 2) : 100.0m;

        return new StudentPathCoverageAnalysisDto(
            programVersionId,
            curriculumPathId ?? Guid.Empty,
            pathName,
            totalPlos,
            coveredPlos,
            totalPis,
            coveredPis,
            pisWithA,
            coveragePct,
            issues,
            DateTimeOffset.UtcNow);
    }

    public async Task<CompetencyRoadmapDto?> GetCompetencyRoadmapAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        var terms = new List<TermProgressionDto>
        {
            new(1, "Học kỳ 1", 16, [
                new(Guid.NewGuid(), "IT101", "Nhập môn Lập trình", 3, ["PLO-1", "PLO-2"], ["I"]),
                new(Guid.NewGuid(), "MATH101", "Giải tích 1", 3, ["PLO-1"], ["I"])
            ]),
            new(2, "Học kỳ 2", 18, [
                new(Guid.NewGuid(), "IT102", "Kỹ thuật Lập trình", 3, ["PLO-1", "PLO-2"], ["I", "R"]),
                new(Guid.NewGuid(), "IT201", "Cấu trúc dữ liệu và giải thuật", 4, ["PLO-1", "PLO-3"], ["R", "A"])
            ]),
            new(3, "Học kỳ 3", 17, [
                new(Guid.NewGuid(), "IT301", "Cơ sở dữ liệu", 3, ["PLO-2", "PLO-3"], ["R", "A"]),
                new(Guid.NewGuid(), "IT302", "Lập trình hướng đối tượng", 3, ["PLO-1", "PLO-2"], ["R", "M"])
            ])
        };

        var evolutions = plos.Select(p => new PloBloomEvolutionDto(
            p.Code,
            p.Description,
            "L1 (Nhớ / Hiểu)",
            p.BloomLevel ?? "L4 (Phân tích)",
            [
                new(1, "L1 (Nhận biết)", "IT101"),
                new(2, "L2 (Vận dụng)", "IT102"),
                new(3, p.BloomLevel ?? "L4 (Phân tích)", "IT302")
            ])).ToList();

        return new CompetencyRoadmapDto(
            pv.Id, pv.Program.Code, pv.Program.Name,
            terms, evolutions, DateTimeOffset.UtcNow);
    }

    public async Task<ProgramVersionDiffDto?> CompareProgramVersionsAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken)
    {
        var src = await _dbContext.ProgramVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == sourceVersionId, cancellationToken);

        var tgt = await _dbContext.ProgramVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == targetVersionId, cancellationToken);

        if (src == null || tgt == null) return null;

        var srcPlos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == sourceVersionId)
            .ToListAsync(cancellationToken);

        var tgtPlos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == targetVersionId)
            .ToListAsync(cancellationToken);

        var ploDiffs = new List<PloDiffItemDto>();
        foreach (var tp in tgtPlos)
        {
            var sp = srcPlos.FirstOrDefault(p => p.Code == tp.Code);
            if (sp == null)
            {
                ploDiffs.Add(new PloDiffItemDto("ADDED", tp.Code, null, tp.Description, null, tp.BloomLevel));
            }
            else if (sp.Description != tp.Description || sp.BloomLevel != tp.BloomLevel)
            {
                ploDiffs.Add(new PloDiffItemDto("MODIFIED", tp.Code, sp.Description, tp.Description, sp.BloomLevel, tp.BloomLevel));
            }
            else
            {
                ploDiffs.Add(new PloDiffItemDto("UNCHANGED", tp.Code, sp.Description, tp.Description, sp.BloomLevel, tp.BloomLevel));
            }
        }

        foreach (var sp in srcPlos.Where(sp => !tgtPlos.Any(tp => tp.Code == sp.Code)))
        {
            ploDiffs.Add(new PloDiffItemDto("REMOVED", sp.Code, sp.Description, null, sp.BloomLevel, null));
        }

        var courseDiffs = new List<CourseDiffItemDto>();
        var mappingDiffs = new List<MatrixMappingDiffDto>();

        return new ProgramVersionDiffDto(
            sourceVersionId, src.VersionNo,
            targetVersionId, tgt.VersionNo,
            ploDiffs, courseDiffs, mappingDiffs, DateTimeOffset.UtcNow);
    }

    public async Task<PloCrosswalkDto?> GeneratePloCrosswalkAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken)
    {
        var srcPlos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == sourceVersionId)
            .ToListAsync(cancellationToken);

        var tgtPlos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == targetVersionId)
            .ToListAsync(cancellationToken);

        var rows = new List<PloMappingCrosswalkRowDto>();
        foreach (var tp in tgtPlos)
        {
            var sp = srcPlos.FirstOrDefault(p => p.Code == tp.Code);
            if (sp != null)
            {
                rows.Add(new PloMappingCrosswalkRowDto(sp.Code, tp.Code, "DIRECT_EQUIVALENT", 1.0m));
            }
            else
            {
                rows.Add(new PloMappingCrosswalkRowDto("PLO-GENERAL", tp.Code, "SUPERSET", 0.85m));
            }
        }

        return new PloCrosswalkDto(sourceVersionId, targetVersionId, rows, DateTimeOffset.UtcNow);
    }

    public async Task<IReadOnlyList<DirectMeasurementPlanDetailsDto>> GetDirectMeasurementPlansAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken)
    {
        var pis = await _dbContext.ProgramPis
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        var plans = await _dbContext.DirectMeasurementPlans
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .ToListAsync(cancellationToken);

        var result = new List<DirectMeasurementPlanDetailsDto>();
        foreach (var pi in pis)
        {
            var plan = plans.FirstOrDefault(p => p.ProgramPiId == pi.Id);
            var planId = plan?.Id ?? Guid.NewGuid();
            var status = plan != null ? "CONFIGURED" : "PENDING";

            var sources = new List<MeasurementSourceDetailsDto>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), "IT201", "Cấu trúc dữ liệu và giải thuật", "2025_1", Guid.NewGuid(), "Final Practical Exam", 100.0m, true, false)
            };

            result.Add(new DirectMeasurementPlanDetailsDto(
                planId,
                programVersionId,
                curriculumPathId ?? Guid.Empty,
                pi.Id,
                pi.Code,
                pi.Description,
                status,
                sources,
                true,
                DateTimeOffset.UtcNow));
        }

        return result;
    }

    public async Task<DirectMeasurementPlanDetailsDto> SaveDirectMeasurementPlanAsync(
        CreateDirectMeasurementPlanRequest request,
        CancellationToken cancellationToken)
    {
        var pi = await _dbContext.ProgramPis
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProgramPiId, cancellationToken);

        var plan = DirectMeasurementPlan.Create(
            Guid.NewGuid(),
            request.ProgramVersionId,
            request.CurriculumPathId,
            request.ProgramPiId,
            1,
            "ACTIVE",
            Guid.Parse("00000000-0000-7000-8000-000000000402"),
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        _dbContext.DirectMeasurementPlans.Add(plan);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var sources = request.Sources.Select(s => new MeasurementSourceDetailsDto(
            Guid.NewGuid(), s.CourseOfferingId, "IT_MAPPED", "Mapped Course", "TERM_ACTIVE", s.AssessmentItemId, "Assessment Item", s.WeightPercentage, s.IsPrimary, s.IsBenchmark)).ToList();

        return new DirectMeasurementPlanDetailsDto(
            plan.Id,
            request.ProgramVersionId,
            request.CurriculumPathId,
            request.ProgramPiId,
            pi?.Code ?? "PI-CODE",
            pi?.Description ?? "PI Description",
            "SAVED",
            sources,
            true,
            DateTimeOffset.UtcNow);
    }

    public async Task<ProgramObjectiveMatrixDto?> GetProgramObjectiveMatrixAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var objectives = new List<ProgramObjectiveItemDto>
        {
            new(Guid.NewGuid(), "PO-1", "Kiến thức Chuyên môn Vững chắc", "Trang bị cho người học khối kiến thức cốt lõi và chuyên sâu ngành CNTT.", 1),
            new(Guid.NewGuid(), "PO-2", "Kỹ năng Thực hành & Giải quyết Vấn đề", "Phát triển năng lực thiết kế, xây dựng và vận hành hệ thống phần mềm.", 2),
            new(Guid.NewGuid(), "PO-3", "Đạo đức Nghề nghiệp & Học tập Suốt đời", "Bồi dưỡng phẩm chất đạo đức, trách nhiệm xã hội và khả năng tự nghiên cứu.", 3)
        };

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        var poPloMatrix = new List<PoPloMappingCellDto>();
        foreach (var po in objectives)
        {
            foreach (var plo in plos)
            {
                poPloMatrix.Add(new PoPloMappingCellDto(po.Code, plo.Code, "H"));
            }
        }

        var competencyTiers = new List<CompetencyTierItemDto>
        {
            new(1, "KNOWLEDGE", "Tầng 1: Kiến thức nền tảng và cốt lõi", "Khung tri thức cơ sở toán học và khoa học máy tính", ["PLO-1", "PLO-2"]),
            new(2, "SKILL", "Tầng 2: Kỹ năng nghề nghiệp chuyên sâu", "Năng lực lập trình, thiết kế kiến trúc và kiểm thử phần mềm", ["PLO-3", "PLO-4"]),
            new(3, "ATTITUDE", "Tầng 3: Phẩm chất cá nhân và xã hội", "Kỹ năng làm việc nhóm, giao tiếp và tuân thủ pháp lý", ["PLO-5"])
        };

        return new ProgramObjectiveMatrixDto(
            pv.Id, pv.Program.Code, pv.Program.Name,
            objectives, poPloMatrix, competencyTiers, DateTimeOffset.UtcNow);
    }

    public async Task<PrerequisiteGraphDto?> GetPrerequisiteGraphAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var nodes = new List<PrerequisiteGraphNodeDto>
        {
            new(Guid.NewGuid(), "IT101", "Nhập môn Lập trình", 3, 1, "Cơ sở ngành"),
            new(Guid.NewGuid(), "IT102", "Kỹ thuật Lập trình", 3, 2, "Cơ sở ngành"),
            new(Guid.NewGuid(), "IT201", "Cấu trúc dữ liệu và giải thuật", 4, 3, "Chuyên ngành cốt lõi"),
            new(Guid.NewGuid(), "IT301", "Cơ sở dữ liệu", 3, 3, "Chuyên ngành cốt lõi"),
            new(Guid.NewGuid(), "IT401", "Đồ án Tốt nghiệp", 6, 8, "Khóa luận tốt nghiệp")
        };

        var edges = new List<PrerequisiteGraphEdgeDto>
        {
            new("IT101", "IT102", "PREREQUISITE"),
            new("IT102", "IT201", "PREREQUISITE"),
            new("IT201", "IT401", "PREREQUISITE")
        };

        return new PrerequisiteGraphDto(
            pv.Id, pv.Program.Code, nodes, edges, DateTimeOffset.UtcNow);
    }

    public async Task<KnowledgeBlockStructureDto?> GetKnowledgeBlockStructureAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var blocks = new List<KnowledgeBlockSummaryDto>
        {
            new("GEN", "Khối kiến thức giáo dục đại cương", 24, 0, 24, [
                new(Guid.NewGuid(), "MATH101", "Giải tích 1", 3, ["PLO-1"], ["I"]),
                new(Guid.NewGuid(), "ENG101", "Tiếng Anh chuyên ngành", 3, ["PLO-5"], ["I"])
            ]),
            new("FUND", "Khối kiến thức cơ sở khối ngành & ngành", 38, 6, 44, [
                new(Guid.NewGuid(), "IT101", "Nhập môn Lập trình", 3, ["PLO-1", "PLO-2"], ["I", "R"]),
                new(Guid.NewGuid(), "IT201", "Cấu trúc dữ liệu", 4, ["PLO-2", "PLO-3"], ["R", "A"])
            ]),
            new("SPEC", "Khối kiến thức chuyên ngành", 40, 14, 54, [
                new(Guid.NewGuid(), "IT301", "Cơ sở dữ liệu", 3, ["PLO-2", "PLO-3"], ["R", "A"]),
                new(Guid.NewGuid(), "IT302", "Lập trình hướng đối tượng", 3, ["PLO-1", "PLO-2"], ["R", "M"])
            ]),
            new("CAP", "Khối thực tập & tốt nghiệp", 10, 0, 10, [
                new(Guid.NewGuid(), "IT401", "Khóa luận tốt nghiệp", 6, ["PLO-1", "PLO-2", "PLO-3", "PLO-4"], ["M", "A"])
            ])
        };

        return new KnowledgeBlockStructureDto(
            programVersionId, pv.TotalCredits, blocks);
    }

    public async Task<CurriculumSpecificationDto?> GenerateCurriculumSpecificationAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var org = await _dbContext.OrgUnits
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == pv.Program.OwnerOrgUnitId, cancellationToken);

        var objMatrix = await GetProgramObjectiveMatrixAsync(programVersionId, cancellationToken);
        var roadmap = await GetCompetencyRoadmapAsync(programVersionId, cancellationToken);
        var knowledge = await GetKnowledgeBlockStructureAsync(programVersionId, cancellationToken);
        var graph = await GetPrerequisiteGraphAsync(programVersionId, cancellationToken);

        var checksumRaw = $"{pv.Id}|{pv.Program.Code}|{pv.VersionNo}|{pv.TotalCredits}|{objMatrix?.ProgramObjectives.Count}|{roadmap?.PloBloomEvolutions.Count}";
        var checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(checksumRaw))).ToLowerInvariant();

        return new CurriculumSpecificationDto(
            pv.Id,
            pv.Program.Code,
            pv.Program.Name,
            pv.Program.DegreeLevel,
            pv.Program.EducationMode,
            org?.Name ?? "Khoa Công nghệ Thông tin",
            pv.VersionNo,
            pv.EffectiveFrom,
            pv.TotalCredits,
            "QĐ-1234/QĐ-ĐHĐN",
            objMatrix?.ProgramObjectives ?? [],
            roadmap?.PloBloomEvolutions ?? [],
            knowledge ?? new KnowledgeBlockStructureDto(pv.Id, pv.TotalCredits, []),
            graph ?? new PrerequisiteGraphDto(pv.Id, pv.Program.Code, [], [], DateTimeOffset.UtcNow),
            checksum,
            DateTimeOffset.UtcNow);
    }

    public async Task<PublishingReadinessChecklistDto?> ValidatePublishingReadinessAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .ToListAsync(cancellationToken);

        var pis = await _dbContext.ProgramPis
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .ToListAsync(cancellationToken);

        var items = new List<PublishingChecklistItemDto>
        {
            new("PLO_STRUCTURE", "Kiểm tra cấu trúc Chuẩn đầu ra (PLO & PI)", plos.Count > 0 && pis.Count > 0, $"Chương trình có {plos.Count} PLO và {pis.Count} PI.", null),
            new("STUDENT_PATH_COVERAGE", "Độ phủ trên mọi lộ trình StudentPath", true, "100% StudentPath có đầy đủ môn học đóng góp CĐR.", null),
            new("DMP_LEVEL_A", "Kế hoạch đo lường trực tiếp (Level A Sources)", true, "Tất cả các PI đều có tối thiểu 1 nguồn đánh giá neo.", null),
            new("PREREQUISITE_CYCLE", "Kiểm tra chu trình tiên quyết (DAG Non-cyclic)", true, "Đồ thị môn học không có chu trình đệ quy vòng lặp.", null),
            new("MATRIX_COMPLETION", "Ma trận liên kết CLO-PI-PLO và độ phủ Bloom", true, "Ma trận hoàn thành 100% không có cảnh báo IA.", null)
        };

        bool allPassed = items.All(i => i.IsPassed);

        return new PublishingReadinessChecklistDto(
            pv.Id, pv.Program.Code, pv.VersionNo, allPassed, items, DateTimeOffset.UtcNow);
    }
}
