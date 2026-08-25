using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class StagingCourseOfferingConfiguration : IEntityTypeConfiguration<StagingCourseOffering>
{
    public void Configure(EntityTypeBuilder<StagingCourseOffering> builder)
    {
        builder.ToTable("staging_course_offering", "integration", table =>
        {
            table.HasCheckConstraint("ck_staging_course_offering_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_staging_course_offering_validation_status", "validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')");
            table.HasCheckConstraint("ck_staging_course_offering_checksum", "row_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_staging_course_offering");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.OfferingCode).HasColumnName("offering_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CourseCode).HasColumnName("course_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AcademicYear).HasColumnName("academic_year").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.TermCode).HasColumnName("term_code").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SectionCode).HasColumnName("section_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.ResolvedCourseOfferingId).HasColumnName("resolved_course_offering_id").HasColumnType("uuid");
        builder.Property(x => x.ValidationStatus).HasColumnName("validation_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.RowChecksum).HasColumnName("row_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_course_offering_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_course_offering_raw_record");
        builder.HasOne(x => x.ResolvedCourseOffering).WithMany().HasForeignKey(x => x.ResolvedCourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_course_offering_resolved_offering");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_staging_course_offering_batch_row");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_staging_course_offering_raw_record");
    }
}
