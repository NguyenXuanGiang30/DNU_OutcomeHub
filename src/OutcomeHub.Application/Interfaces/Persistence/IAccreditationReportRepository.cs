using OutcomeHub.Application.DTOs.Analytics;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IAccreditationReportRepository
{
    Task<MoetAccreditationReportDto?> GenerateMoetReportAsync(
        Guid programVersionId,
        Guid? measurementPeriodId,
        CancellationToken cancellationToken);

    Task<AunQaAccreditationReportDto?> GenerateAunQaReportAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<AbetAccreditationReportDto?> GenerateAbetReportAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<AccreditationDossierDto?> GenerateAccreditationDossierAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<StudentObeTranscriptDto?> GenerateStudentTranscriptAsync(
        Guid studentId,
        CancellationToken cancellationToken);
}
