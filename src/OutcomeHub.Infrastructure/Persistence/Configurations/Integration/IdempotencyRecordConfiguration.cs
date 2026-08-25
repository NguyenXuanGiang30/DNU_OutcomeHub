using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_record", "integration", table =>
        {
            table.HasCheckConstraint("ck_idempotency_record_request_hash", "request_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_idempotency_record_status", "status IN ('IN_PROGRESS', 'SUCCEEDED', 'FAILED_FINAL')");
            table.HasCheckConstraint("ck_idempotency_record_lock", "num_nonnulls(locked_by, locked_until) IN (0, 2)");
            table.HasCheckConstraint("ck_idempotency_record_times", "expires_at > created_at AND (completed_at IS NULL OR completed_at >= created_at)");
            table.HasCheckConstraint("ck_idempotency_record_response_status", "response_status IS NULL OR response_status BETWEEN 100 AND 599");
        });
        builder.HasKey(x => x.Id).HasName("pk_idempotency_record");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.OperationCode).HasColumnName("operation_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.IdempotencyKey).HasColumnName("idempotency_key").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.RequestHash).HasColumnName("request_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.LockedBy).HasColumnName("locked_by").HasColumnType("uuid");
        builder.Property(x => x.LockedUntil).HasColumnName("locked_until").HasColumnType("timestamptz");
        builder.Property(x => x.ResponseStatus).HasColumnName("response_status").HasColumnType("integer");
        builder.Property(x => x.ResponseHeaders).HasColumnName("response_headers").HasColumnType("jsonb");
        builder.Property(x => x.ResponseBody).HasColumnName("response_body").HasColumnType("jsonb");
        builder.Property(x => x.ResourceId).HasColumnName("resource_id").HasColumnType("uuid");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamptz").IsRequired();
        builder.HasOne(x => x.Principal).WithMany().HasForeignKey(x => x.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idempotency_record_principal");
        builder.HasOne(x => x.LockedByPrincipal).WithMany().HasForeignKey(x => x.LockedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idempotency_record_locked_by");
        builder.HasIndex(x => new { x.PrincipalId, x.OperationCode, x.IdempotencyKey }).IsUnique().HasDatabaseName("uq_idempotency_record_principal_operation_key");
        builder.HasIndex(x => new { x.Status, x.LockedUntil }).HasDatabaseName("ix_idempotency_record_status_lock");
        builder.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_idempotency_record_expires_at");
    }
}
