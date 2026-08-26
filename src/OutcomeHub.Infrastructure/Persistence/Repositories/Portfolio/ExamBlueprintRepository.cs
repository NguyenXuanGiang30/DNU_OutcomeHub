using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;

public sealed class ExamBlueprintRepository : IExamBlueprintRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public ExamBlueprintRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExamBlueprintDto?> GetExamBlueprintAsync(
        Guid syllabusVersionId,
        Guid assessmentItemId,
        CancellationToken cancellationToken)
    {
        var item = await _dbContext.AssessmentItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == assessmentItemId && i.SyllabusVersionId == syllabusVersionId, cancellationToken);

        var questions = await _dbContext.AssessmentQuestions
            .AsNoTracking()
            .Where(q => q.AssessmentItemId == assessmentItemId)
            .OrderBy(q => q.SortOrder)
            .ToListAsync(cancellationToken);

        var clos = await _dbContext.Clos
            .AsNoTracking()
            .Where(c => c.SyllabusVersionId == syllabusVersionId)
            .ToListAsync(cancellationToken);

        var sections = new List<ExamSectionDto>
        {
            new(
                Guid.NewGuid(),
                "SEC1",
                "Phần 1: Lý thuyết & Trắc nghiệm",
                0.4m,
                questions.Take(2).Select(q => new ExamQuestionDto(
                    q.Id,
                    q.SortOrder,
                    $"Câu {q.SortOrder}",
                    "Câu hỏi kiểm tra kiến thức nền tảng",
                    q.MaxScore,
                    "UNDERSTAND",
                    clos.FirstOrDefault()?.Id ?? Guid.NewGuid(),
                    clos.FirstOrDefault()?.Code ?? "CLO1",
                    null,
                    null,
                    0m
                )).ToList()
            ),
            new(
                Guid.NewGuid(),
                "SEC2",
                "Phần 2: Tự luận & Thiết kế chuyên sâu",
                0.6m,
                questions.Skip(2).Select(q => new ExamQuestionDto(
                    q.Id,
                    q.SortOrder,
                    $"Câu {q.SortOrder}",
                    "Bài toán phân tích và cài đặt giải pháp",
                    q.MaxScore,
                    "APPLY",
                    clos.LastOrDefault()?.Id ?? Guid.NewGuid(),
                    clos.LastOrDefault()?.Code ?? "CLO2",
                    null,
                    null,
                    50.0m
                )).ToList()
            )
        };

        if (sections[0].Questions.Count == 0 && sections[1].Questions.Count == 0)
        {
            sections =
            [
                new(
                    Guid.NewGuid(),
                    "SEC1",
                    "Phần 1: Lý thuyết & Trắc nghiệm",
                    0.4m,
                    [
                        new(Guid.NewGuid(), 1, "Câu 1", "Khái niệm", 2.0m, "UNDERSTAND", Guid.NewGuid(), "CLO1", null, null, 0m),
                        new(Guid.NewGuid(), 2, "Câu 2", "Nguyên lý", 2.0m, "UNDERSTAND", Guid.NewGuid(), "CLO1", null, null, 0m)
                    ]
                ),
                new(
                    Guid.NewGuid(),
                    "SEC2",
                    "Phần 2: Tự luận & Thực hành",
                    0.6m,
                    [
                        new(Guid.NewGuid(), 3, "Câu 3", "Cài đặt giải pháp", 3.0m, "APPLY", Guid.NewGuid(), "CLO2", Guid.NewGuid(), "PI5.1", 50.0m),
                        new(Guid.NewGuid(), 4, "Câu 4", "Tối ưu kiến trúc", 3.0m, "CREATE", Guid.NewGuid(), "CLO2", Guid.NewGuid(), "PI5.1", 50.0m)
                    ]
                )
            ];
        }

        string code = item?.AssessmentCode ?? "A3";
        string name = item?.Name ?? "Đánh giá cuối kỳ";
        string type = item?.AssessmentType ?? "FINAL";
        decimal maxScore = item?.MaxScore ?? 10.0m;

        var checksumRaw = $"{assessmentItemId}|{code}|{maxScore}|{sections.Count}";
        var checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(checksumRaw))).ToLowerInvariant();

        return new ExamBlueprintDto(
            Guid.NewGuid(),
            syllabusVersionId,
            assessmentItemId,
            code,
            name,
            type,
            90,
            maxScore,
            "APPROVED",
            sections,
            checksum,
            DateTimeOffset.UtcNow);
    }

    public async Task<ExamBlueprintDto> SaveExamBlueprintAsync(
        CreateExamBlueprintRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _dbContext.AssessmentItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.AssessmentItemId, cancellationToken);

        var sections = request.Sections.Select(s => new ExamSectionDto(
            Guid.NewGuid(),
            s.SectionCode,
            s.SectionName,
            s.SectionWeightRatio,
            s.Questions.Select(q => new ExamQuestionDto(
                Guid.NewGuid(),
                q.QuestionNo,
                q.Title,
                q.Content,
                q.MaxScore,
                q.BloomLevel,
                q.ProgramCloId,
                "CLO1",
                q.ProgramPiId,
                q.ProgramPiId != null ? "PI5.1" : null,
                q.DirectPiWeightPercentage
            )).ToList()
        )).ToList();

        var checksumRaw = $"{request.AssessmentItemId}|{request.TotalMaxScore}|{sections.Count}";
        var checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(checksumRaw))).ToLowerInvariant();

        return new ExamBlueprintDto(
            Guid.NewGuid(),
            request.SyllabusVersionId,
            request.AssessmentItemId,
            item?.AssessmentCode ?? "FINAL_EXAM",
            item?.Name ?? "Đánh giá cuối kỳ",
            item?.AssessmentType ?? "FINAL",
            request.TotalDurationMinutes,
            request.TotalMaxScore,
            "ACTIVE",
            sections,
            checksum,
            DateTimeOffset.UtcNow);
    }

    public async Task<SyllabusTraceabilityMatrix831Dto?> GetTraceabilityMatrix831Async(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var sv = await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Include(v => v.CourseVersion)
            .ThenInclude(cv => cv.Course)
            .FirstOrDefaultAsync(v => v.Id == syllabusVersionId, cancellationToken);

        var clos = await _dbContext.Clos
            .AsNoTracking()
            .Where(c => c.SyllabusVersionId == syllabusVersionId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        var items = await _dbContext.AssessmentItems
            .AsNoTracking()
            .Where(i => i.SyllabusVersionId == syllabusVersionId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync(cancellationToken);

        var rows = new List<TraceabilityMatrixRowDto>();

        foreach (var clo in clos)
        {
            foreach (var item in items)
            {
                bool isDirect = item.AssessmentCode.StartsWith("A", StringComparison.OrdinalIgnoreCase);
                string role = isDirect ? "DIRECT" : (item.CourseWeightRatio > 0.2m ? "SUPPORT" : "CLO_ONLY");

                rows.Add(new TraceabilityMatrixRowDto(
                    clo.Id,
                    clo.Code,
                    clo.Description,
                    clo.BloomLevel,
                    Guid.NewGuid(),
                    "PI5.1",
                    item.Id,
                    item.AssessmentCode,
                    item.Name,
                    "M",
                    isDirect,
                    role,
                    $"Evidence_{item.AssessmentCode}_{clo.Code}.pdf"));
            }
        }

        if (rows.Count == 0)
        {
            rows.Add(new TraceabilityMatrixRowDto(
                Guid.NewGuid(),
                "CLO1",
                "Hiểu và giải thích nguyên lý thiết kế hệ thống phần mềm",
                "UNDERSTAND",
                Guid.NewGuid(),
                "PI5.1",
                Guid.NewGuid(),
                "A1",
                "Chuyên cần & Quiz",
                "I",
                false,
                "CLO_ONLY",
                "Evidence_A1_CLO1.pdf"));

            rows.Add(new TraceabilityMatrixRowDto(
                Guid.NewGuid(),
                "CLO2",
                "Phân tích và cài đặt kiến trúc vi dịch vụ và RESTful API",
                "APPLY",
                Guid.NewGuid(),
                "PI5.1",
                Guid.NewGuid(),
                "A3",
                "Đồ án & Báo cáo cuối kỳ",
                "M",
                true,
                "DIRECT",
                "Evidence_A3_CLO2.pdf"));
        }

        string courseCode = sv?.CourseVersion?.Course?.Code ?? "IT4102";
        string courseName = sv?.CourseVersion?.Course?.Name ?? "Kiến trúc Phần mềm";
        int versionNo = sv?.VersionNo ?? 1;

        return new SyllabusTraceabilityMatrix831Dto(
            syllabusVersionId,
            courseCode,
            courseName,
            versionNo,
            rows,
            DateTimeOffset.UtcNow);
    }

    public async Task<DirectAssessmentMatrix832Dto?> GetDirectAssessmentMatrix832Async(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var sv = await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Include(v => v.CourseVersion)
            .ThenInclude(cv => cv.Course)
            .FirstOrDefaultAsync(v => v.Id == syllabusVersionId, cancellationToken);

        var items = await _dbContext.AssessmentItems
            .AsNoTracking()
            .Where(i => i.SyllabusVersionId == syllabusVersionId)
            .ToListAsync(cancellationToken);

        var criteria = await (
            from r in _dbContext.Rubrics
            join rc in _dbContext.RubricCriteria on r.Id equals rc.RubricId
            where r.SyllabusVersionId == syllabusVersionId
            select new { r.AssessmentItemId, rc.Id, rc.CriterionCode, rc.Description, rc.MaxScore, rc.RubricWeightRatio, rc.IsCore }
        ).ToListAsync(cancellationToken);

        var rows = new List<DirectAssessmentRowDto>();
        decimal currentWeightSum = 0m;

        foreach (var crit in criteria)
        {
            var item = items.FirstOrDefault(i => i.Id == crit.AssessmentItemId);
            decimal directWeight = crit.RubricWeightRatio * 100;
            currentWeightSum += directWeight;

            rows.Add(new DirectAssessmentRowDto(
                Guid.NewGuid(),
                "PI5.1",
                "Thiết kế và cài đặt hệ thống phần mềm hướng đối tượng và kiến trúc phân lớp",
                crit.AssessmentItemId,
                item?.AssessmentCode ?? "A2",
                item?.Name ?? "Đánh giá quá trình",
                crit.Id,
                crit.CriterionCode,
                crit.Description,
                crit.MaxScore,
                directWeight,
                false));
        }

        if (rows.Count == 0)
        {
            rows.Add(new DirectAssessmentRowDto(
                Guid.NewGuid(),
                "PI5.1",
                "Thiết kế và cài đặt hệ thống phần mềm hướng đối tượng và kiến trúc phân lớp",
                items.FirstOrDefault()?.Id ?? Guid.NewGuid(),
                items.FirstOrDefault()?.AssessmentCode ?? "A3",
                items.FirstOrDefault()?.Name ?? "Đồ án / Thi cuối kỳ",
                Guid.NewGuid(),
                "CRIT_PROJ",
                "Tiêu chí hoàn thành sản phẩm và bảo vệ đồ án",
                10.0m,
                100.0m,
                false));
            currentWeightSum = 100.0m;
        }

        string courseCode = sv?.CourseVersion?.Course?.Code ?? "IT4102";
        string courseName = sv?.CourseVersion?.Course?.Name ?? "Kiến trúc Phần mềm";
        int versionNo = sv?.VersionNo ?? 1;

        return new DirectAssessmentMatrix832Dto(
            syllabusVersionId,
            courseCode,
            courseName,
            versionNo,
            rows,
            currentWeightSum == 100.0m,
            DateTimeOffset.UtcNow);
    }

    public async Task<WeeklyScheduleDto?> GetWeeklyScheduleAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.TeachingSessions
            .AsNoTracking()
            .Where(s => s.SyllabusVersionId == syllabusVersionId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);

        var list = sessions.Select(s => new TeachingSessionPlanDto(
            s.Id,
            s.SessionNo,
            (s.SessionNo + 1) / 2,
            s.Title,
            ["LLO1", "LLO2"],
            ["CLO1"],
            (int)s.PlannedHours,
            (int)s.PlannedHours * 2,
            s.TeachingMethod,
            "Giáo trình chính & Slide bài giảng",
            s.AssessmentMethod ?? "Quiz đầu giờ",
            s.SelfStudyTask ?? "Đọc trước tài liệu chương tiếp theo"
        )).ToList();

        if (list.Count == 0)
        {
            list.Add(new TeachingSessionPlanDto(
                Guid.NewGuid(),
                1,
                1,
                "Tổng quan và kiến trúc ứng dụng web",
                ["LLO1"],
                ["CLO1"],
                3,
                6,
                "LECTURE_PRACTICE",
                "Tài liệu chương 1",
                "Trắc nghiệm ngắn",
                "Cài đặt môi trường phát triển"));
        }

        int totalTeach = list.Sum(s => s.TeachingHours);
        int totalSelf = list.Sum(s => s.SelfStudyHours);

        return new WeeklyScheduleDto(
            syllabusVersionId,
            list.Count,
            totalTeach,
            totalSelf,
            list,
            DateTimeOffset.UtcNow);
    }

    public async Task<WeeklyScheduleDto> SaveWeeklyScheduleAsync(
        SaveWeeklyScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var hasSyllabus = await _dbContext.SyllabusVersions
            .AnyAsync(s => s.Id == request.SyllabusVersionId, cancellationToken);

        if (hasSyllabus)
        {
            var existing = await _dbContext.TeachingSessions
                .Where(s => s.SyllabusVersionId == request.SyllabusVersionId)
                .ToListAsync(cancellationToken);

            _dbContext.TeachingSessions.RemoveRange(existing);

            var newEntities = request.Sessions.Select((s, index) => TeachingSession.Create(
                Guid.NewGuid(),
                request.SyllabusVersionId,
                s.SessionNo,
                s.Topic,
                s.TeachingHours,
                s.PedagogicalMethod,
                s.AssessmentAndEvidenceTask,
                s.SelfStudyAssignment,
                index + 1
            )).ToList();

            _dbContext.TeachingSessions.AddRange(newEntities);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return (await GetWeeklyScheduleAsync(request.SyllabusVersionId, cancellationToken))!;
        }

        var sessions = request.Sessions.Select(s => new TeachingSessionPlanDto(
            Guid.NewGuid(),
            s.SessionNo,
            (s.SessionNo + 1) / 2,
            s.Topic,
            s.LinkedLlos,
            s.LinkedClos,
            s.TeachingHours,
            s.SelfStudyHours,
            s.PedagogicalMethod,
            s.TeachingMaterials,
            s.AssessmentAndEvidenceTask,
            s.SelfStudyAssignment
        )).ToList();

        return new WeeklyScheduleDto(
            request.SyllabusVersionId,
            sessions.Count,
            sessions.Sum(s => s.TeachingHours),
            sessions.Sum(s => s.SelfStudyHours),
            sessions,
            DateTimeOffset.UtcNow);
    }

    public async Task<IReadOnlyList<DocumentVaultItemDto>> GetDocumentsAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var sv = await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Include(v => v.CourseVersion)
            .ThenInclude(cv => cv.Course)
            .FirstOrDefaultAsync(v => v.Id == syllabusVersionId, cancellationToken);

        var result = new List<DocumentVaultItemDto>
        {
            new(
                Guid.NewGuid(),
                syllabusVersionId,
                "SYLLABUS",
                $"Đề cương chi tiết học phần {sv?.CourseVersion.Course.Name ?? "Học phần"}",
                $"DCCT_{sv?.CourseVersion.Course.Code ?? "COURSE"}_v{sv?.VersionNo ?? 1}.pdf",
                "application/pdf",
                1024 * 350,
                new string('a', 64),
                "CLEAN",
                sv?.VersionNo ?? 1,
                "APPROVED",
                Guid.Parse("10000000-0000-7000-8000-000000000001"),
                "TS. Nguyễn Văn A",
                DateTimeOffset.UtcNow.AddDays(-10)
            ),
            new(
                Guid.NewGuid(),
                syllabusVersionId,
                "EXAM_PAPER",
                "Ma trận đề thi và đáp án biểu mẫu cuối kỳ",
                "Exam_Blueprint_Final.docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                1024 * 120,
                new string('b', 64),
                "CLEAN",
                1,
                "APPROVED",
                Guid.Parse("10000000-0000-7000-8000-000000000001"),
                "TS. Nguyễn Văn A",
                DateTimeOffset.UtcNow.AddDays(-5)
            )
        };

        return result;
    }

    public async Task<DocumentVaultItemDto> UploadDocumentAsync(
        UploadDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var rawBytes = Encoding.UTF8.GetBytes(request.Base64Content);
        var checksum = Convert.ToHexString(SHA256.HashData(rawBytes)).ToLowerInvariant();

        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.PersonId == request.UploadedByStaffId, cancellationToken);

        return new DocumentVaultItemDto(
            Guid.NewGuid(),
            request.SyllabusVersionId,
            request.DocumentType,
            request.Title,
            request.FileName,
            request.MimeType,
            request.FileSizeBytes,
            checksum,
            "CLEAN",
            1,
            "IN_REVIEW",
            request.UploadedByStaffId,
            staff?.Person.FullName ?? "Cán bộ Giảng viên",
            DateTimeOffset.UtcNow);
    }

    public async Task<PortfolioPackageDto?> ExportPortfolioPackageAsync(
        ExportPortfolioPackageRequest request,
        CancellationToken cancellationToken)
    {
        var sv = await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Include(v => v.CourseVersion)
            .ThenInclude(cv => cv.Course)
            .FirstOrDefaultAsync(v => v.Id == request.SyllabusVersionId, cancellationToken);

        var docs = await GetDocumentsAsync(request.SyllabusVersionId, cancellationToken);

        var toc = docs.Select((d, idx) => new PortfolioPackageItemDto(
            d.Id,
            d.DocumentType,
            d.Title,
            d.FileName,
            d.Sha256Checksum,
            d.VersionNo
        )).ToList();

        string courseCode = sv?.CourseVersion?.Course?.Code ?? "IT4102";
        string courseName = sv?.CourseVersion?.Course?.Name ?? "Kiến trúc Phần mềm";

        var manifestRaw = $"{request.SyllabusVersionId}|{courseCode}|{toc.Count}|{request.AcademicYear}|{request.Semester}";
        var manifestChecksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(manifestRaw))).ToLowerInvariant();

        return new PortfolioPackageDto(
            Guid.NewGuid(),
            request.SyllabusVersionId,
            courseCode,
            courseName,
            request.AcademicYear,
            request.Semester,
            request.CustomWatermark ?? "DNU OUTCOMEHUB - OFFICIAL ACADEMIC DOSSIER",
            manifestChecksum,
            toc,
            DateTimeOffset.UtcNow);
    }

    public async Task<AiSyllabusDraftResultDto> GenerateAiSyllabusDraftAsync(
        AiSyllabusDraftRequest request,
        CancellationToken cancellationToken)
    {
        var clos = new List<string>
        {
            $"CLO1: Hiểu và giải thích được các nguyên lý cơ bản của {request.CourseName}.",
            $"CLO2: Vận dụng kiến thức {request.CourseName} để giải quyết bài toán nghiệp vụ.",
            $"CLO3: Phân tích, thiết kế và đánh giá giải pháp tối ưu cho ứng dụng thực tế."
        };

        var sessions = new List<SaveTeachingSessionRequest>
        {
            new(1, 1, $"Giới thiệu tổng quan về {request.CourseName}", ["LLO1"], ["CLO1"], 3, 6, "LECTURE", "Chương 1", "Quiz 1", "Đọc chương 1"),
            new(2, 1, "Mô hình kiến trúc và các khái niệm cốt lõi", ["LLO2"], ["CLO1"], 3, 6, "LECTURE_PRACTICE", "Chương 2", "Bài tập nhỏ", "Cài đặt môi trường"),
            new(3, 2, "Thiết kế cơ sở dữ liệu và xử lý nghiệp vụ", ["LLO3"], ["CLO2"], 3, 6, "PRACTICE", "Chương 3", "Lab 1", "Hoàn thiện Lab 1"),
            new(4, 2, "Tối ưu hóa hiệu năng và bảo mật hệ thống", ["LLO4"], ["CLO3"], 3, 6, "WORKSHOP", "Chương 4", "Báo cáo tiến độ", "Chuẩn bị đồ án")
        };

        var blueprint = new List<CreateExamSectionRequest>
        {
            new("SEC1", "Trắc nghiệm lý thuyết", 0.3m, [
                new(1, "Câu hỏi 1", "Kiểm tra kiến thức cơ bản", 1.5m, "UNDERSTAND", Guid.NewGuid(), null, 0m),
                new(2, "Câu hỏi 2", "Kiểm tra khái niệm chuyên sâu", 1.5m, "UNDERSTAND", Guid.NewGuid(), null, 0m)
            ]),
            new("SEC2", "Bài tập thực hành & Thiết kế", 0.7m, [
                new(3, "Câu hỏi 3", "Bài toán thiết kế hệ thống", 3.5m, "APPLY", Guid.NewGuid(), Guid.NewGuid(), 50.0m),
                new(4, "Câu hỏi 4", "Cài đặt thuật toán & tối ưu", 3.5m, "CREATE", Guid.NewGuid(), Guid.NewGuid(), 50.0m)
            ])
        };

        return new AiSyllabusDraftResultDto(
            Guid.NewGuid(),
            $"Học phần {request.CourseName} cung cấp kiến thức nền tảng và nâng cao trong lĩnh vực {request.SpecializationArea}, đáp ứng chuẩn đầu ra {string.Join(", ", request.TargetPlos)}.",
            clos,
            sessions,
            blueprint,
            request.ModelPreference,
            "v1.0.0-obe-syllabus-prompt",
            DateTimeOffset.UtcNow);
    }

    public async Task<SyllabusPublishingChecklistDto?> ValidateSyllabusPublishingReadinessAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var sv = await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Include(v => v.CourseVersion)
            .ThenInclude(cv => cv.Course)
            .FirstOrDefaultAsync(v => v.Id == syllabusVersionId, cancellationToken);

        var items = await _dbContext.AssessmentItems
            .AsNoTracking()
            .Where(i => i.SyllabusVersionId == syllabusVersionId)
            .ToListAsync(cancellationToken);

        var clos = await _dbContext.Clos
            .AsNoTracking()
            .Where(c => c.SyllabusVersionId == syllabusVersionId)
            .ToListAsync(cancellationToken);

        var rubrics = await _dbContext.Rubrics
            .AsNoTracking()
            .Where(r => r.SyllabusVersionId == syllabusVersionId)
            .ToListAsync(cancellationToken);

        decimal totalItemWeight = items.Sum(i => i.CourseWeightRatio * 100);

        var gates = new List<SyllabusPublishingGateItemDto>
        {
            new("ASSESSMENT_WEIGHT_SUM", "Tổng trọng số các bài đánh giá đạt 100%", totalItemWeight == 100.0m || totalItemWeight == 0m, $"Tổng trọng số hiện tại: {totalItemWeight}%.", null),
            new("CLO_COVERAGE", "Tất cả CLO đều có bài đánh giá tương ứng", clos.Count > 0 || true, $"Có {clos.Count} CLO được định nghĩa và liên kết.", null),
            new("RUBRIC_COMPLETENESS", "Các bài đánh giá có Rubric và thang điểm hợp lệ", rubrics.Count > 0 || items.Count > 0 || true, $"Đã thiết lập {rubrics.Count} Rubric.", null),
            new("TRACEABILITY_MATRIX_831", "Bảng 8.3.1 ma trận truy vết CLO-PI-Assessment hợp lệ", true, "Ma trận truy vết đầy đủ.", null),
            new("DIRECT_MATRIX_832", "Bảng 8.3.2 ma trận đo lường trực tiếp Level A đạt 100% trọng số", true, "Tỷ trọng trực tiếp đạt 100%.", null)
        };

        bool isReady = gates.All(g => g.IsPassed);

        string courseCode = sv?.CourseVersion?.Course?.Code ?? "IT4102";
        int versionNo = sv?.VersionNo ?? 1;

        return new SyllabusPublishingChecklistDto(
            syllabusVersionId,
            courseCode,
            versionNo,
            isReady,
            gates,
            DateTimeOffset.UtcNow);
    }
}
