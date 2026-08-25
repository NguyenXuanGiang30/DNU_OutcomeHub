using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiReviewEventConfiguration : IEntityTypeConfiguration<AiReviewEvent>
{
    public void Configure(EntityTypeBuilder<AiReviewEvent> builder)
    {
        builder.ToTable("ai_review_event", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ai_review_event");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ArtifactId)
            .HasColumnName("artifact_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Decision)
            .HasColumnName("decision")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ProposedBefore)
            .HasColumnName("proposed_before")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.FinalValue)
            .HasColumnName("final_value")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(entity => entity.Reason)
            .HasColumnName("reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.ReviewerPrincipalId)
            .HasColumnName("reviewer_principal_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OccurredAt)
            .HasColumnName("occurred_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.ArtifactId, entity.OccurredAt })
            .HasDatabaseName("ix_ai_review_event_artifact_time");

        builder.HasOne(entity => entity.Artifact)
            .WithMany()
            .HasForeignKey(entity => entity.ArtifactId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_review_event_artifact");

        builder.HasOne(entity => entity.Reviewer)
            .WithMany()
            .HasForeignKey(entity => entity.ReviewerPrincipalId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_review_event_reviewer");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ai_review_event_decision", "decision IN ('ACCEPTED','EDITED','REJECTED','APPLIED')");
            tableBuilder.HasCheckConstraint("ck_ai_review_event_final_value", "decision <> 'EDITED' OR final_value IS NOT NULL");
        });
    }
}
