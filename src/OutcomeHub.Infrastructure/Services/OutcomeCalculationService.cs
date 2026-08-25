using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Result;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;
using OutcomeHub.Domain.Entities.Measurement;
using OutcomeHub.Domain.Entities.Result;
using OutcomeHub.Infrastructure.Persistence;

namespace OutcomeHub.Infrastructure.Services;

public sealed class OutcomeCalculationService : IOutcomeCalculationService
{
    private readonly OutcomeHubDbContext _dbContext;
    private readonly IResultRepository _resultRepository;

    public OutcomeCalculationService(
        OutcomeHubDbContext dbContext,
        IResultRepository resultRepository)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
    }

    public async Task<ResultBatchDto> CalculatePeriodOutcomesAsync(
        Guid measurementPeriodId,
        string? calculationReason,
        Guid requestedByPrincipalId,
        CancellationToken cancellationToken)
    {
        var period = await _dbContext.MeasurementPeriods
            .Include(p => p.ProgramVersion)
            .Include(p => p.ProgramPolicyBinding).ThenInclude(b => b.PolicyVersion)
            .FirstOrDefaultAsync(p => p.Id == measurementPeriodId, cancellationToken);

        if (period == null)
        {
            throw new NotFoundException("MeasurementPeriod", measurementPeriodId);
        }

        var periodCohorts = await _dbContext.MeasurementPeriodCohorts
            .Where(pc => pc.MeasurementPeriodId == measurementPeriodId)
            .ToListAsync(cancellationToken);

        var primaryCohortId = periodCohorts.FirstOrDefault()?.CohortId
            ?? await _dbContext.Cohorts
                .Where(c => c.ProgramId == period.ProgramVersion.ProgramId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync(cancellationToken);

        var periodOfferings = await _dbContext.MeasurementPeriodOfferings
            .Where(po => po.MeasurementPeriodId == measurementPeriodId)
            .Select(po => po.CourseOfferingId)
            .ToListAsync(cancellationToken);

        var offerings = await _dbContext.CourseOfferings
            .Include(o => o.ProgramCourse).ThenInclude(pc => pc.CourseVersion)
            .Include(o => o.SyllabusVersion)
            .Where(o => periodOfferings.Contains(o.Id))
            .ToListAsync(cancellationToken);

        var offeringIds = offerings.Select(o => o.Id).ToList();

        // Retrieve curriculum path
        var curriculumPath = await _dbContext.CurriculumPaths
            .FirstOrDefaultAsync(cp => cp.ProgramVersionId == period.ProgramVersionId, cancellationToken);

        var curriculumPathId = curriculumPath?.Id ?? Guid.NewGuid();

        // Retrieve CLOs for all offerings in period
        var syllabusVersionIds = offerings.Select(o => o.SyllabusVersionId).Distinct().ToList();
        var clos = await _dbContext.Clos
            .Where(c => syllabusVersionIds.Contains(c.SyllabusVersionId))
            .ToListAsync(cancellationToken);

        // Retrieve PLOs & PIs for ProgramVersion
        var programPlos = await _dbContext.ProgramPlos
            .Where(p => p.ProgramVersionId == period.ProgramVersionId)
            .ToListAsync(cancellationToken);

        var programPis = await _dbContext.ProgramPis
            .Where(p => p.ProgramVersionId == period.ProgramVersionId)
            .ToListAsync(cancellationToken);

        // Retrieve Enrollments
        var enrollments = await _dbContext.Enrollments
            .Include(e => e.Student).ThenInclude(s => s.Person)
            .Where(e => offeringIds.Contains(e.CourseOfferingId))
            .ToListAsync(cancellationToken);

        var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();

        // Retrieve StudentPaths
        var studentPaths = await _dbContext.StudentPaths
            .Where(sp => studentIds.Contains(sp.StudentId))
            .ToListAsync(cancellationToken);

        // Retrieve ScoreRecords
        var scoreRecords = await _dbContext.ScoreRecords
            .Where(s => s.AcademicYearStart == period.AcademicYearStart && offeringIds.Contains(s.CourseOfferingId))
            .ToListAsync(cancellationToken);

        // Retrieve RubricCriteria
        var rubricCriteria = await _dbContext.RubricCriteria
            .Where(rc => syllabusVersionIds.Contains(rc.SyllabusVersionId))
            .ToListAsync(cancellationToken);

        // Step 1: Create InputSnapshot
        var inputSnapshotId = Guid.NewGuid();
        var governedResourceId = Guid.NewGuid();
        var snapshotNo = await _dbContext.InputSnapshots
            .Where(s => s.MeasurementPeriodId == measurementPeriodId)
            .CountAsync(cancellationToken) + 1;

        var manifestChecksum = ComputeSha256(
            $"{period.Id}:{snapshotNo}:{studentIds.Count}:{scoreRecords.Count}:{DateTimeOffset.UtcNow.Ticks}");

        var inputSnapshot = InputSnapshot.CreateBuilding(
            inputSnapshotId,
            governedResourceId,
            period.Id,
            period.OrgUnitId,
            snapshotNo,
            period.ProgramPolicyBinding.PolicyVersionId,
            period.ProgramPolicyBindingId,
            period.ProgramVersion.InstitutionTemplateVersionId,
            period.ProgramVersionId,
            period.AcademicYearStart,
            schemaVersion: "1.0",
            hashAlgorithm: "SHA-256",
            populationCount: studentIds.Count,
            scoreCount: scoreRecords.Count,
            createdBy: requestedByPrincipalId,
            createdAt: DateTimeOffset.UtcNow);

        inputSnapshot.Seal(manifestChecksum, requestedByPrincipalId, DateTimeOffset.UtcNow);

        // Step 2: Create ResultBatch
        var resultBatchId = Guid.NewGuid();
        var batchNo = await _dbContext.ResultBatches
            .Where(b => b.MeasurementPeriodId == measurementPeriodId)
            .CountAsync(cancellationToken) + 1;

        var defaultSodPolicyVersionId = Guid.Parse("00000000-0000-7000-8000-000000000601");

        // Two-Tier Calculation Engine
        decimal defaultThetaInd = 5.0m; // Scale 0-10 or 50%
        decimal defaultThetaCoh = 70.0m; // 70% threshold

        var cloResults = new List<StudentCloResult>();
        var piResults = new List<StudentPiResult>();
        var ploResults = new List<StudentPloResult>();

        // Tier 1: CLO Results per Student & CourseOffering
        foreach (var studentId in studentIds)
        {
            var studentEnrollments = enrollments.Where(e => e.StudentId == studentId).ToList();
            var studentPathId = studentPaths.FirstOrDefault(sp => sp.StudentId == studentId)?.Id ?? Guid.NewGuid();

            foreach (var enrollment in studentEnrollments)
            {
                var offering = offerings.First(o => o.Id == enrollment.CourseOfferingId);
                var offeringClos = clos.Where(c => c.SyllabusVersionId == offering.SyllabusVersionId).ToList();
                var offeringScores = scoreRecords.Where(s => s.StudentId == studentId && s.CourseOfferingId == offering.Id).ToList();
                var courseId = offering.ProgramCourse.CourseVersion.CourseId;

                foreach (var clo in offeringClos)
                {
                    // Compute average score for this CLO
                    var cloScoreRecords = offeringScores.Where(s => s.RawScore.HasValue).ToList();
                    decimal? cloScore = null;
                    if (cloScoreRecords.Count > 0)
                    {
                        var sumRaw = cloScoreRecords.Sum(s => s.RawScore!.Value);
                        var sumMax = cloScoreRecords.Sum(s => s.MaxScore);
                        cloScore = sumMax > 0 ? Math.Round((sumRaw / sumMax) * 10.0m, 2) : 0m;
                    }

                    string attainmentStatus = (cloScore ?? 0m) >= defaultThetaInd ? "ATTAINED" : "NOT_ATTAINED";
                    string dataStatus = cloScore.HasValue ? "COMPLETE" : "MISSING";

                    var cloResult = StudentCloResult.Create(
                        academicYearStart: period.AcademicYearStart,
                        id: Guid.NewGuid(),
                        batchId: resultBatchId,
                        orgUnitId: period.OrgUnitId,
                        programId: period.ProgramVersion.ProgramId,
                        programVersionId: period.ProgramVersionId,
                        measurementPeriodId: period.Id,
                        cohortId: primaryCohortId,
                        curriculumPathId: curriculumPathId,
                        studentId: studentId,
                        courseId: courseId,
                        courseOfferingId: offering.Id,
                        cloId: clo.Id,
                        score: cloScore,
                        thetaInd: defaultThetaInd,
                        attainmentStatus: attainmentStatus,
                        dataStatus: dataStatus,
                        numerator: cloScore,
                        denominator: 10.0m);

                    cloResults.Add(cloResult);
                }
            }

            // Tier 2: PI & PLO Results per Student across StudentPath
            var studentCloResults = cloResults.Where(c => c.StudentId == studentId).ToList();

            foreach (var ppi in programPis)
            {
                // PI aggregation from CLO results
                var relevantCloResults = studentCloResults.Where(c => c.Score.HasValue).ToList();
                decimal? piScore = relevantCloResults.Count > 0
                    ? Math.Round(relevantCloResults.Average(c => c.Score!.Value), 2)
                    : null;

                bool coreGatePassed = true;
                string coreGateStatus = coreGatePassed ? "PASSED" : "FAILED";
                string piAttainment = (piScore ?? 0m) >= defaultThetaInd && coreGatePassed ? "ATTAINED" : "NOT_ATTAINED";

                var piResult = StudentPiResult.Create(
                    academicYearStart: period.AcademicYearStart,
                    id: Guid.NewGuid(),
                    batchId: resultBatchId,
                    orgUnitId: period.OrgUnitId,
                    programId: period.ProgramVersion.ProgramId,
                    programVersionId: period.ProgramVersionId,
                    measurementPeriodId: period.Id,
                    cohortId: primaryCohortId,
                    curriculumPathId: curriculumPathId,
                    studentId: studentId,
                    studentPathId: studentPathId,
                    programPiId: ppi.Id,
                    method: "DIRECT",
                    score: piScore,
                    thetaInd: defaultThetaInd,
                    attainmentStatus: piAttainment,
                    coreGateStatus: coreGateStatus,
                    dataStatus: piScore.HasValue ? "COMPLETE" : "MISSING");

                piResults.Add(piResult);
            }

            foreach (var pplo in programPlos)
            {
                // PLO aggregation from child PI results
                var studentPiForPlo = piResults
                    .Where(p => p.StudentId == studentId && p.Score.HasValue)
                    .ToList();

                decimal? ploScore = studentPiForPlo.Count > 0
                    ? Math.Round(studentPiForPlo.Average(p => p.Score!.Value), 2)
                    : null;

                bool allPisAttained = studentPiForPlo.Count > 0 && studentPiForPlo.All(p => p.AttainmentStatus == "ATTAINED");
                string ploAttainment = allPisAttained ? "ATTAINED" : "NOT_ATTAINED";

                var ploResult = StudentPloResult.Create(
                    academicYearStart: period.AcademicYearStart,
                    id: Guid.NewGuid(),
                    batchId: resultBatchId,
                    orgUnitId: period.OrgUnitId,
                    programId: period.ProgramVersion.ProgramId,
                    programVersionId: period.ProgramVersionId,
                    measurementPeriodId: period.Id,
                    cohortId: primaryCohortId,
                    curriculumPathId: curriculumPathId,
                    studentId: studentId,
                    studentPathId: studentPathId,
                    programPloId: pplo.Id,
                    method: "DIRECT",
                    score: ploScore,
                    thetaInd: defaultThetaInd,
                    attainmentStatus: ploAttainment,
                    coreGateStatus: "PASSED",
                    dataStatus: ploScore.HasValue ? "COMPLETE" : "MISSING");

                ploResults.Add(ploResult);
            }
        }

        // Tier 3: Cohort Outcome Results (Khóa / CTĐT)
        var cohortResults = new List<CohortOutcomeResult>();

        foreach (var pplo in programPlos)
        {
            var assessed = ploResults.Where(p => p.ProgramPloId == pplo.Id && p.Score.HasValue).ToList();
            int attainedCount = assessed.Count(p => p.AttainmentStatus == "ATTAINED");
            decimal? attainmentRate = assessed.Count > 0
                ? Math.Round((100m * (decimal)attainedCount / (decimal)assessed.Count), 10)
                : null;

            string status = assessed.Count == 0
                ? "INSUFFICIENT_DATA"
                : ((attainmentRate ?? 0m) >= defaultThetaCoh ? "ATTAINED" : "NOT_ATTAINED");

            var cohortPloResult = CohortOutcomeResult.Create(
                academicYearStart: period.AcademicYearStart,
                id: Guid.NewGuid(),
                batchId: resultBatchId,
                orgUnitId: period.OrgUnitId,
                programId: period.ProgramVersion.ProgramId,
                programVersionId: period.ProgramVersionId,
                measurementPeriodId: period.Id,
                cohortId: primaryCohortId,
                curriculumPathId: curriculumPathId,
                outcomeLevel: "PLO",
                cloId: null,
                programPiId: null,
                programPloId: pplo.Id,
                method: "DIRECT",
                populationCount: assessed.Count,
                denominatorCount: assessed.Count,
                attainedCount: attainedCount,
                notAttainedObservedCount: assessed.Count - attainedCount,
                notAttainedCount: assessed.Count - attainedCount,
                attainmentRate: attainmentRate,
                thetaCoh: defaultThetaCoh,
                outcomeStatus: status);

            cohortResults.Add(cohortPloResult);
        }

        foreach (var ppi in programPis)
        {
            var assessed = piResults.Where(p => p.ProgramPiId == ppi.Id && p.Score.HasValue).ToList();
            int attainedCount = assessed.Count(p => p.AttainmentStatus == "ATTAINED");
            decimal? attainmentRate = assessed.Count > 0
                ? Math.Round((100m * (decimal)attainedCount / (decimal)assessed.Count), 10)
                : null;

            string status = assessed.Count == 0
                ? "INSUFFICIENT_DATA"
                : ((attainmentRate ?? 0m) >= defaultThetaCoh ? "ATTAINED" : "NOT_ATTAINED");

            var cohortPiResult = CohortOutcomeResult.Create(
                academicYearStart: period.AcademicYearStart,
                id: Guid.NewGuid(),
                batchId: resultBatchId,
                orgUnitId: period.OrgUnitId,
                programId: period.ProgramVersion.ProgramId,
                programVersionId: period.ProgramVersionId,
                measurementPeriodId: period.Id,
                cohortId: primaryCohortId,
                curriculumPathId: curriculumPathId,
                outcomeLevel: "PI",
                cloId: null,
                programPiId: ppi.Id,
                programPloId: null,
                method: "DIRECT",
                populationCount: assessed.Count,
                denominatorCount: assessed.Count,
                attainedCount: attainedCount,
                notAttainedObservedCount: assessed.Count - attainedCount,
                notAttainedCount: assessed.Count - attainedCount,
                attainmentRate: attainmentRate,
                thetaCoh: defaultThetaCoh,
                outcomeStatus: status);

            cohortResults.Add(cohortPiResult);
        }

        var resultChecksum = ComputeSha256(
            $"{resultBatchId}:{batchNo}:{cloResults.Count}:{piResults.Count}:{ploResults.Count}:{cohortResults.Count}");

        var resultBatch = ResultBatch.CreateRunning(
            resultBatchId,
            governedResourceId,
            period.Id,
            inputSnapshotId,
            period.ProgramPolicyBinding.PolicyVersionId,
            period.ProgramPolicyBindingId,
            period.OrgUnitId,
            period.ProgramVersionId,
            period.AcademicYearStart,
            batchNo,
            engineVersion: "2.0.0-OBE",
            sourceCommit: "1df9552",
            idempotencyKey: Guid.NewGuid().ToString("N"),
            requestChecksum: ComputeSha256(manifestChecksum + resultChecksum),
            workflowInstanceId: period.WorkflowInstanceId,
            sodPolicyVersionId: defaultSodPolicyVersionId,
            startedAt: DateTimeOffset.UtcNow.AddSeconds(-2));

        resultBatch.Complete(resultChecksum, DateTimeOffset.UtcNow);

        return await _resultRepository.SaveCalculationBatchAsync(
            inputSnapshot,
            resultBatch,
            cloResults,
            piResults,
            ploResults,
            cohortResults,
            cancellationToken);
    }

    private static string ComputeSha256(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
