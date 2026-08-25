using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class CurrentPublicationConfiguration : IEntityTypeConfiguration<CurrentPublication>
{
    public void Configure(EntityTypeBuilder<CurrentPublication> builder)
    {
        builder.ToTable("current_publication", "result");

        builder.HasKey(entity => entity.MeasurementPeriodId)
            .HasName("pk_current_publication");

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PublicationId)
            .HasColumnName("publication_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.UpdatedBy)
            .HasColumnName("updated_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.RowVersion)
            .HasColumnName("row_version")
            .HasColumnType("bigint")
            .HasDefaultValue(1L)
            .IsConcurrencyToken()
            .IsRequired(true);

        builder.HasIndex(entity => entity.PublicationId)
            .IsUnique()
            .HasDatabaseName("uq_current_publication_1");

        builder.HasIndex(entity => entity.BatchId)
            .IsUnique()
            .HasDatabaseName("uq_current_publication_2");

        builder.HasOne(entity => entity.MeasurementPeriod).WithOne().HasForeignKey<CurrentPublication>(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_current_publication_measurement_period");
        builder.HasOne(entity => entity.Publication).WithOne().HasForeignKey<CurrentPublication>(entity => new { entity.PublicationId, entity.BatchId, entity.MeasurementPeriodId }).HasPrincipalKey<Publication>(entity => new { entity.Id, entity.BatchId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_current_publication_publication_batch_period");
        builder.HasOne(entity => entity.Batch).WithOne().HasForeignKey<CurrentPublication>(entity => new { entity.BatchId, entity.MeasurementPeriodId }).HasPrincipalKey<ResultBatch>(entity => new { entity.Id, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_current_publication_batch_period");
        builder.HasOne(entity => entity.UpdatedByPrincipal).WithMany().HasForeignKey(entity => entity.UpdatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_current_publication_updated_by");
        builder.ToTable(table => table.HasCheckConstraint("ck_current_publication_row_version", "row_version > 0"));
    }
}
