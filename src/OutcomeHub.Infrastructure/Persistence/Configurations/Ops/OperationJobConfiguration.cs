using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ops;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ops;

public sealed class OperationJobConfiguration : IEntityTypeConfiguration<OperationJob>
{
    public void Configure(EntityTypeBuilder<OperationJob> builder)
    {
        // subject_type/subject_id is a controlled polymorphic locator; allow-list and target existence
        // require trigger SQL. FOR UPDATE SKIP LOCKED claim semantics live in repository SQL.
        builder.ToTable("operation_job", "ops", table =>
        {
            table.HasCheckConstraint("ck_operation_job_type", "job_type IN ('IMPORT', 'EXPORT', 'CALCULATION', 'OCR', 'AI', 'WEBHOOK', 'REPORT_REFRESH')");
            table.HasCheckConstraint("ck_operation_job_status", "status IN ('QUEUED', 'RETRY_WAIT', 'RUNNING', 'SUCCEEDED', 'FAILED', 'CANCEL_REQUESTED', 'CANCELLED')");
            table.HasCheckConstraint("ck_operation_job_progress", "progress_current >= 0 AND (progress_total IS NULL OR (progress_total >= 0 AND progress_current <= progress_total))");
            table.HasCheckConstraint("ck_operation_job_attempts", "max_attempts > 0 AND attempt_count >= 0 AND attempt_count <= max_attempts");
            table.HasCheckConstraint("ck_operation_job_retryable", "status NOT IN ('QUEUED', 'RETRY_WAIT') OR attempt_count < max_attempts");
            table.HasCheckConstraint("ck_operation_job_running_lease", "status <> 'RUNNING' OR (leased_by_principal_id IS NOT NULL AND lease_until IS NOT NULL)");
            table.HasCheckConstraint("ck_operation_job_terminal", "status NOT IN ('SUCCEEDED', 'FAILED', 'CANCELLED') OR completed_at IS NOT NULL");
            table.HasCheckConstraint("ck_operation_job_cancel", "num_nonnulls(cancel_requested_by, cancel_requested_at) IN (0, 2)");
            table.HasCheckConstraint("ck_operation_job_row_version", "row_version > 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_operation_job");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.JobType).HasColumnName("job_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SubjectType).HasColumnName("subject_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SubjectId).HasColumnName("subject_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ProgressCurrent).HasColumnName("progress_current").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.ProgressTotal).HasColumnName("progress_total").HasColumnType("bigint");
        builder.Property(x => x.QueueName).HasColumnName("queue_name").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.TransportMessageId).HasColumnName("transport_message_id").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.AvailableAt).HasColumnName("available_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").HasColumnType("integer").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.AttemptCount).HasColumnName("attempt_count").HasColumnType("integer").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.MaxAttempts).HasColumnName("max_attempts").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RequestedBy).HasColumnName("requested_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LeasedByPrincipalId).HasColumnName("leased_by_principal_id").HasColumnType("uuid");
        builder.Property(x => x.LeaseUntil).HasColumnName("lease_until").HasColumnType("timestamptz");
        builder.Property(x => x.RequestId).HasColumnName("request_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CorrelationId).HasColumnName("correlation_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CancelRequestedBy).HasColumnName("cancel_requested_by").HasColumnType("uuid");
        builder.Property(x => x.CancelRequestedAt).HasColumnName("cancel_requested_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz");
        builder.Property(x => x.HeartbeatAt).HasColumnName("heartbeat_at").HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");
        builder.Property(x => x.ErrorCode).HasColumnName("error_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.ErrorDetailRedacted).HasColumnName("error_detail_redacted").HasColumnType("text");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").HasDefaultValue(1L).IsConcurrencyToken().IsRequired();
        builder.HasOne(x => x.RequestedByPrincipal).WithMany().HasForeignKey(x => x.RequestedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_operation_job_requested_by");
        builder.HasOne(x => x.AccessScope).WithMany().HasForeignKey(x => x.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_operation_job_access_scope");
        builder.HasOne(x => x.LeasedByPrincipal).WithMany().HasForeignKey(x => x.LeasedByPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_operation_job_leased_by");
        builder.HasOne(x => x.CancelRequestedByPrincipal).WithMany().HasForeignKey(x => x.CancelRequestedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_operation_job_cancel_requested_by");
        builder.HasIndex(x => new { x.QueueName, x.Status, x.AvailableAt, x.Priority, x.CreatedAt })
            .IsDescending(false, false, false, true, false)
            .HasFilter("status IN ('QUEUED','RETRY_WAIT')")
            .HasDatabaseName("ix_operation_job_claim");
        builder.HasIndex(x => x.LeaseUntil)
            .HasFilter("status = 'RUNNING'")
            .HasDatabaseName("ix_operation_job_expired_lease");
        builder.HasIndex(x => x.RequestId).HasDatabaseName("ix_operation_job_request_id");
        builder.HasIndex(x => x.CorrelationId).HasDatabaseName("ix_operation_job_correlation_id");
    }
}
