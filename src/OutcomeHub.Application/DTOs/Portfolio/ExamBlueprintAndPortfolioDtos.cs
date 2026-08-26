namespace OutcomeHub.Application.DTOs.Portfolio;

// ── Exam Blueprint & Assessment Matrix (FR-PRT-03, FR-PRT-05, FR-PRT-06) ──

public sealed record ExamQuestionDto(
    Guid Id,
    int QuestionNo,
    string Title,
    string Content,
    decimal MaxScore,
    string BloomLevel,
    Guid ProgramCloId,
    string CloCode,
    Guid? ProgramPiId,
    string? PiCode,
    decimal DirectPiWeightPercentage);

public sealed record ExamSectionDto(
    Guid Id,
    string SectionCode,
    string SectionName,
    decimal SectionWeightRatio,
    IReadOnlyList<ExamQuestionDto> Questions);

public sealed record ExamBlueprintDto(
    Guid Id,
    Guid SyllabusVersionId,
    Guid AssessmentItemId,
    string AssessmentCode,
    string AssessmentName,
    string AssessmentType,
    int TotalDurationMinutes,
    decimal TotalMaxScore,
    string Status,
    IReadOnlyList<ExamSectionDto> Sections,
    string IntegrityChecksum,
    DateTimeOffset CreatedAt);

public sealed record CreateExamQuestionRequest(
    int QuestionNo,
    string Title,
    string Content,
    decimal MaxScore,
    string BloomLevel,
    Guid ProgramCloId,
    Guid? ProgramPiId,
    decimal DirectPiWeightPercentage);

public sealed record CreateExamSectionRequest(
    string SectionCode,
    string SectionName,
    decimal SectionWeightRatio,
    IReadOnlyList<CreateExamQuestionRequest> Questions);

public sealed record CreateExamBlueprintRequest(
    Guid SyllabusVersionId,
    Guid AssessmentItemId,
    int TotalDurationMinutes,
    decimal TotalMaxScore,
    IReadOnlyList<CreateExamSectionRequest> Sections);

// ── Traceability Matrix Table 8.3.1 (FR-PRT-17) ──

public sealed record TraceabilityMatrixRowDto(
    Guid ProgramCloId,
    string CloCode,
    string CloDescription,
    string BloomLevel,
    Guid? ProgramPiId,
    string? PiCode,
    Guid AssessmentItemId,
    string AssessmentCode,
    string AssessmentName,
    string ContributionLevel, // I, R, M
    bool IsDirectAssessment,  // A flag
    string AssessmentRole,    // DIRECT, SUPPORT, CLO_ONLY
    string EvidenceArtifactName);

public sealed record SyllabusTraceabilityMatrix831Dto(
    Guid SyllabusVersionId,
    string CourseCode,
    string CourseName,
    int VersionNo,
    IReadOnlyList<TraceabilityMatrixRowDto> Rows,
    DateTimeOffset GeneratedAt);

// ── Direct Assessment Matrix Table 8.3.2 (FR-PRT-05, FR-PRT-18, FR-PRT-19) ──

public sealed record DirectAssessmentRowDto(
    Guid ProgramPiId,
    string PiCode,
    string PiDescription,
    Guid AssessmentItemId,
    string AssessmentCode,
    string AssessmentName,
    Guid RubricCriterionId,
    string CriterionCode,
    string CriterionDescription,
    decimal MaxScore,
    decimal DirectCriterionWeightPercentage, // Must sum to 100% per PI
    bool IsMultiPiSplitting);

public sealed record DirectAssessmentMatrix832Dto(
    Guid SyllabusVersionId,
    string CourseCode,
    string CourseName,
    int VersionNo,
    IReadOnlyList<DirectAssessmentRowDto> Rows,
    bool IsWeightSumValid,
    DateTimeOffset GeneratedAt);

// ── Teaching Schedule & Weekly Lesson Plan (FR-PRT-20) ──

public sealed record TeachingSessionPlanDto(
    Guid Id,
    int SessionNo,
    int WeekNo,
    string Topic,
    IReadOnlyList<string> LinkedLlos,
    IReadOnlyList<string> LinkedClos,
    int TeachingHours,
    int SelfStudyHours,
    string PedagogicalMethod,
    string TeachingMaterials,
    string AssessmentAndEvidenceTask,
    string SelfStudyAssignment);

public sealed record WeeklyScheduleDto(
    Guid SyllabusVersionId,
    int TotalWeeks,
    int TotalTeachingHours,
    int TotalSelfStudyHours,
    IReadOnlyList<TeachingSessionPlanDto> Sessions,
    DateTimeOffset UpdatedAt);

public sealed record SaveTeachingSessionRequest(
    int SessionNo,
    int WeekNo,
    string Topic,
    IReadOnlyList<string> LinkedLlos,
    IReadOnlyList<string> LinkedClos,
    int TeachingHours,
    int SelfStudyHours,
    string PedagogicalMethod,
    string TeachingMaterials,
    string AssessmentAndEvidenceTask,
    string SelfStudyAssignment);

public sealed record SaveWeeklyScheduleRequest(
    Guid SyllabusVersionId,
    IReadOnlyList<SaveTeachingSessionRequest> Sessions);

// ── Academic Document Vault & Evidence Artifacts (FR-PRT-07, FR-PRT-08, FR-PRT-10, FR-PRT-11) ──

public sealed record DocumentVaultItemDto(
    Guid Id,
    Guid SyllabusVersionId,
    string DocumentType, // SYLLABUS, EXAM_PAPER, SCORING_GUIDE, RUBRIC_SAMPLE, STUDENT_EVIDENCE
    string Title,
    string FileName,
    string MimeType,
    long FileSizeBytes,
    string Sha256Checksum,
    string VirusScanStatus, // CLEAN, PENDING, INFECTED
    int VersionNo,
    string Status, // DRAFT, IN_REVIEW, APPROVED, LOCKED_FOR_MEASUREMENT
    Guid UploadedByStaffId,
    string UploaderName,
    DateTimeOffset UploadedAt);

public sealed record UploadDocumentRequest(
    Guid SyllabusVersionId,
    string DocumentType,
    string Title,
    string FileName,
    string MimeType,
    long FileSizeBytes,
    string Base64Content,
    Guid UploadedByStaffId);

// ── Portfolio Package Engine (FR-PRT-12) ──

public sealed record PortfolioPackageItemDto(
    Guid DocumentId,
    string SectionCategory,
    string Title,
    string FileName,
    string Sha256Checksum,
    int VersionNo);

public sealed record PortfolioPackageDto(
    Guid PackageId,
    Guid SyllabusVersionId,
    string CourseCode,
    string CourseName,
    string AcademicYear,
    string Semester,
    string WatermarkText,
    string ManifestChecksum,
    IReadOnlyList<PortfolioPackageItemDto> TableOfContents,
    DateTimeOffset ExportedAt);

public sealed record ExportPortfolioPackageRequest(
    Guid SyllabusVersionId,
    string AcademicYear,
    string Semester,
    string? CustomWatermark = null);

// ── AI Syllabus Draft Assistant (FR-PRT-09) ──

public sealed record AiSyllabusDraftRequest(
    Guid ProgramVersionId,
    Guid CourseId,
    string CourseName,
    int Credits,
    IReadOnlyList<string> TargetPlos,
    string SpecializationArea,
    string ModelPreference = "gemini-1.5-pro");

public sealed record AiSyllabusDraftResultDto(
    Guid DraftSessionId,
    string CourseOverview,
    IReadOnlyList<string> GeneratedClos,
    IReadOnlyList<SaveTeachingSessionRequest> GeneratedTeachingSchedule,
    IReadOnlyList<CreateExamSectionRequest> GeneratedAssessmentBlueprint,
    string ModelUsed,
    string PromptVersion,
    DateTimeOffset GeneratedAt);

// ── Syllabus Publishing Readiness Checklist (FR-PRT-21) ──

public sealed record SyllabusPublishingGateItemDto(
    string GateCode,
    string Description,
    bool IsPassed,
    string Details,
    string? ActionRequired);

public sealed record SyllabusPublishingChecklistDto(
    Guid SyllabusVersionId,
    string CourseCode,
    int VersionNo,
    bool IsReadyForPublishing,
    IReadOnlyList<SyllabusPublishingGateItemDto> Gates,
    DateTimeOffset CheckedAt);
