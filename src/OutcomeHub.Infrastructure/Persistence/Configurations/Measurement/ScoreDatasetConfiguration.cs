using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class ScoreDatasetConfiguration : IEntityTypeConfiguration<ScoreDataset>
{
    public void Configure(EntityTypeBuilder<ScoreDataset> builder)
    {
        builder.ToTable("score_dataset", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_score_dataset");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceSystemId)
            .HasColumnName("source_system_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Classification)
            .HasColumnName("classification")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_score_dataset_1");

        builder.HasIndex(entity => new { entity.Id, entity.CourseOfferingId, entity.AcademicYearStart })
            .IsUnique()
            .HasDatabaseName("uq_score_dataset_2");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_score_dataset_academic_year", "academic_year_start BETWEEN 1900 AND 9999");
            table.HasCheckConstraint("ck_score_dataset_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
        });
        builder.HasOne(entity => entity.GovernedResource).WithMany().HasForeignKey(entity => entity.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_dataset_governed_resource");
        builder.HasOne(entity => entity.SourceSystem).WithMany().HasForeignKey(entity => entity.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_dataset_source_system");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => new { entity.CourseOfferingId, entity.AcademicYearStart }).HasPrincipalKey(entity => new { entity.Id, entity.AcademicYearStart }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_dataset_course_offering_year");
    }
}
