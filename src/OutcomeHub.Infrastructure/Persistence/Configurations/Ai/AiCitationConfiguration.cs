using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiCitationConfiguration : IEntityTypeConfiguration<AiCitation>
{
    public void Configure(EntityTypeBuilder<AiCitation> builder)
    {
        builder.ToTable("ai_citation", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ai_citation");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ArtifactId)
            .HasColumnName("artifact_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceSnapshotId)
            .HasColumnName("source_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PageNo)
            .HasColumnName("page_no")
            .HasColumnType("integer")
            .IsRequired(false);

        builder.Property(entity => entity.RegionPolygon)
            .HasColumnName("region_polygon")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(entity => entity.RowLocator)
            .HasColumnName("row_locator")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(entity => entity.SourceTextExcerpt)
            .HasColumnName("source_text_excerpt")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.SourceChecksum)
            .HasColumnName("source_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.ArtifactId, entity.SourceSnapshotId })
            .HasDatabaseName("ix_ai_citation_artifact_source");

        builder.HasOne(entity => entity.Artifact)
            .WithMany()
            .HasForeignKey(entity => entity.ArtifactId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_citation_artifact");

        builder.HasOne(entity => entity.SourceSnapshot)
            .WithMany()
            .HasForeignKey(entity => new { entity.SourceSnapshotId, entity.SourceChecksum })
            .HasPrincipalKey(entity => new { entity.Id, entity.SourceChecksum })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_citation_source_snapshot_checksum");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ai_citation_page", "page_no IS NULL OR page_no > 0");
            tableBuilder.HasCheckConstraint("ck_ai_citation_locator", "page_no IS NOT NULL OR row_locator IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_ai_citation_region", "region_polygon IS NULL OR page_no IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_ai_citation_checksum", "source_checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
