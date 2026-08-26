using OutcomeHub.Application.DTOs.Analytics;

namespace OutcomeHub.Application.Interfaces.Services;

public interface IAccreditationReportService
{
    Task<MoetAccreditationReportDto> GetMoetReportAsync(
        Guid programVersionId,
        Guid? measurementPeriodId,
        CancellationToken cancellationToken);

    Task<AunQaAccreditationReportDto> GetAunQaReportAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<AbetAccreditationReportDto> GetAbetReportAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<AccreditationDossierDto> GetAccreditationDossierAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<StudentObeTranscriptDto> GetStudentObeTranscriptAsync(
        Guid studentId,
        CancellationToken cancellationToken);
}
