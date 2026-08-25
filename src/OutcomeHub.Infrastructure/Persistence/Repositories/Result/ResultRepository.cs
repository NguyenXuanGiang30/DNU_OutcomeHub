using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Result;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Result;

public sealed class ResultRepository : IResultRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public ResultRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ResultBatchDto?> GetResultBatchByIdAsync(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var batch = await _dbContext.ResultBatches
            .AsNoTracking()
            .Include(b => b.MeasurementPeriod)
            .FirstOrDefaultAsync(b => b.Id == batchId, cancellationToken);

        if (batch == null)
        {
            return null;
        }

        return new ResultBatchDto(
            batch.Id,
            batch.MeasurementPeriodId,
            batch.MeasurementPeriod.Code,
            batch.ProgramVersionId,
            batch.BatchNo,
            batch.Status,
            batch.EngineVersion,
            batch.CompletedAt,
            batch.ResultChecksum);
    }

    public async Task<IReadOnlyList<ResultBatchDto>> GetBatchesByPeriodIdAsync(
        Guid periodId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ResultBatches
            .AsNoTracking()
            .Include(b => b.MeasurementPeriod)
            .Where(b => b.MeasurementPeriodId == periodId)
            .OrderByDescending(b => b.BatchNo)
            .Select(b => new ResultBatchDto(
                b.Id,
                b.MeasurementPeriodId,
                b.MeasurementPeriod.Code,
                b.ProgramVersionId,
                b.BatchNo,
                b.Status,
                b.EngineVersion,
                b.CompletedAt,
                b.ResultChecksum))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentCloResultDto>> GetStudentCloResultsAsync(
        Guid? batchId,
        Guid? studentId,
        Guid? courseOfferingId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StudentCloResults
            .AsNoTracking()
            .Include(c => c.Student).ThenInclude(s => s.Person)
            .Include(c => c.CourseOffering)
            .Include(c => c.Clo)
            .AsQueryable();

        if (batchId.HasValue)
        {
            query = query.Where(c => c.BatchId == batchId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(c => c.StudentId == studentId.Value);
        }

        if (courseOfferingId.HasValue)
        {
            query = query.Where(c => c.CourseOfferingId == courseOfferingId.Value);
        }

        return await query
            .OrderBy(c => c.Student.StudentCode)
            .ThenBy(c => c.Clo.Code)
            .Select(c => new StudentCloResultDto(
                c.Id,
                c.BatchId,
                c.StudentId,
                c.Student.StudentCode,
                c.Student.Person.FullName,
                c.CourseOfferingId,
                c.CourseOffering.Code,
                c.CloId,
                c.Clo.Code,
                c.Score,
                c.ThetaInd,
                c.AttainmentStatus,
                c.DataStatus))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPiResultDto>> GetStudentPiResultsAsync(
        Guid? batchId,
        Guid? studentId,
        Guid? programVersionId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StudentPiResults
            .AsNoTracking()
            .Include(p => p.Student).ThenInclude(s => s.Person)
            .Include(p => p.ProgramPi)
            .AsQueryable();

        if (batchId.HasValue)
        {
            query = query.Where(p => p.BatchId == batchId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(p => p.StudentId == studentId.Value);
        }

        if (programVersionId.HasValue)
        {
            query = query.Where(p => p.ProgramVersionId == programVersionId.Value);
        }

        return await query
            .OrderBy(p => p.Student.StudentCode)
            .ThenBy(p => p.ProgramPi.Code)
            .Select(p => new StudentPiResultDto(
                p.Id,
                p.BatchId,
                p.StudentId,
                p.Student.StudentCode,
                p.Student.Person.FullName,
                p.ProgramPiId,
                p.ProgramPi.Code,
                p.ProgramPi.Description,
                p.Score,
                p.ThetaInd,
                p.AttainmentStatus,
                p.CoreGateStatus,
                p.DataStatus))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPloResultDto>> GetStudentPloResultsAsync(
        Guid? batchId,
        Guid? studentId,
        Guid? programVersionId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StudentPloResults
            .AsNoTracking()
            .Include(p => p.Student).ThenInclude(s => s.Person)
            .Include(p => p.ProgramPlo)
            .AsQueryable();

        if (batchId.HasValue)
        {
            query = query.Where(p => p.BatchId == batchId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(p => p.StudentId == studentId.Value);
        }

        if (programVersionId.HasValue)
        {
            query = query.Where(p => p.ProgramVersionId == programVersionId.Value);
        }

        return await query
            .OrderBy(p => p.Student.StudentCode)
            .ThenBy(p => p.ProgramPlo.Code)
            .Select(p => new StudentPloResultDto(
                p.Id,
                p.BatchId,
                p.StudentId,
                p.Student.StudentCode,
                p.Student.Person.FullName,
                p.ProgramPloId,
                p.ProgramPlo.Code,
                p.ProgramPlo.Description,
                p.Score,
                p.ThetaInd,
                p.AttainmentStatus,
                p.CoreGateStatus,
                p.DataStatus))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CohortOutcomeResultDto>> GetCohortOutcomeResultsAsync(
        Guid? batchId,
        Guid? cohortId,
        string? outcomeLevel,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.CohortOutcomeResults
            .AsNoTracking()
            .Include(c => c.ProgramPlo)
            .Include(c => c.ProgramPi)
            .Include(c => c.Clo)
            .AsQueryable();

        if (batchId.HasValue)
        {
            query = query.Where(c => c.BatchId == batchId.Value);
        }

        if (cohortId.HasValue)
        {
            query = query.Where(c => c.CohortId == cohortId.Value);
        }

        if (!string.IsNullOrWhiteSpace(outcomeLevel))
        {
            query = query.Where(c => c.OutcomeLevel == outcomeLevel);
        }

        var list = await query.ToListAsync(cancellationToken);

        return list.Select(c =>
        {
            string targetCode = c.OutcomeLevel switch
            {
                "PLO" => c.ProgramPlo?.Code ?? "PLO",
                "PI" => c.ProgramPi?.Code ?? "PI",
                "CLO" => c.Clo?.Code ?? "CLO",
                _ => string.Empty
            };

            string targetDesc = c.OutcomeLevel switch
            {
                "PLO" => c.ProgramPlo?.Description ?? string.Empty,
                "PI" => c.ProgramPi?.Description ?? string.Empty,
                "CLO" => c.Clo?.Description ?? string.Empty,
                _ => string.Empty
            };

            Guid? targetId = c.OutcomeLevel switch
            {
                "PLO" => c.ProgramPloId,
                "PI" => c.ProgramPiId,
                "CLO" => c.CloId,
                _ => null
            };

            return new CohortOutcomeResultDto(
                c.Id,
                c.BatchId,
                c.ProgramVersionId,
                c.CohortId,
                c.OutcomeLevel,
                targetId,
                targetCode,
                targetDesc,
                c.PopulationCount,
                c.DenominatorCount,
                c.AttainedCount,
                c.AttainmentRate,
                c.ThetaCoh,
                c.OutcomeStatus);
        }).ToList();
    }

    public async Task<ProgramOutcomeDashboardDto?> GetProgramOutcomeDashboardAsync(
        Guid periodId,
        Guid programVersionId,
        Guid cohortId,
        CancellationToken cancellationToken)
    {
        var latestBatch = await _dbContext.ResultBatches
            .AsNoTracking()
            .Include(b => b.MeasurementPeriod)
            .Include(b => b.ProgramVersion).ThenInclude(v => v.Program)
            .Where(b => b.MeasurementPeriodId == periodId && b.ProgramVersionId == programVersionId)
            .OrderByDescending(b => b.BatchNo)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestBatch == null)
        {
            return null;
        }

        var cohort = await _dbContext.Cohorts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cohortId, cancellationToken);

        var cohortResults = await GetCohortOutcomeResultsAsync(
            latestBatch.Id,
            cohortId,
            outcomeLevel: null,
            cancellationToken);

        var ploResults = cohortResults.Where(r => r.OutcomeLevel == "PLO").ToList();
        var piResults = cohortResults.Where(r => r.OutcomeLevel == "PI").ToList();

        int totalPlos = ploResults.Count;
        int attainedPlos = ploResults.Count(p => p.OutcomeStatus == "ATTAINED");
        decimal attainmentRate = totalPlos > 0 ? (decimal)attainedPlos / totalPlos * 100m : 0m;
        int totalStudents = (int)(ploResults.FirstOrDefault()?.PopulationCount ?? 0);

        return new ProgramOutcomeDashboardDto(
            programVersionId,
            latestBatch.ProgramVersion.Program.Name,
            latestBatch.ProgramVersion.Code,
            cohortId,
            cohort?.Code ?? "COHORT",
            periodId,
            latestBatch.MeasurementPeriod.Code,
            totalStudents,
            totalPlos,
            attainedPlos,
            attainmentRate,
            ploResults,
            piResults);
    }

    public async Task<ResultBatchDto> SaveCalculationBatchAsync(
        InputSnapshot inputSnapshot,
        ResultBatch resultBatch,
        IReadOnlyList<StudentCloResult> cloResults,
        IReadOnlyList<StudentPiResult> piResults,
        IReadOnlyList<StudentPloResult> ploResults,
        IReadOnlyList<CohortOutcomeResult> cohortResults,
        CancellationToken cancellationToken)
    {
        // 0. Ensure governed_resource rows exist
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({inputSnapshot.GovernedResourceId}, 'measurement.input_snapshot', 'CONFIDENTIAL', 'ACTIVE', CURRENT_TIMESTAMP) ON CONFLICT (id) DO NOTHING",
            cancellationToken);

        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({resultBatch.GovernedResourceId}, 'result.result_batch', 'CONFIDENTIAL', 'ACTIVE', CURRENT_TIMESTAMP) ON CONFLICT (id) DO NOTHING",
            cancellationToken);

        // 1. Insert snapshot in BUILDING status
        var sealedChecksum = inputSnapshot.ManifestChecksum;
        var sealedBy = inputSnapshot.SealedBy;
        var sealedAt = inputSnapshot.SealedAt;

        var buildingSnapshot = InputSnapshot.CreateBuilding(
            inputSnapshot.Id,
            inputSnapshot.GovernedResourceId,
            inputSnapshot.MeasurementPeriodId,
            inputSnapshot.OrgUnitId,
            inputSnapshot.SnapshotNo,
            inputSnapshot.PolicyVersionId,
            inputSnapshot.ProgramPolicyBindingId,
            inputSnapshot.InstitutionTemplateVersionId,
            inputSnapshot.ProgramVersionId,
            inputSnapshot.AcademicYearStart,
            inputSnapshot.SchemaVersion,
            inputSnapshot.HashAlgorithm,
            inputSnapshot.PopulationCount,
            inputSnapshot.ScoreCount,
            inputSnapshot.CreatedBy,
            inputSnapshot.CreatedAt);

        await _dbContext.InputSnapshots.AddAsync(buildingSnapshot, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 2. Seal snapshot
        if (sealedChecksum != null && sealedBy.HasValue && sealedAt.HasValue)
        {
            buildingSnapshot.Seal(sealedChecksum, sealedBy.Value, sealedAt.Value);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // 3. Insert ResultBatch in RUNNING status
        var batchResultChecksum = resultBatch.ResultChecksum;
        var batchCompletedAt = resultBatch.CompletedAt;

        var runningBatch = ResultBatch.CreateRunning(
            resultBatch.Id,
            resultBatch.GovernedResourceId,
            resultBatch.MeasurementPeriodId,
            resultBatch.InputSnapshotId,
            resultBatch.PolicyVersionId,
            resultBatch.ProgramPolicyBindingId,
            resultBatch.OrgUnitId,
            resultBatch.ProgramVersionId,
            resultBatch.AcademicYearStart,
            resultBatch.BatchNo,
            resultBatch.EngineVersion,
            resultBatch.SourceCommit,
            resultBatch.IdempotencyKey,
            resultBatch.RequestChecksum,
            resultBatch.WorkflowInstanceId,
            resultBatch.SodPolicyVersionId,
            resultBatch.StartedAt ?? DateTimeOffset.UtcNow);

        await _dbContext.ResultBatches.AddAsync(runningBatch, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 4. Insert Result Details
        if (cloResults.Count > 0)
        {
            await _dbContext.StudentCloResults.AddRangeAsync(cloResults, cancellationToken);
        }

        if (piResults.Count > 0)
        {
            await _dbContext.StudentPiResults.AddRangeAsync(piResults, cancellationToken);
        }

        if (ploResults.Count > 0)
        {
            await _dbContext.StudentPloResults.AddRangeAsync(ploResults, cancellationToken);
        }

        if (cohortResults.Count > 0)
        {
            await _dbContext.CohortOutcomeResults.AddRangeAsync(cohortResults, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        // 5. Complete ResultBatch to CALCULATED
        if (batchResultChecksum != null && batchCompletedAt.HasValue)
        {
            runningBatch.Complete(batchResultChecksum, batchCompletedAt.Value);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new ResultBatchDto(
            resultBatch.Id,
            resultBatch.MeasurementPeriodId,
            string.Empty,
            resultBatch.ProgramVersionId,
            resultBatch.BatchNo,
            runningBatch.Status,
            resultBatch.EngineVersion,
            runningBatch.CompletedAt,
            runningBatch.ResultChecksum);
    }
}
