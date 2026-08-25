namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class CalculationPolicyVersion
{
    private CalculationPolicyVersion()
    {
    }

    public Guid Id { get; private set; }

    public Guid PolicyId { get; private set; }

    public int VersionNo { get; private set; }

    public DateOnly EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }

    public string Status { get; private set; } = null!;

    public string FormulaFamily { get; private set; } = null!;

    public string EngineContractVersion { get; private set; } = null!;

    public int DirectSourceMin { get; private set; }

    public int DirectSourceMax { get; private set; }

    public string MissingDataRule { get; private set; } = null!;

    public string RepeatAttemptRule { get; private set; } = null!;

    public string WithdrawalRule { get; private set; } = null!;

    public string RecognitionRule { get; private set; } = null!;

    public string DirectIndirectMode { get; private set; } = null!;

    public decimal? Alpha { get; private set; }

    public string CoreGateMode { get; private set; } = null!;

    public int DefaultMinSampleSize { get; private set; }

    public string Definition { get; private set; } = null!;

    public string SchemaVersion { get; private set; } = null!;

    public Guid WorkflowInstanceId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public Guid? SupersedesId { get; private set; }

    public CalculationPolicy Policy { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public CalculationPolicyVersion? Supersedes { get; private set; }
    public ICollection<CalculationPolicyVersion> Successors { get; private set; } = new List<CalculationPolicyVersion>();
}
