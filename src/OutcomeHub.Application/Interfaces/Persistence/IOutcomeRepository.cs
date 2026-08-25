using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IOutcomeRepository
{
    Task<ProgramOutcomeTreeDto?> GetOutcomeTreeAsync(
        Guid programVersionId,
        CancellationToken cancellationToken = default);

    Task<ProgramPloDto?> GetPloByIdAsync(
        Guid ploId,
        CancellationToken cancellationToken = default);

    Task<ProgramPloDto> CreatePloAsync(
        ProgramPlo plo,
        CancellationToken cancellationToken = default);

    Task<ProgramPiDto> CreatePiAsync(
        ProgramPi pi,
        CancellationToken cancellationToken = default);
}
