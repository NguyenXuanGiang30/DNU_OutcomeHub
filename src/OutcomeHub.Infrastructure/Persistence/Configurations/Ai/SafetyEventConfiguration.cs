using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class SafetyEventConfiguration : IEntityTypeConfiguration<SafetyEvent>
{
    public void Configure(EntityTypeBuilder<SafetyEvent> builder)
    {
        builder.ToTable("safety_event", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_safety_event");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.AiJobId)
            .HasColumnName("ai_job_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EventType)
            .HasColumnName("event_type")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Severity)
            .HasColumnName("severity")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.DetectorVersion)
            .HasColumnName("detector_version")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Blocked)
            .HasColumnName("blocked")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.DetailsRedacted)
            .HasColumnName("details_redacted")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.OccurredAt)
            .HasColumnName("occurred_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.AiJobId, entity.Severity, entity.OccurredAt })
            .HasDatabaseName("ix_safety_event_job_severity_time");

        builder.HasOne(entity => entity.AiJob)
            .WithMany()
            .HasForeignKey(entity => entity.AiJobId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_safety_event_ai_job");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_safety_event_severity", "severity IN ('INFO','WARNING','ERROR','BLOCKING')");
            tableBuilder.HasCheckConstraint("ck_safety_event_type", "event_type = upper(btrim(event_type)) AND char_length(event_type) > 0");
        });
    }
}
