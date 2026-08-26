namespace OutcomeHub.Application.DTOs.Academic;

// ── FR-CTD-10: StudentPath Coverage Analysis ──
public sealed record StudentPathCoverageAnalysisDto(
    Guid ProgramVersionId,
    Guid CurriculumPathId,
    string PathName,
    int TotalPlos,
    int CoveredPlos,
    int TotalPis,
    int CoveredPis,
    int PisWithLevelA,
    decimal CoveragePercentage,
    IReadOnlyList<CoverageIssueDto> Issues,
    DateTimeOffset AnalyzedAt);

public sealed record CoverageIssueDto(
    string IssueType, // "MISSING_LEVEL_A", "MISSING_LEVEL_M", "UNCOVERED_PI", "ORPHANED_CLO", "PATH_OVERLAP"
    string Severity,  // "ERROR", "WARNING", "INFO"
    string TargetCode,
    string Description,
    string RemediationSuggestion);

// ── FR-CTD-11: Competency Development Roadmap ──
public sealed record CompetencyRoadmapDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    IReadOnlyList<TermProgressionDto> Terms,
    IReadOnlyList<PloBloomEvolutionDto> PloBloomEvolutions,
    DateTimeOffset GeneratedAt);

public sealed record TermProgressionDto(
    int TermNo,
    string TermName,
    int TotalCredits,
    IReadOnlyList<CourseTermSummaryDto> Courses);

public sealed record CourseTermSummaryDto(
    Guid CourseId,
    string CourseCode,
    string CourseName,
    decimal Credits,
    IReadOnlyList<string> ContributedPlos,
    IReadOnlyList<string> MatrixLevels); // e.g. ["I", "R", "A"]

public sealed record PloBloomEvolutionDto(
    string PloCode,
    string Description,
    string InitialBloomLevel,
    string TargetBloomLevel,
    IReadOnlyList<TermBloomProgressDto> ProgressMilestones);

public sealed record TermBloomProgressDto(
    int TermNo,
    string MaxBloomLevelReached,
    string ReachedViaCourseCode);

// ── FR-CTD-14 & FR-CTD-23: ProgramVersion Diff & Crosswalk ──
public sealed record ProgramVersionDiffDto(
    Guid SourceVersionId,
    int SourceVersionNo,
    Guid TargetVersionId,
    int TargetVersionNo,
    IReadOnlyList<PloDiffItemDto> PloDiffs,
    IReadOnlyList<CourseDiffItemDto> CourseDiffs,
    IReadOnlyList<MatrixMappingDiffDto> MappingDiffs,
    DateTimeOffset ComparedAt);

public sealed record PloDiffItemDto(
    string ChangeType, // "ADDED", "REMOVED", "MODIFIED", "UNCHANGED"
    string PloCode,
    string? OldDescription,
    string? NewDescription,
    string? OldBloomLevel,
    string? NewBloomLevel);

public sealed record CourseDiffItemDto(
    string ChangeType,
    string CourseCode,
    string CourseName,
    decimal? OldCredits,
    decimal? NewCredits);

public sealed record MatrixMappingDiffDto(
    string CourseCode,
    string PloCode,
    string? OldLevel,
    string? NewLevel,
    bool? OldIsAssessmentSource,
    bool? NewIsAssessmentSource);

public sealed record PloCrosswalkDto(
    Guid SourceVersionId,
    Guid TargetVersionId,
    IReadOnlyList<PloMappingCrosswalkRowDto> Rows,
    DateTimeOffset GeneratedAt);

public sealed record PloMappingCrosswalkRowDto(
    string SourcePloCode,
    string TargetPloCode,
    string MappingRelationship, // "DIRECT_EQUIVALENT", "SUBSET", "SUPERSET", "SPLIT", "MERGED"
    decimal ConfidenceWeight);

// ── FR-CTD-15 & FR-CTD-16: Direct Measurement Plan (DMP) ──
public sealed record DirectMeasurementPlanDetailsDto(
    Guid PlanId,
    Guid ProgramVersionId,
    Guid CurriculumPathId,
    Guid ProgramPiId,
    string PiCode,
    string PiDescription,
    string Status,
    IReadOnlyList<MeasurementSourceDetailsDto> Sources,
    bool MeetsLevelAPolicy,
    DateTimeOffset UpdatedAt);

public sealed record MeasurementSourceDetailsDto(
    Guid SourceId,
    Guid CourseOfferingId,
    string CourseCode,
    string CourseName,
    string TermCode,
    Guid AssessmentItemId,
    string AssessmentName,
    decimal WeightPercentage, // Must sum to 100%
    bool IsPrimary,
    bool IsBenchmark);

public sealed record CreateDirectMeasurementPlanRequest(
    Guid ProgramVersionId,
    Guid CurriculumPathId,
    Guid ProgramPiId,
    IReadOnlyList<CreateMeasurementSourceItemRequest> Sources);

public sealed record CreateMeasurementSourceItemRequest(
    Guid CourseOfferingId,
    Guid AssessmentItemId,
    decimal WeightPercentage,
    bool IsPrimary,
    bool IsBenchmark);

// ── FR-CTD-20: PO - PLO - Competency Tier 1-3 Matrix ──
public sealed record ProgramObjectiveMatrixDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    IReadOnlyList<ProgramObjectiveItemDto> ProgramObjectives,
    IReadOnlyList<PoPloMappingCellDto> PoPloMatrix,
    IReadOnlyList<CompetencyTierItemDto> CompetencyTiers,
    DateTimeOffset GeneratedAt);

public sealed record ProgramObjectiveItemDto(
    Guid PoId,
    string Code,
    string Title,
    string Description,
    int SortOrder);

public sealed record PoPloMappingCellDto(
    string PoCode,
    string PloCode,
    string ContributionLevel); // "H" (High), "M" (Medium), "L" (Low), "NONE"

public sealed record CompetencyTierItemDto(
    int TierLevel, // 1, 2, 3
    string Domain, // KNOWLEDGE, SKILL, ATTITUDE
    string Name,
    string Description,
    IReadOnlyList<string> AlignedPlos);

// ── FR-CTD-21 & FR-CTD-22: Prerequisite Graph & Knowledge Blocks ──
public sealed record PrerequisiteGraphDto(
    Guid ProgramVersionId,
    string ProgramCode,
    IReadOnlyList<PrerequisiteGraphNodeDto> Nodes,
    IReadOnlyList<PrerequisiteGraphEdgeDto> Edges,
    DateTimeOffset GeneratedAt);

public sealed record PrerequisiteGraphNodeDto(
    Guid CourseId,
    string CourseCode,
    string CourseName,
    decimal Credits,
    int RecommendedTerm,
    string KnowledgeBlock);

public sealed record PrerequisiteGraphEdgeDto(
    string SourceCourseCode,
    string TargetCourseCode,
    string DependencyType); // "PREREQUISITE", "PREVIOUS", "COREQUISITE"

public sealed record KnowledgeBlockStructureDto(
    Guid ProgramVersionId,
    decimal TotalCredits,
    IReadOnlyList<KnowledgeBlockSummaryDto> KnowledgeBlocks);

public sealed record KnowledgeBlockSummaryDto(
    string BlockCode,
    string BlockName,
    decimal RequiredCredits,
    decimal ElectiveCredits,
    decimal TotalCredits,
    IReadOnlyList<CourseTermSummaryDto> Courses);

// ── FR-CTD-24: Curriculum Specification Document (Bản mô tả CTĐT) ──
public sealed record CurriculumSpecificationDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    string DegreeLevel,
    string EducationMode,
    string OrgUnitName,
    int VersionNo,
    DateOnly EffectiveFrom,
    decimal TotalCredits,
    string DecisionNumber,
    IReadOnlyList<ProgramObjectiveItemDto> Objectives,
    IReadOnlyList<PloBloomEvolutionDto> Plos,
    KnowledgeBlockStructureDto KnowledgeStructure,
    PrerequisiteGraphDto PrerequisiteGraph,
    string IntegrityChecksum,
    DateTimeOffset GeneratedAt);

// ── FR-CTD-25: Program Version Publishing Checklist ──
public sealed record PublishingReadinessChecklistDto(
    Guid ProgramVersionId,
    string ProgramCode,
    int VersionNo,
    bool IsReadyForPublishing,
    IReadOnlyList<PublishingChecklistItemDto> ChecklistItems,
    DateTimeOffset CheckedAt);

public sealed record PublishingChecklistItemDto(
    string Category, // "PLO_STRUCTURE", "STUDENT_PATH_COVERAGE", "DMP_LEVEL_A", "PREREQUISITE_CYCLE", "MATRIX_COMPLETION"
    string ItemTitle,
    bool IsPassed,
    string Details,
    string? BlockingReason);
