using Microsoft.EntityFrameworkCore;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;
using OutcomeHub.Domain.Entities.Measurement;
using OutcomeHub.Domain.Entities.Portfolio;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence;

public sealed class OutcomeHubDbContext(DbContextOptions<OutcomeHubDbContext> options)
    : DbContext(options)
{
    // Academic DbSets
    public DbSet<OrgUnit> OrgUnits => Set<OrgUnit>();
    public DbSet<Program> Programs => Set<Program>();
    public DbSet<ProgramVersion> ProgramVersions => Set<ProgramVersion>();
    public DbSet<ProgramPlo> ProgramPlos => Set<ProgramPlo>();
    public DbSet<ProgramPi> ProgramPis => Set<ProgramPi>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseVersion> CourseVersions => Set<CourseVersion>();
    public DbSet<ProgramCourse> ProgramCourses => Set<ProgramCourse>();
    public DbSet<Cohort> Cohorts => Set<Cohort>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentPath> StudentPaths => Set<StudentPath>();
    public DbSet<CurriculumPath> CurriculumPaths => Set<CurriculumPath>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();
    public DbSet<CourseOfferingInstructor> CourseOfferingInstructors => Set<CourseOfferingInstructor>();
    public DbSet<DirectMeasurementPlan> DirectMeasurementPlans => Set<DirectMeasurementPlan>();
    public DbSet<DirectMeasurementSource> DirectMeasurementSources => Set<DirectMeasurementSource>();

    // Portfolio DbSets
    public DbSet<Syllabus> Syllabuses => Set<Syllabus>();
    public DbSet<SyllabusVersion> SyllabusVersions => Set<SyllabusVersion>();
    public DbSet<AssessmentItem> AssessmentItems => Set<AssessmentItem>();
    public DbSet<Rubric> Rubrics => Set<Rubric>();
    public DbSet<RubricCriterion> RubricCriteria => Set<RubricCriterion>();
    public DbSet<AssessmentQuestion> AssessmentQuestions => Set<AssessmentQuestion>();
    public DbSet<TeachingSession> TeachingSessions => Set<TeachingSession>();
    public DbSet<Clo> Clos => Set<Clo>();

    // Measurement DbSets
    public DbSet<MeasurementPeriod> MeasurementPeriods => Set<MeasurementPeriod>();
    public DbSet<MeasurementPeriodCohort> MeasurementPeriodCohorts => Set<MeasurementPeriodCohort>();
    public DbSet<MeasurementPeriodOffering> MeasurementPeriodOfferings => Set<MeasurementPeriodOffering>();
    public DbSet<MeasurementPeriodTarget> MeasurementPeriodTargets => Set<MeasurementPeriodTarget>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ScoreRecord> ScoreRecords => Set<ScoreRecord>();
    public DbSet<InputSnapshot> InputSnapshots => Set<InputSnapshot>();

    // Result DbSets
    public DbSet<ResultBatch> ResultBatches => Set<ResultBatch>();
    public DbSet<StudentCloResult> StudentCloResults => Set<StudentCloResult>();
    public DbSet<StudentPiResult> StudentPiResults => Set<StudentPiResult>();
    public DbSet<StudentPloResult> StudentPloResults => Set<StudentPloResult>();
    public DbSet<CohortOutcomeResult> CohortOutcomeResults => Set<CohortOutcomeResult>();

    // IAM DbSets
    public DbSet<Principal> Principals => Set<Principal>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleVersion> RoleVersions => Set<RoleVersion>();
    public DbSet<RoleVersionPermission> RoleVersionPermissions => Set<RoleVersionPermission>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<AccessScope> AccessScopes => Set<AccessScope>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<SodPolicyVersion> SodPolicyVersions => Set<SodPolicyVersion>();
    public DbSet<SodRule> SodRules => Set<SodRule>();
    public DbSet<SodException> SodExceptions => Set<SodException>();
    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();
    public DbSet<IdentityProvider> IdentityProviders => Set<IdentityProvider>();
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();

    // Audit DbSets
    public DbSet<OutcomeHub.Domain.Entities.Audit.AuditEvent> AuditEvents => Set<OutcomeHub.Domain.Entities.Audit.AuditEvent>();

    // Governance DbSets
    public DbSet<OutcomeHub.Domain.Entities.Governance.LegalHold> LegalHolds => Set<OutcomeHub.Domain.Entities.Governance.LegalHold>();
    public DbSet<OutcomeHub.Domain.Entities.Governance.LegalHoldItem> LegalHoldItems => Set<OutcomeHub.Domain.Entities.Governance.LegalHoldItem>();

    // Quality DbSets
    public DbSet<OutcomeHub.Domain.Entities.Quality.ImprovementPlan> ImprovementPlans => Set<OutcomeHub.Domain.Entities.Quality.ImprovementPlan>();
    public DbSet<OutcomeHub.Domain.Entities.Quality.ImprovementAction> ImprovementActions => Set<OutcomeHub.Domain.Entities.Quality.ImprovementAction>();
    public DbSet<OutcomeHub.Domain.Entities.Quality.ImprovementFinding> ImprovementFindings => Set<OutcomeHub.Domain.Entities.Quality.ImprovementFinding>();
    public DbSet<OutcomeHub.Domain.Entities.Quality.ImprovementEvidence> ImprovementEvidences => Set<OutcomeHub.Domain.Entities.Quality.ImprovementEvidence>();
    public DbSet<OutcomeHub.Domain.Entities.Quality.RemeasurementEvaluation> RemeasurementEvaluations => Set<OutcomeHub.Domain.Entities.Quality.RemeasurementEvaluation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutcomeHubDbContext).Assembly);
    }
}
