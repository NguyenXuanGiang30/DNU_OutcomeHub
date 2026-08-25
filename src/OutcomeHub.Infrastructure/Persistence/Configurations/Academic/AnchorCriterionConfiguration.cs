using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class AnchorCriterionConfiguration : IEntityTypeConfiguration<AnchorCriterion>
{
    public void Configure(EntityTypeBuilder<AnchorCriterion> builder)
    {
        builder.ToTable("anchor_criterion", "academic");
        builder.HasKey(x => new { x.AnchorAssessmentId, x.SyllabusTraceabilityId }).HasName("pk_anchor_criterion");
        builder.Property(x => x.AnchorAssessmentId).HasColumnName("anchor_assessment_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusTraceabilityId).HasColumnName("syllabus_traceability_id").HasColumnType("uuid").IsRequired();
        builder.HasOne(x => x.AnchorAssessment).WithMany().HasForeignKey(x => x.AnchorAssessmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_anchor_criterion_anchor_assessment");
        builder.HasOne(x => x.SyllabusTraceability).WithMany().HasForeignKey(x => x.SyllabusTraceabilityId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_anchor_criterion_traceability");
    }
}
