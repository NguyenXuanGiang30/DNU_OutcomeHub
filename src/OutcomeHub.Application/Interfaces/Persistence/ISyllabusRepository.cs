using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface ISyllabusRepository
{
    Task<PagedResult<SyllabusDto>> GetPagedSyllabusesAsync(
        PagedRequest request,
        Guid? programCourseId,
        Guid? ownerOrgUnitId,
        CancellationToken cancellationToken);

    Task<Syllabus?> GetSyllabusByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Syllabus> CreateSyllabusAsync(Syllabus syllabus, CancellationToken cancellationToken);

    Task<IReadOnlyList<SyllabusVersionDto>> GetSyllabusVersionsAsync(
        Guid syllabusId,
        CancellationToken cancellationToken);

    Task<SyllabusVersion?> GetSyllabusVersionByIdAsync(
        Guid versionId,
        CancellationToken cancellationToken);

    Task<SyllabusVersion> CreateSyllabusVersionAsync(
        SyllabusVersion syllabusVersion,
        CancellationToken cancellationToken);
}
