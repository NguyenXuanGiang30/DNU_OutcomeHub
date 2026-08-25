using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IScoreRepository
{
    Task<PagedResult<ScoreRecordDto>> GetPagedScoresAsync(
        PagedRequest request,
        Guid? courseOfferingId,
        Guid? studentId,
        Guid? assessmentItemId,
        short? academicYearStart,
        CancellationToken cancellationToken = default);

    Task<ScoreRecord> SubmitScoreRecordAsync(
        ScoreIdentity identity,
        ScoreRecord record,
        CancellationToken cancellationToken = default);
}
