using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class RemeasurementEvaluationConfiguration : IEntityTypeConfiguration<RemeasurementEvaluation>
{
    public void Configure(EntityTypeBuilder<RemeasurementEvaluation> builder)
    {
        builder.ToTable("remeasurement_evaluation", "quality");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_remeasurement_evaluation");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ImprovementPlanId)
            .HasColumnName("improvement_plan_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.BeforeBatchId)
            .HasColumnName("before_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AfterBatchId)
            .HasColumnName("after_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ComparabilityStatus)
            .HasColumnName("comparability_status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.BaselineValue)
            .HasColumnName("baseline_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.AfterValue)
            .HasColumnName("after_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.DeltaValue)
            .HasColumnName("delta_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.Conclusion)
            .HasColumnName("conclusion")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.VerifiedBy)
            .HasColumnName("verified_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VerifiedAt)
            .HasColumnName("verified_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_remeasurement_evaluation_batches", "before_batch_id <> after_batch_id");
            table.HasCheckConstraint("ck_remeasurement_evaluation_values", "(baseline_value IS NULL OR baseline_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (after_value IS NULL OR after_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (delta_value IS NULL OR delta_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric))");
            table.HasCheckConstraint("ck_remeasurement_evaluation_delta", "num_nonnulls(baseline_value, after_value, delta_value) IN (0, 3) AND (delta_value IS NULL OR delta_value = after_value - baseline_value)");
        });
        builder.HasOne(entity => entity.ImprovementPlan).WithMany().HasForeignKey(entity => entity.ImprovementPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_remeasurement_evaluation_plan");
        builder.HasOne(entity => entity.BeforeBatch).WithMany().HasForeignKey(entity => entity.BeforeBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_remeasurement_evaluation_before_batch");
        builder.HasOne(entity => entity.AfterBatch).WithMany().HasForeignKey(entity => entity.AfterBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_remeasurement_evaluation_after_batch");
        builder.HasOne(entity => entity.VerifiedByPrincipal).WithMany().HasForeignKey(entity => entity.VerifiedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_remeasurement_evaluation_verified_by");
        builder.HasIndex(entity => new { entity.ImprovementPlanId, entity.BeforeBatchId, entity.AfterBatchId }).IsUnique().HasDatabaseName("uq_remeasurement_evaluation_plan_batches");
    }
}
