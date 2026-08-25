namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CurriculumPlan
{
    private CurriculumPlan() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal DeclaredTotalCredits { get; private set; }
    public string Status { get; private set; } = null!;
    public string Checksum { get; private set; } = null!;

    public ProgramVersion ProgramVersion { get; private set; } = null!;
}
