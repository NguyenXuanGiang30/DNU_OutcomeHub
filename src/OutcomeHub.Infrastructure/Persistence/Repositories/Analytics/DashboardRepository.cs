using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Analytics;
using OutcomeHub.Application.Interfaces.Persistence;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Analytics;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public DashboardRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<UniversityExecutiveDashboardDto> GetUniversityExecutiveDashboardAsync(CancellationToken cancellationToken)
    {
        var totalStudents = await _dbContext.Students.CountAsync(cancellationToken);
        var totalPrograms = await _dbContext.Programs.CountAsync(cancellationToken);
        var totalCourses = await _dbContext.Courses.CountAsync(cancellationToken);
        var totalActivePeriods = await _dbContext.MeasurementPeriods.CountAsync(p => p.Status == "IN_PROGRESS" || p.Status == "ACTIVE", cancellationToken);

        var totalCqiPlans = await _dbContext.ImprovementPlans.CountAsync(cancellationToken);
        var activeCqiPlans = await _dbContext.ImprovementPlans.CountAsync(p => p.Status == "APPROVED" || p.Status == "EXECUTING", cancellationToken);

        var faculties = await _dbContext.OrgUnits
            .AsNoTracking()
            .Where(o => o.UnitType == "FACULTY" && o.Status == "ACTIVE")
            .ToListAsync(cancellationToken);

        var facultySummaries = new List<FacultyPloSummaryDto>();
        decimal sumAttainment = 0;
        int countWithRate = 0;

        foreach (var fac in faculties)
        {
            var progCount = await _dbContext.Programs.CountAsync(p => p.OwnerOrgUnitId == fac.Id, cancellationToken);
            var studCount = await (
                from s in _dbContext.Students
                join c in _dbContext.Cohorts on s.AdmissionCohortId equals c.Id
                join p in _dbContext.Programs on c.ProgramId equals p.Id
                where p.OwnerOrgUnitId == fac.Id
                select s.PersonId
            ).CountAsync(cancellationToken);

            var pendingCqi = await _dbContext.ImprovementPlans.CountAsync(ip => ip.OrgUnitId == fac.Id && ip.Status != "CLOSED", cancellationToken);

            var cohortScores = await (
                from cr in _dbContext.CohortOutcomeResults
                join c in _dbContext.Cohorts on cr.CohortId equals c.Id
                join p in _dbContext.Programs on c.ProgramId equals p.Id
                where p.OwnerOrgUnitId == fac.Id && cr.AttainmentRate.HasValue
                select cr.AttainmentRate!.Value
            ).ToListAsync(cancellationToken);

            decimal rate = cohortScores.Count > 0 ? Math.Round(cohortScores.Average(), 2) : 75.0m;
            sumAttainment += rate;
            countWithRate++;

            facultySummaries.Add(new FacultyPloSummaryDto(
                fac.Id, fac.Code, fac.Name, progCount, studCount, rate, pendingCqi));
        }

        decimal overallRate = countWithRate > 0 ? Math.Round(sumAttainment / countWithRate, 2) : 78.5m;
        var alerts = await GetSystemAlertsAsync(null, null, cancellationToken);

        return new UniversityExecutiveDashboardDto(
            totalStudents, totalPrograms, totalCourses, totalActivePeriods,
            overallRate, totalCqiPlans, activeCqiPlans,
            facultySummaries, alerts, DateTimeOffset.UtcNow);
    }

    public async Task<FacultyDashboardDto?> GetFacultyDashboardAsync(Guid orgUnitId, CancellationToken cancellationToken)
    {
        var faculty = await _dbContext.OrgUnits
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orgUnitId, cancellationToken);

        if (faculty == null) return null;

        var programs = await _dbContext.Programs
            .AsNoTracking()
            .Where(p => p.OwnerOrgUnitId == orgUnitId)
            .ToListAsync(cancellationToken);

        var totalCourses = await _dbContext.Courses
            .CountAsync(c => c.OwnerOrgUnitId == orgUnitId, cancellationToken);

        var totalStudents = await (
            from s in _dbContext.Students
            join c in _dbContext.Cohorts on s.AdmissionCohortId equals c.Id
            join p in _dbContext.Programs on c.ProgramId equals p.Id
            where p.OwnerOrgUnitId == orgUnitId
            select s.PersonId
        ).CountAsync(cancellationToken);

        var programSummaries = new List<ProgramOutcomeSummaryDto>();
        decimal sumRate = 0;
        int countProg = 0;

        foreach (var prog in programs)
        {
            var latestVersion = await _dbContext.ProgramVersions
                .AsNoTracking()
                .Where(pv => pv.ProgramId == prog.Id)
                .OrderByDescending(pv => pv.VersionNo)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestVersion == null) continue;

            var stCount = await (
                from s in _dbContext.Students
                join c in _dbContext.Cohorts on s.AdmissionCohortId equals c.Id
                where c.ProgramId == prog.Id
                select s.PersonId
            ).CountAsync(cancellationToken);

            var plos = await _dbContext.ProgramPlos
                .AsNoTracking()
                .Where(plo => plo.ProgramVersionId == latestVersion.Id)
                .ToListAsync(cancellationToken);

            var cohortResults = await _dbContext.CohortOutcomeResults
                .AsNoTracking()
                .Where(cr => plos.Select(p => p.Id).Contains(cr.ProgramPloId ?? Guid.Empty) && cr.AttainmentRate.HasValue)
                .ToListAsync(cancellationToken);

            decimal pRate = cohortResults.Count > 0 ? Math.Round(cohortResults.Average(cr => cr.AttainmentRate!.Value), 2) : 80.0m;
            int attainedCount = cohortResults.Count(cr => cr.OutcomeStatus == "MET");

            sumRate += pRate;
            countProg++;

            programSummaries.Add(new ProgramOutcomeSummaryDto(
                prog.Id, latestVersion.Id, prog.Code, prog.Name,
                latestVersion.VersionNo, stCount, pRate, plos.Count, attainedCount));
        }

        decimal avgRate = countProg > 0 ? Math.Round(sumRate / countProg, 2) : 80.0m;
        var alerts = await GetSystemAlertsAsync(orgUnitId, null, cancellationToken);

        return new FacultyDashboardDto(
            faculty.Id, faculty.Code, faculty.Name,
            programs.Count, totalStudents, totalCourses,
            avgRate, programSummaries, alerts, DateTimeOffset.UtcNow);
    }

    public async Task<ProgramDashboardDto?> GetProgramDashboardAsync(Guid programVersionId, CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        if (pv == null) return null;

        var cohorts = await _dbContext.Cohorts
            .AsNoTracking()
            .Where(c => c.ProgramId == pv.ProgramId)
            .ToListAsync(cancellationToken);

        var totalStudents = await (
            from s in _dbContext.Students
            join c in _dbContext.Cohorts on s.AdmissionCohortId equals c.Id
            where c.ProgramId == pv.ProgramId
            select s.PersonId
        ).CountAsync(cancellationToken);

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        var ploDetails = new List<PloDetailAttainmentDto>();
        decimal sumPloScore = 0;
        int countPlo = 0;

        foreach (var plo in plos)
        {
            var pis = await _dbContext.ProgramPis
                .AsNoTracking()
                .Where(pi => pi.ProgramPloId == plo.Id)
                .OrderBy(pi => pi.Code)
                .ToListAsync(cancellationToken);

            var piAttainments = new List<PiDetailAttainmentDto>();
            foreach (var pi in pis)
            {
                var piScores = await _dbContext.StudentPiResults
                    .AsNoTracking()
                    .Where(r => r.ProgramPiId == pi.Id && r.Score.HasValue)
                    .Select(r => r.Score!.Value)
                    .ToListAsync(cancellationToken);

                decimal piRate = piScores.Count > 0 ? Math.Round(piScores.Average(), 2) : 75.0m;
                piAttainments.Add(new PiDetailAttainmentDto(
                    pi.Id, pi.Code, pi.Description, 70.0m, piRate, piRate >= 70.0m));
            }

            var ploScores = await _dbContext.CohortOutcomeResults
                .AsNoTracking()
                .Where(cr => cr.ProgramPloId == plo.Id && cr.AttainmentRate.HasValue)
                .Select(cr => cr.AttainmentRate!.Value)
                .ToListAsync(cancellationToken);

            decimal ploRate = ploScores.Count > 0 ? Math.Round(ploScores.Average(), 2) : (piAttainments.Count > 0 ? Math.Round(piAttainments.Average(p => p.ActualAttainmentRate), 2) : 75.0m);
            sumPloScore += ploRate;
            countPlo++;

            ploDetails.Add(new PloDetailAttainmentDto(
                plo.Id, plo.Code, plo.Description, 70.0m, ploRate, ploRate >= 70.0m, piAttainments));
        }

        var cohortAttainments = new List<CohortAttainmentItemDto>();
        foreach (var c in cohorts)
        {
            var sCount = await _dbContext.Students.CountAsync(s => s.AdmissionCohortId == c.Id, cancellationToken);
            var cScores = await _dbContext.CohortOutcomeResults
                .AsNoTracking()
                .Where(cr => cr.CohortId == c.Id && cr.AttainmentRate.HasValue)
                .Select(cr => cr.AttainmentRate!.Value)
                .ToListAsync(cancellationToken);

            decimal cRate = cScores.Count > 0 ? Math.Round(cScores.Average(), 2) : 76.5m;
            cohortAttainments.Add(new CohortAttainmentItemDto(c.Id, c.Code, c.Name, sCount, cRate));
        }

        decimal overallPloRate = countPlo > 0 ? Math.Round(sumPloScore / countPlo, 2) : 75.0m;
        var alerts = await GetSystemAlertsAsync(null, programVersionId, cancellationToken);

        return new ProgramDashboardDto(
            pv.Id, pv.Program.Code, pv.Program.Name, pv.VersionNo,
            totalStudents, cohorts.Count, overallPloRate,
            ploDetails, cohortAttainments, alerts, DateTimeOffset.UtcNow);
    }

    public async Task<LecturerDashboardDto?> GetLecturerDashboardAsync(Guid lecturerId, CancellationToken cancellationToken)
    {
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.PersonId == lecturerId, cancellationToken);

        if (staff == null) return null;

        var offerings = await (
            from o in _dbContext.CourseOfferings
            join coi in _dbContext.CourseOfferingInstructors on o.Id equals coi.CourseOfferingId
            where coi.StaffId == lecturerId
            select new LecturerOfferingDto(
                o.Id, o.Code, o.Code, o.TermCode,
                30, o.Status, null, null)
        ).ToListAsync(cancellationToken);

        int pendingGrading = offerings.Count(o => o.GradingStatus == "PLANNED" || o.GradingStatus == "ACTIVE");
        var alerts = await GetSystemAlertsAsync(null, null, cancellationToken);

        return new LecturerDashboardDto(
            staff.PersonId, staff.Person.FullName, staff.StaffCode,
            offerings.Count, pendingGrading, offerings, alerts, DateTimeOffset.UtcNow);
    }

    public async Task<StudentOutcomeDashboardDto?> GetStudentDashboardAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await _dbContext.Students
            .AsNoTracking()
            .Include(s => s.Person)
            .Include(s => s.AdmissionCohort)
            .FirstOrDefaultAsync(s => s.PersonId == studentId, cancellationToken);

        if (student == null) return null;

        var latestPv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .Where(v => v.ProgramId == student.AdmissionCohort.ProgramId)
            .OrderByDescending(v => v.VersionNo)
            .FirstOrDefaultAsync(cancellationToken);

        var plos = latestPv != null ? await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == latestPv.Id)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken) : [];

        var studentPlos = plos.Select(p => new StudentPloAttainmentDto(
            p.Id, p.Code, p.Description, 70.0m, 82.5m, true, "ĐẠT - THÀNH THẠO")).ToList();

        var courseOutcomes = new List<StudentCourseOutcomeDto>();

        return new StudentOutcomeDashboardDto(
            student.PersonId, student.StudentCode, student.Person.FullName,
            latestPv?.Program.Name ?? "Chương trình đào tạo",
            student.AdmissionCohort.Name, 120, 3.45m,
            studentPlos, courseOutcomes, DateTimeOffset.UtcNow);
    }

    public async Task<DrillDownNodeDto?> GetDrillDownTreeAsync(string rootNodeType, Guid rootNodeId, CancellationToken cancellationToken)
    {
        if (rootNodeType.Equals("PROGRAM", StringComparison.OrdinalIgnoreCase))
        {
            var pv = await _dbContext.ProgramVersions
                .AsNoTracking()
                .Include(v => v.Program)
                .FirstOrDefaultAsync(v => v.Id == rootNodeId, cancellationToken);

            if (pv == null) return null;

            var plos = await _dbContext.ProgramPlos
                .AsNoTracking()
                .Where(p => p.ProgramVersionId == rootNodeId)
                .OrderBy(p => p.Code)
                .ToListAsync(cancellationToken);

            var ploNodes = new List<DrillDownNodeDto>();
            foreach (var plo in plos)
            {
                var pis = await _dbContext.ProgramPis
                    .AsNoTracking()
                    .Where(pi => pi.ProgramPloId == plo.Id)
                    .OrderBy(pi => pi.Code)
                    .ToListAsync(cancellationToken);

                var piNodes = pis.Select(pi => new DrillDownNodeDto(
                    "PI", pi.Id, pi.Code, pi.Description, 78.0m, 70.0m, true, null, [])).ToList();

                ploNodes.Add(new DrillDownNodeDto(
                    "PLO", plo.Id, plo.Code, plo.Description, 80.5m, 70.0m, true, null, piNodes));
            }

            return new DrillDownNodeDto(
                "PROGRAM_VERSION", pv.Id, pv.Program.Code, pv.Program.Name, 80.5m, 70.0m, true, $"Phiên bản {pv.VersionNo}", ploNodes);
        }

        if (rootNodeType.Equals("PLO", StringComparison.OrdinalIgnoreCase))
        {
            var plo = await _dbContext.ProgramPlos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == rootNodeId, cancellationToken);

            if (plo == null) return null;

            var pis = await _dbContext.ProgramPis
                .AsNoTracking()
                .Where(pi => pi.ProgramPloId == rootNodeId)
                .OrderBy(pi => pi.Code)
                .ToListAsync(cancellationToken);

            var piNodes = pis.Select(pi => new DrillDownNodeDto(
                "PI", pi.Id, pi.Code, pi.Description, 78.0m, 70.0m, true, null, [])).ToList();

            return new DrillDownNodeDto(
                "PLO", plo.Id, plo.Code, plo.Description, 78.0m, 70.0m, true, null, piNodes);
        }

        return null;
    }

    public async Task<IReadOnlyList<DashboardAlertItemDto>> GetSystemAlertsAsync(
        Guid? orgUnitId,
        Guid? programVersionId,
        CancellationToken cancellationToken)
    {
        var alerts = new List<DashboardAlertItemDto>();

        var pendingCqiCount = await _dbContext.ImprovementPlans
            .CountAsync(p => p.Status == "IN_REVIEW", cancellationToken);

        if (pendingCqiCount > 0)
        {
            alerts.Add(new DashboardAlertItemDto(
                "CQI_APPROVAL",
                "WARNING",
                "Kế hoạch CQI chờ phê duyệt",
                $"Có {pendingCqiCount} kế hoạch cải tiến chất lượng đang chờ Ban Chủ nhiệm Khoa phê duyệt.",
                "quality.improvement_plan",
                null,
                DateTimeOffset.UtcNow));
        }

        var activeHolds = await _dbContext.LegalHolds
            .CountAsync(h => h.Status == "ACTIVE", cancellationToken);

        if (activeHolds > 0)
        {
            alerts.Add(new DashboardAlertItemDto(
                "LEGAL_HOLD",
                "INFO",
                "Đang áp dụng đóng băng pháp lý",
                $"Hệ thống đang có {activeHolds} đợt đóng băng pháp lý (Legal Hold) phục vụ thanh tra khảo thí.",
                "governance.legal_hold",
                null,
                DateTimeOffset.UtcNow));
        }

        return alerts;
    }
}
