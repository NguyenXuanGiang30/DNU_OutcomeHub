using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class StagingScoreConfiguration : IEntityTypeConfiguration<StagingScore>
{
    public void Configure(EntityTypeBuilder<StagingScore> builder)
    {
        builder.ToTable("staging_score", "integration", table =>
        {
            table.HasCheckConstraint("ck_staging_score_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_staging_score_values", "(raw_score IS NULL OR raw_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (max_score IS NULL OR (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0))");
            table.HasCheckConstraint("ck_staging_score_validation_status", "validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')");
            table.HasCheckConstraint("ck_staging_score_checksum", "row_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_staging_score");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.StudentCode).HasColumnName("student_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.OfferingCode).HasColumnName("offering_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AssessmentCode).HasColumnName("assessment_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CriterionCode).HasColumnName("criterion_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.RawScore).HasColumnName("raw_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10);
        builder.Property(x => x.MaxScore).HasColumnName("max_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10);
        builder.Property(x => x.ResolvedScoreAcademicYearStart).HasColumnName("resolved_score_academic_year_start").HasColumnType("smallint");
        builder.Property(x => x.ResolvedScoreRecordId).HasColumnName("resolved_score_record_id").HasColumnType("uuid");
        builder.Property(x => x.ValidationStatus).HasColumnName("validation_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.RowChecksum).HasColumnName("row_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_score_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_score_raw_record");
        builder.HasOne(x => x.ResolvedScoreRecord).WithMany()
            .HasForeignKey(x => new { x.ResolvedScoreAcademicYearStart, x.ResolvedScoreRecordId })
            .HasPrincipalKey(x => new { ResolvedScoreAcademicYearStart = x.AcademicYearStart, ResolvedScoreRecordId = x.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_staging_score_resolved_score");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_staging_score_batch_row");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_staging_score_raw_record");
    }
}
