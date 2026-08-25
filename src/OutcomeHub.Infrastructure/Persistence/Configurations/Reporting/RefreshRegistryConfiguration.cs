using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Reporting;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Reporting;

public sealed class RefreshRegistryConfiguration : IEntityTypeConfiguration<RefreshRegistry>
{
    public void Configure(EntityTypeBuilder<RefreshRegistry> builder)
    {
        builder.ToTable("refresh_registry", "reporting");

        builder.HasKey(entity => entity.ViewName)
            .HasName("pk_refresh_registry");

        builder.Property(entity => entity.ViewName)
            .HasColumnName("view_name")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.LastStartedAt)
            .HasColumnName("last_started_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.LastCompletedAt)
            .HasColumnName("last_completed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.SourceWatermark)
            .HasColumnName("source_watermark")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(entity => entity.RowCount)
            .HasColumnName("row_count")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(entity => entity.DurationMs)
            .HasColumnName("duration_ms")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(entity => entity.Error)
            .HasColumnName("error")
            .HasColumnType("text")
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.Status, entity.LastStartedAt })
            .HasDatabaseName("ix_refresh_registry_status_started_at");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_refresh_registry_view_name", "view_name = lower(btrim(view_name)) AND char_length(view_name) > 0 AND view_name ~ '^[a-z][a-z0-9_]*(\\.[a-z][a-z0-9_]*)?$'");
            tableBuilder.HasCheckConstraint("ck_refresh_registry_status", "status IN ('PENDING','RUNNING','SUCCEEDED','FAILED')");
            tableBuilder.HasCheckConstraint("ck_refresh_registry_time", "last_completed_at IS NULL OR (last_started_at IS NOT NULL AND last_completed_at >= last_started_at)");
            tableBuilder.HasCheckConstraint("ck_refresh_registry_state_time", "(status = 'PENDING') OR (status = 'RUNNING' AND last_started_at IS NOT NULL) OR (status IN ('SUCCEEDED','FAILED') AND last_started_at IS NOT NULL AND last_completed_at IS NOT NULL)");
            tableBuilder.HasCheckConstraint("ck_refresh_registry_counts", "(row_count IS NULL OR row_count >= 0) AND (duration_ms IS NULL OR duration_ms >= 0)");
            tableBuilder.HasCheckConstraint("ck_refresh_registry_error", "status <> 'FAILED' OR error IS NOT NULL");
        });
    }
}
