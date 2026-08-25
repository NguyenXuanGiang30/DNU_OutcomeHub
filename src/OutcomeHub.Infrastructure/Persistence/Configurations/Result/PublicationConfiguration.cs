using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class PublicationConfiguration : IEntityTypeConfiguration<Publication>
{
    public void Configure(EntityTypeBuilder<Publication> builder)
    {
        builder.ToTable("publication", "result");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_publication");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PublicationType)
            .HasColumnName("publication_type")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.PublishedBy)
            .HasColumnName("published_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PublishedAt)
            .HasColumnName("published_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.WatermarkTemplate)
            .HasColumnName("watermark_template")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.DocumentVersionId)
            .HasColumnName("document_version_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.Id, entity.BatchId, entity.MeasurementPeriodId })
            .IsUnique()
            .HasDatabaseName("uq_publication_1");

        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_measurement_period");
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { entity.Id, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_batch_period");
        builder.HasOne(entity => entity.PublishedByPrincipal).WithMany().HasForeignKey(entity => entity.PublishedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_published_by");
        builder.HasOne(entity => entity.DocumentVersion).WithMany().HasForeignKey(entity => entity.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_document_version");
        builder.ToTable(table => table.HasCheckConstraint("ck_publication_type", "publication_type = btrim(publication_type) AND char_length(publication_type) > 0"));
    }
}
