using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;

public sealed class RubricRepository : IRubricRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public RubricRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<AssessmentItemDto>> GetAssessmentItemsAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.AssessmentItems
            .AsNoTracking()
            .Where(a => a.SyllabusVersionId == syllabusVersionId)
            .OrderBy(a => a.SortOrder)
            .Select(a => new AssessmentItemDto(
                a.Id,
                a.SyllabusVersionId,
                a.ParentId,
                a.AssessmentCode,
                a.Name,
                a.AssessmentType,
                a.CourseWeightRatio,
                a.IndividualComponentRatio,
                a.IsGroupAssessment,
                a.CountsTowardCourseGrade,
                a.MaxScore,
                a.SortOrder,
                a.Rubric != null))
            .ToListAsync(cancellationToken);
    }

    public async Task<AssessmentItem> CreateAssessmentItemAsync(
        AssessmentItem item,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(item);

        await _dbContext.AssessmentItems.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<IReadOnlyList<RubricDto>> GetRubricsBySyllabusVersionIdAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var rubrics = await _dbContext.Set<Rubric>()
            .AsNoTracking()
            .Where(r => r.SyllabusVersionId == syllabusVersionId)
            .Include(r => r.Criteria)
                .ThenInclude(c => c.Levels)
            .OrderBy(r => r.Code)
            .ToListAsync(cancellationToken);

        return rubrics.Select(r => new RubricDto(
            r.Id,
            r.SyllabusVersionId,
            r.SyllabusTemplateVersionId,
            r.AssessmentItemId,
            r.Code,
            r.Name,
            r.MaxScore,
            r.RubricScaleId,
            r.Criteria
                .OrderBy(c => c.SortOrder)
                .Select(c => new RubricCriterionDto(
                    c.Id,
                    c.RubricId,
                    c.AssessmentItemId,
                    c.CriterionCode,
                    c.Description,
                    c.MaxScore,
                    c.RubricWeightRatio,
                    c.ScoreSourceMode,
                    c.IsCore,
                    c.IndividualEvidence,
                    c.SortOrder,
                    c.Levels
                        .OrderBy(l => l.LevelOrder)
                        .Select(l => new RubricLevelDto(
                            l.Id,
                            l.RubricCriterionId,
                            l.LevelCode,
                            l.LevelOrder,
                            l.Label,
                            l.Description,
                            l.ScoreFrom,
                            l.ScoreTo,
                            l.NumericValue))
                        .ToList()))
                .ToList()))
            .ToList();
    }

    public async Task<RubricDto?> GetRubricByIdAsync(
        Guid rubricId,
        CancellationToken cancellationToken)
    {
        var r = await _dbContext.Set<Rubric>()
            .AsNoTracking()
            .Include(rub => rub.Criteria)
                .ThenInclude(c => c.Levels)
            .FirstOrDefaultAsync(rub => rub.Id == rubricId, cancellationToken);

        if (r == null)
        {
            return null;
        }

        return new RubricDto(
            r.Id,
            r.SyllabusVersionId,
            r.SyllabusTemplateVersionId,
            r.AssessmentItemId,
            r.Code,
            r.Name,
            r.MaxScore,
            r.RubricScaleId,
            r.Criteria
                .OrderBy(c => c.SortOrder)
                .Select(c => new RubricCriterionDto(
                    c.Id,
                    c.RubricId,
                    c.AssessmentItemId,
                    c.CriterionCode,
                    c.Description,
                    c.MaxScore,
                    c.RubricWeightRatio,
                    c.ScoreSourceMode,
                    c.IsCore,
                    c.IndividualEvidence,
                    c.SortOrder,
                    c.Levels
                        .OrderBy(l => l.LevelOrder)
                        .Select(l => new RubricLevelDto(
                            l.Id,
                            l.RubricCriterionId,
                            l.LevelCode,
                            l.LevelOrder,
                            l.Label,
                            l.Description,
                            l.ScoreFrom,
                            l.ScoreTo,
                            l.NumericValue))
                        .ToList()))
                .ToList());
    }

    public async Task<Rubric> CreateRubricAsync(
        Rubric rubric,
        IReadOnlyList<RubricCriterion> criteria,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        await _dbContext.Set<Rubric>().AddAsync(rubric, cancellationToken);
        if (criteria != null && criteria.Count > 0)
        {
            await _dbContext.RubricCriteria.AddRangeAsync(criteria, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return rubric;
    }
}
