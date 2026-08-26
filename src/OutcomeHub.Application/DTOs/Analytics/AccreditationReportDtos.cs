namespace OutcomeHub.Application.DTOs.Analytics;

public sealed record MoetAccreditationReportDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    string FacultyName,
    string DegreeLevel,
    int TotalCredits,
    string StandardFramework,
    IReadOnlyList<MoetPloAssessmentRowDto> PloMatrixAssessments,
    IReadOnlyList<MoetCqiSummaryDto> CqiImprovements,
    string Conclusion,
    DateTimeOffset ExportedAt);

public sealed record MoetPloAssessmentRowDto(
    string PloCode,
    string PloDescription,
    string VqfDomain,
    decimal ExpectedBenchmark,
    decimal ActualAverageAttainment,
    int TotalEvaluatedStudents,
    int MetStandardStudentsCount,
    decimal AttainmentPercentage,
    string AssessmentResult);

public sealed record MoetCqiSummaryDto(
    string PlanCode,
    string ProblemStatement,
    string RootCause,
    string TargetImprovement,
    string ExecutionStatus,
    decimal? BeforeMetric,
    decimal? AfterMetric);

public sealed record AunQaAccreditationReportDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    string FacultyName,
    string CriterionName,
    IReadOnlyList<AunQaExpectedLearningOutcomeDto> ExpectedLearningOutcomes,
    IReadOnlyList<AunQaTeachingAssessmentAlignmentDto> AlignmentMatrix,
    IReadOnlyList<AunQaContinuousImprovementCycleDto> QualityCycles,
    DateTimeOffset ExportedAt);

public sealed record AunQaExpectedLearningOutcomeDto(
    string PloCode,
    string Description,
    string AlignmentToMission,
    decimal AttainmentRate,
    bool MeetsCriterion);

public sealed record AunQaTeachingAssessmentAlignmentDto(
    string CourseCode,
    string CourseName,
    string ClosSummary,
    string TeachingMethods,
    string AssessmentMethods,
    string PlosAddressed);

public sealed record AunQaContinuousImprovementCycleDto(
    string PeriodName,
    string FindingDescription,
    string ActionImplemented,
    string MeasuredResult);

public sealed record AbetAccreditationReportDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    string Commission,
    IReadOnlyList<AbetStudentOutcomeAssessmentDto> StudentOutcomes,
    IReadOnlyList<AbetContinuousImprovementEvidenceDto> ContinuousImprovementEvidences,
    DateTimeOffset ExportedAt);

public sealed record AbetStudentOutcomeAssessmentDto(
    string OutcomeIndex,
    string Description,
    decimal AttainmentBenchmark,
    decimal AttainmentResult,
    string Status,
    IReadOnlyList<string> ContributingCourses);

public sealed record AbetContinuousImprovementEvidenceDto(
    string ActionTitle,
    string OutcomeAddressed,
    string ModificationMade,
    string RemeasurementImpact);

public sealed record AccreditationDossierDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    int VersionNo,
    string OrgUnitName,
    DateTimeOffset BaselineDate,
    IReadOnlyList<PloDetailAttainmentDto> ProgramLearningOutcomes,
    IReadOnlyList<DossierCourseSummaryDto> CurriculumCourses,
    IReadOnlyList<MoetCqiSummaryDto> ContinuousImprovements,
    string DossierIntegrityChecksum,
    DateTimeOffset GeneratedAt);

public sealed record DossierCourseSummaryDto(
    Guid CourseId,
    string CourseCode,
    string CourseName,
    int Credits,
    int ClosCount,
    decimal AverageCloAttainmentRate);

public sealed record StudentObeTranscriptDto(
    Guid StudentId,
    string StudentCode,
    string FullName,
    string DegreeProgram,
    string Cohort,
    decimal CumulativeGpa,
    int TotalCreditsAccumulated,
    IReadOnlyList<StudentPloCompetencyTranscriptDto> PloCompetencies,
    IReadOnlyList<StudentCourseOutcomeRecordDto> CourseAttainments,
    string TranscriptVerificationCode,
    DateTimeOffset IssuedAt);

public sealed record StudentPloCompetencyTranscriptDto(
    string PloCode,
    string PloTitle,
    string VqfDomain,
    decimal RequiredProficiency,
    decimal AchievedProficiency,
    string MasteryLevel,
    bool IsCertified);

public sealed record StudentCourseOutcomeRecordDto(
    string CourseCode,
    string CourseName,
    int Credits,
    decimal Grade,
    string LetterGrade,
    decimal CloAverageAttainmentRate);
