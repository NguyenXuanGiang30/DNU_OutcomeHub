namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class AssessmentItem
{
    private AssessmentItem() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string AssessmentCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string AssessmentType { get; private set; } = null!;
    public decimal CourseWeightRatio { get; private set; }
    public decimal? IndividualComponentRatio { get; private set; }
    public bool IsGroupAssessment { get; private set; }
    public bool CountsTowardCourseGrade { get; private set; }
    public decimal MaxScore { get; private set; }
    public int SortOrder { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public AssessmentItem? Parent { get; private set; }
    public ICollection<AssessmentItem> Children { get; private set; } = new List<AssessmentItem>();
    public Rubric? Rubric { get; private set; }

    public static AssessmentItem Create(
        Guid id,
        Guid syllabusVersionId,
        Guid? parentId,
        string assessmentCode,
        string name,
        string assessmentType,
        decimal courseWeightRatio,
        decimal? individualComponentRatio,
        bool isGroupAssessment,
        bool countsTowardCourseGrade,
        decimal maxScore,
        int sortOrder)
    {
        return new AssessmentItem
        {
            Id = id,
            SyllabusVersionId = syllabusVersionId,
            ParentId = parentId,
            AssessmentCode = assessmentCode.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            AssessmentType = assessmentType.Trim().ToUpperInvariant(),
            CourseWeightRatio = courseWeightRatio,
            IndividualComponentRatio = individualComponentRatio,
            IsGroupAssessment = isGroupAssessment,
            CountsTowardCourseGrade = countsTowardCourseGrade,
            MaxScore = maxScore,
            SortOrder = sortOrder,
        };
    }
}
