using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class StagingStudentConfiguration : IEntityTypeConfiguration<StagingStudent>
{
    public void Configure(EntityTypeBuilder<StagingStudent> builder)
    {
        builder.ToTable("staging_student", "integration", table =>
        {
            table.HasCheckConstraint("ck_staging_student_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_staging_student_code", "student_code = btrim(student_code) AND char_length(student_code) > 0");
            table.HasCheckConstraint("ck_staging_student_validation_status", "validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')");
            table.HasCheckConstraint("ck_staging_student_checksum", "row_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_staging_student");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.StudentCode).HasColumnName("student_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.FullName).HasColumnName("full_name").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.Email).HasColumnName("email").HasColumnType("varchar(320)").HasMaxLength(320);
        builder.Property(x => x.ResolvedStudentId).HasColumnName("resolved_student_id").HasColumnType("uuid");
        builder.Property(x => x.ValidationStatus).HasColumnName("validation_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.RowChecksum).HasColumnName("row_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_student_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_student_raw_record");
        builder.HasOne(x => x.ResolvedStudent).WithMany().HasForeignKey(x => x.ResolvedStudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_student_resolved_student");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_staging_student_batch_row");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_staging_student_raw_record");
    }
}
