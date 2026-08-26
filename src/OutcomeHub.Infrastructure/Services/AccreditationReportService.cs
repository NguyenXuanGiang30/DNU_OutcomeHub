using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Analytics;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Infrastructure.Services;

public sealed class AccreditationReportService : IAccreditationReportService
{
    private readonly IAccreditationReportRepository _repository;

    public AccreditationReportService(IAccreditationReportRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<MoetAccreditationReportDto> GetMoetReportAsync(
        Guid programVersionId,
        Guid? measurementPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GenerateMoetReportAsync(programVersionId, measurementPeriodId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<AunQaAccreditationReportDto> GetAunQaReportAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GenerateAunQaReportAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<AbetAccreditationReportDto> GetAbetReportAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GenerateAbetReportAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<AccreditationDossierDto> GetAccreditationDossierAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GenerateAccreditationDossierAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<StudentObeTranscriptDto> GetStudentObeTranscriptAsync(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GenerateStudentTranscriptAsync(studentId, cancellationToken);
        return result ?? throw new NotFoundException("Student", studentId);
    }
}
