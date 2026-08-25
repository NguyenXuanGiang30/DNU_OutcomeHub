using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class SyncJobConfiguration : IEntityTypeConfiguration<SyncJob>
{
    public void Configure(EntityTypeBuilder<SyncJob> builder)
    {
        builder.ToTable("sync_job", "integration", table =>
        {
            table.HasCheckConstraint("ck_sync_job_counts", "read_count >= 0 AND accepted_count >= 0 AND rejected_count >= 0 AND accepted_count + rejected_count <= read_count");
            table.HasCheckConstraint("ck_sync_job_completion", "completed_at IS NULL OR completed_at >= started_at");
        });
        builder.HasKey(x => x.Id).HasName("pk_sync_job");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DataType).HasColumnName("data_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Mode).HasColumnName("mode").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CursorFrom).HasColumnName("cursor_from").HasColumnType("text");
        builder.Property(x => x.CursorTo).HasColumnName("cursor_to").HasColumnType("text");
        builder.Property(x => x.UpdatedSince).HasColumnName("updated_since").HasColumnType("timestamptz");
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");
        builder.Property(x => x.ReadCount).HasColumnName("read_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.AcceptedCount).HasColumnName("accepted_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.RejectedCount).HasColumnName("rejected_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.ErrorSummary).HasColumnName("error_summary").HasColumnType("text");
        builder.Property(x => x.RequestId).HasColumnName("request_id").HasColumnType("uuid").IsRequired();
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sync_job_source_system");
        builder.HasIndex(x => new { x.SourceSystemId, x.DataType, x.StartedAt }).HasDatabaseName("ix_sync_job_source_data_started");
        builder.HasIndex(x => x.RequestId).HasDatabaseName("ix_sync_job_request_id");
    }
}
