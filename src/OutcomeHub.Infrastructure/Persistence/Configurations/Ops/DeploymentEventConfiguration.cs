using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ops;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ops;

public sealed class DeploymentEventConfiguration : IEntityTypeConfiguration<DeploymentEvent>
{
    public void Configure(EntityTypeBuilder<DeploymentEvent> builder)
    {
        builder.ToTable("deployment_event", "ops", table =>
        {
            table.HasCheckConstraint("ck_deployment_event_completion", "completed_at IS NULL OR completed_at >= started_at");
            table.HasCheckConstraint("ck_deployment_event_duration", "duration_ms IS NULL OR duration_ms >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_deployment_event");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ApplicationRelease).HasColumnName("application_release").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.MigrationVersionFrom).HasColumnName("migration_version_from").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.MigrationVersionTo).HasColumnName("migration_version_to").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");
        builder.Property(x => x.Actor).HasColumnName("actor").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DurationMs).HasColumnName("duration_ms").HasColumnType("bigint");
        builder.Property(x => x.LogReference).HasColumnName("log_reference").HasColumnType("varchar(1024)").HasMaxLength(1024);
        builder.HasIndex(x => new { x.ApplicationRelease, x.StartedAt }).HasDatabaseName("ix_deployment_event_release_started");
    }
}
