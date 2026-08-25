using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IProgramRepository
{
    Task<PagedResult<ProgramDto>> GetPagedAsync(
        PagedRequest request,
        Guid? ownerOrgUnitId = null,
        CancellationToken cancellationToken = default);

    Task<ProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProgramDto> CreateAsync(Program program, CancellationToken cancellationToken = default);

    Task<ProgramDto> UpdateAsync(Program program, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProgramVersionDto>> GetVersionsByProgramIdAsync(
        Guid programId,
        CancellationToken cancellationToken = default);

    Task<ProgramVersionDto?> GetVersionByIdAsync(
        Guid versionId,
        CancellationToken cancellationToken = default);

    Task<ProgramVersionDto> CreateVersionAsync(
        ProgramVersion version,
        CancellationToken cancellationToken = default);

    Task<ProgramVersionDto> PublishVersionAsync(
        Guid versionId,
        CancellationToken cancellationToken = default);
}
