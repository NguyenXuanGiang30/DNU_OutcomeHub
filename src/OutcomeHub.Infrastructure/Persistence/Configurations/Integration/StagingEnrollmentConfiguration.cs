using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class StagingEnrollmentConfiguration : IEntityTypeConfiguration<StagingEnrollment>
{
    public void Configure(EntityTypeBuilder<StagingEnrollment> builder)
    {
        builder.ToTable("staging_enrollment", "integration", table =>
        {
            table.HasCheckConstraint("ck_staging_enrollment_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_staging_enrollment_validation_status", "validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')");
            table.HasCheckConstraint("ck_staging_enrollment_checksum", "row_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_staging_enrollment");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.StudentCode).HasColumnName("student_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.OfferingCode).HasColumnName("offering_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.EnrollmentStatus).HasColumnName("enrollment_status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ResolvedEnrollmentId).HasColumnName("resolved_enrollment_id").HasColumnType("uuid");
        builder.Property(x => x.ValidationStatus).HasColumnName("validation_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.RowChecksum).HasColumnName("row_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_enrollment_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_enrollment_raw_record");
        builder.HasOne(x => x.ResolvedEnrollment).WithMany().HasForeignKey(x => x.ResolvedEnrollmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_enrollment_resolved_enrollment");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_staging_enrollment_batch_row");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_staging_enrollment_raw_record");
    }
}
