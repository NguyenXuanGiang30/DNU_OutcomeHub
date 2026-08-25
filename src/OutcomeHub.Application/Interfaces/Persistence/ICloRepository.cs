using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface ICloRepository
{
    Task<IReadOnlyList<CloDto>> GetClosBySyllabusVersionIdAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken);

    Task<Clo> CreateCloAsync(Clo clo, CancellationToken cancellationToken);

    Task<Clo> UpdateCloAsync(
        Guid id,
        string description,
        string domain,
        string bloomLevel,
        bool isCore,
        int sortOrder,
        CancellationToken cancellationToken);

    Task<bool> DeleteCloAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CoursePiMappingDto>> GetCoursePiMappingsAsync(
        Guid programVersionId,
        Guid? programCourseId,
        CancellationToken cancellationToken);

    Task<CoursePiMapping> SetCoursePiMappingAsync(
        CoursePiMapping mapping,
        CancellationToken cancellationToken);
}
