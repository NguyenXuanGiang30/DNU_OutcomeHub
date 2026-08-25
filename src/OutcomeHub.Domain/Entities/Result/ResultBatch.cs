namespace OutcomeHub.Domain.Entities.Result;

public sealed class ResultBatch
{
    private ResultBatch()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid InputSnapshotId { get; private set; }

    public Guid PolicyVersionId { get; private set; }

    public Guid ProgramPolicyBindingId { get; private set; }

    public Guid OrgUnitId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public short AcademicYearStart { get; private set; }

    public int BatchNo { get; private set; }

    public string EngineVersion { get; private set; } = null!;

    public string SourceCommit { get; private set; } = null!;

    public string? ContainerDigest { get; private set; }

    public string Status { get; private set; } = null!;

    public string IdempotencyKey { get; private set; } = null!;

    public string RequestChecksum { get; private set; } = null!;

    public Guid? RecalculatesBatchId { get; private set; }

    public string? RecalculationReason { get; private set; }

    public Guid WorkflowInstanceId { get; private set; }

    public Guid SodPolicyVersionId { get; private set; }

    public string? ResultChecksum { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? PublishedAt { get; private set; }

    public OutcomeHub.Domain.Entities.Governance.GovernedResource GovernedResource { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.ProgramPolicyBinding ProgramPolicyBinding { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public ResultBatch? RecalculatesBatch { get; private set; }
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.SodPolicyVersion SodPolicyVersion { get; private set; } = null!;

    public static ResultBatch CreateRunning(
        Guid id,
        Guid governedResourceId,
        Guid measurementPeriodId,
        Guid inputSnapshotId,
        Guid policyVersionId,
        Guid programPolicyBindingId,
        Guid orgUnitId,
        Guid programVersionId,
        short academicYearStart,
        int batchNo,
        string engineVersion,
        string sourceCommit,
        string idempotencyKey,
        string requestChecksum,
        Guid workflowInstanceId,
        Guid sodPolicyVersionId,
        DateTimeOffset startedAt)
    {
        return new ResultBatch
        {
            Id = id,
            GovernedResourceId = governedResourceId,
            MeasurementPeriodId = measurementPeriodId,
            InputSnapshotId = inputSnapshotId,
            PolicyVersionId = policyVersionId,
            ProgramPolicyBindingId = programPolicyBindingId,
            OrgUnitId = orgUnitId,
            ProgramVersionId = programVersionId,
            AcademicYearStart = academicYearStart,
            BatchNo = batchNo,
            EngineVersion = engineVersion,
            SourceCommit = sourceCommit,
            Status = "RUNNING",
            IdempotencyKey = idempotencyKey,
            RequestChecksum = requestChecksum,
            WorkflowInstanceId = workflowInstanceId,
            SodPolicyVersionId = sodPolicyVersionId,
            ResultChecksum = null,
            StartedAt = startedAt,
            CompletedAt = null,
            PublishedAt = null
        };
    }

    public void Complete(string resultChecksum, DateTimeOffset completedAt)
    {
        ResultChecksum = resultChecksum;
        CompletedAt = completedAt;
        Status = "CALCULATED";
    }
}
