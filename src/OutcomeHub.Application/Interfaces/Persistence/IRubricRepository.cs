using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IRubricRepository
{
    Task<IReadOnlyList<AssessmentItemDto>> GetAssessmentItemsAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken);

    Task<AssessmentItem> CreateAssessmentItemAsync(
        AssessmentItem item,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<RubricDto>> GetRubricsBySyllabusVersionIdAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken);

    Task<RubricDto?> GetRubricByIdAsync(
        Guid rubricId,
        CancellationToken cancellationToken);

    Task<Rubric> CreateRubricAsync(
        Rubric rubric,
        IReadOnlyList<RubricCriterion> criteria,
        CancellationToken cancellationToken);
}
