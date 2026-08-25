using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class CalculationRunConfiguration : IEntityTypeConfiguration<CalculationRun>
{
    public void Configure(EntityTypeBuilder<CalculationRun> builder)
    {
        builder.ToTable("calculation_run", "result");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_calculation_run");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AttemptNo)
            .HasColumnName("attempt_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.WorkerId)
            .HasColumnName("worker_id")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.HeartbeatAt)
            .HasColumnName("heartbeat_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.ProgressRatio)
            .HasColumnName("progress_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.ErrorCode)
            .HasColumnName("error_code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.ErrorDetail)
            .HasColumnName("error_detail")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.LogReference)
            .HasColumnName("log_reference")
            .HasColumnType("varchar(512)")
            .HasMaxLength(512)
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.BatchId, entity.AttemptNo })
            .IsUnique()
            .HasDatabaseName("uq_calculation_run_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_calculation_run_attempt_no", "attempt_no > 0");
            table.HasCheckConstraint("ck_calculation_run_progress", "progress_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND progress_ratio >= 0 AND progress_ratio <= 1");
            table.HasCheckConstraint("ck_calculation_run_times", "(heartbeat_at IS NULL OR heartbeat_at >= started_at) AND (completed_at IS NULL OR completed_at >= started_at)");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => entity.BatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_calculation_run_batch");
    }
}
