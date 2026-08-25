using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ops;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ops;

public sealed class JobAttemptConfiguration : IEntityTypeConfiguration<JobAttempt>
{
    public void Configure(EntityTypeBuilder<JobAttempt> builder)
    {
        builder.ToTable("job_attempt", "ops", table =>
        {
            table.HasCheckConstraint("ck_job_attempt_number", "attempt_no > 0");
            table.HasCheckConstraint("ck_job_attempt_times", "(heartbeat_at IS NULL OR heartbeat_at >= started_at) AND (finished_at IS NULL OR finished_at >= started_at)");
        });
        builder.HasKey(x => new { x.OperationJobId, x.AttemptNo }).HasName("pk_job_attempt");
        builder.Property(x => x.OperationJobId).HasColumnName("operation_job_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AttemptNo).HasColumnName("attempt_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.WorkerId).HasColumnName("worker_id").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.HeartbeatAt).HasColumnName("heartbeat_at").HasColumnType("timestamptz");
        builder.Property(x => x.FinishedAt).HasColumnName("finished_at").HasColumnType("timestamptz");
        builder.Property(x => x.Outcome).HasColumnName("outcome").HasColumnType("varchar(32)").HasMaxLength(32);
        builder.Property(x => x.ErrorCode).HasColumnName("error_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.LogReference).HasColumnName("log_reference").HasColumnType("varchar(1024)").HasMaxLength(1024);
        builder.HasOne(x => x.OperationJob).WithMany(x => x.Attempts).HasForeignKey(x => x.OperationJobId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_job_attempt_operation_job");
        builder.HasIndex(x => new { x.WorkerId, x.StartedAt }).HasDatabaseName("ix_job_attempt_worker_started");
    }
}
