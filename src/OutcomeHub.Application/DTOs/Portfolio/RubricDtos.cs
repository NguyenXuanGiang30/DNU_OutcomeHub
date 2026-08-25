namespace OutcomeHub.Application.DTOs.Portfolio;

public sealed record AssessmentItemDto(
    Guid Id,
    Guid SyllabusVersionId,
    Guid? ParentId,
    string AssessmentCode,
    string Name,
    string AssessmentType,
    decimal CourseWeightRatio,
    decimal? IndividualComponentRatio,
    bool IsGroupAssessment,
    bool CountsTowardCourseGrade,
    decimal MaxScore,
    int SortOrder,
    bool HasRubric);

public sealed record CreateAssessmentItemRequest(
    Guid? ParentId,
    string AssessmentCode,
    string Name,
    string AssessmentType,
    decimal CourseWeightRatio,
    decimal? IndividualComponentRatio,
    bool IsGroupAssessment,
    bool CountsTowardCourseGrade,
    decimal MaxScore,
    int SortOrder);

public sealed record RubricLevelDto(
    Guid Id,
    Guid RubricCriterionId,
    string LevelCode,
    int LevelOrder,
    string Label,
    string? Description,
    decimal ScoreFrom,
    decimal ScoreTo,
    decimal? NumericValue);

public sealed record CreateRubricLevelRequest(
    string LevelCode,
    int LevelOrder,
    string Label,
    string? Description,
    decimal ScoreFrom,
    decimal ScoreTo,
    decimal? NumericValue);

public sealed record RubricCriterionDto(
    Guid Id,
    Guid RubricId,
    Guid AssessmentItemId,
    string CriterionCode,
    string Description,
    decimal MaxScore,
    decimal RubricWeightRatio,
    string ScoreSourceMode,
    bool IsCore,
    bool IndividualEvidence,
    int SortOrder,
    IReadOnlyList<RubricLevelDto> Levels);

public sealed record CreateRubricCriterionRequest(
    string CriterionCode,
    string Description,
    decimal MaxScore,
    decimal RubricWeightRatio,
    string ScoreSourceMode,
    bool IsCore,
    bool IndividualEvidence,
    int SortOrder,
    IReadOnlyList<CreateRubricLevelRequest>? Levels = null);

public sealed record RubricDto(
    Guid Id,
    Guid SyllabusVersionId,
    Guid SyllabusTemplateVersionId,
    Guid AssessmentItemId,
    string Code,
    string Name,
    decimal MaxScore,
    Guid RubricScaleId,
    IReadOnlyList<RubricCriterionDto> Criteria);

public sealed record CreateRubricRequest(
    Guid AssessmentItemId,
    Guid SyllabusTemplateVersionId,
    string Code,
    string Name,
    decimal MaxScore,
    Guid RubricScaleId,
    IReadOnlyList<CreateRubricCriterionRequest>? Criteria = null);
