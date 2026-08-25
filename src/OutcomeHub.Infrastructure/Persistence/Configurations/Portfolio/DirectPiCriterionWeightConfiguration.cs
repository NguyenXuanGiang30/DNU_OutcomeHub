using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class DirectPiCriterionWeightConfiguration : IEntityTypeConfiguration<DirectPiCriterionWeight>
{
    public void Configure(EntityTypeBuilder<DirectPiCriterionWeight> builder)
    {
        builder.ToTable("direct_pi_criterion_weight", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_direct_pi_criterion_weight_ratio", "direct_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND direct_weight_ratio > 0 AND direct_weight_ratio <= 1");
            });
        builder.HasKey(x => x.Id).HasName("pk_direct_pi_criterion_weight");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusTraceabilityId).HasColumnName("syllabus_traceability_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DirectWeightRatio).HasColumnName("direct_weight_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired();
        builder.Property(x => x.IsCoreGate).HasColumnName("is_core_gate").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz").IsRequired(false);

        builder.HasIndex(x => x.SyllabusTraceabilityId).IsUnique().HasDatabaseName("uq_direct_pi_criterion_weight_traceability");
        builder.HasOne(x => x.SyllabusTraceability).WithOne(x => x.DirectPiCriterionWeight).HasForeignKey<DirectPiCriterionWeight>(x => x.SyllabusTraceabilityId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_pi_criterion_weight_traceability");
    }
}
