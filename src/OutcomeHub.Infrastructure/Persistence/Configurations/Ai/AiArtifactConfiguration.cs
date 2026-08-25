using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiArtifactConfiguration : IEntityTypeConfiguration<AiArtifact>
{
    public void Configure(EntityTypeBuilder<AiArtifact> builder)
    {
        builder.ToTable("ai_artifact", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ai_artifact");

        builder.HasAlternateKey(entity => new { entity.Id, entity.AiJobId })
            .HasName("uq_ai_artifact_id_job");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AiJobId)
            .HasColumnName("ai_job_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ArtifactType)
            .HasColumnName("artifact_type")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.TargetResourceType)
            .HasColumnName("target_resource_type")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.TargetResourceId)
            .HasColumnName("target_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.FieldPath)
            .HasColumnName("field_path")
            .HasColumnType("varchar(512)")
            .HasMaxLength(512)
            .IsRequired(true);

        builder.Property(entity => entity.ProposedValue)
            .HasColumnName("proposed_value")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.Confidence)
            .HasColumnName("confidence")
            .HasColumnType("numeric(5,4)")
            .HasPrecision(5, 4)
            .IsRequired(true);

        builder.Property(entity => entity.IsInferred)
            .HasColumnName("is_inferred")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.ReviewStatus)
            .HasColumnName("review_status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ReviewedBy)
            .HasColumnName("reviewed_by")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ReviewedAt)
            .HasColumnName("reviewed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.AppliedResourceVersion)
            .HasColumnName("applied_resource_version")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_ai_artifact_governed_resource");

        builder.HasIndex(entity => new { entity.AiJobId, entity.ReviewStatus, entity.ArtifactType })
            .HasDatabaseName("ix_ai_artifact_job_review_status_type");

        builder.HasIndex(entity => entity.ReviewedBy)
            .HasDatabaseName("ix_ai_artifact_reviewed_by");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_artifact_governed_resource");

        builder.HasOne(entity => entity.AiJob)
            .WithMany()
            .HasForeignKey(entity => new { entity.AiJobId, entity.TargetResourceType, entity.TargetResourceId })
            .HasPrincipalKey(entity => new { entity.Id, entity.TargetResourceType, entity.TargetResourceId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_artifact_job_target");

        builder.HasOne(entity => entity.Reviewer)
            .WithMany()
            .HasForeignKey(entity => entity.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_artifact_reviewed_by");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ai_artifact_confidence", "confidence >= 0 AND confidence <= 1 AND confidence <> 'NaN'::numeric AND confidence NOT IN ('Infinity'::numeric, '-Infinity'::numeric)");
            tableBuilder.HasCheckConstraint("ck_ai_artifact_review_status", "review_status IN ('PENDING','ACCEPTED','EDITED','REJECTED','APPLIED')");
            tableBuilder.HasCheckConstraint("ck_ai_artifact_review_pair", "(review_status = 'PENDING' AND reviewed_by IS NULL AND reviewed_at IS NULL) OR (review_status <> 'PENDING' AND reviewed_by IS NOT NULL AND reviewed_at IS NOT NULL)");
            tableBuilder.HasCheckConstraint("ck_ai_artifact_applied_version", "(review_status = 'APPLIED') = (applied_resource_version IS NOT NULL)");
        });
    }
}
