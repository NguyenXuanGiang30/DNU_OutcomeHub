using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class StagingRubricCriterionConfiguration : IEntityTypeConfiguration<StagingRubricCriterion>
{
    public void Configure(EntityTypeBuilder<StagingRubricCriterion> builder)
    {
        builder.ToTable("staging_rubric_criterion", "integration", table =>
        {
            table.HasCheckConstraint("ck_staging_rubric_criterion_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_staging_rubric_criterion_max_score", "max_score IS NULL OR (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0)");
            table.HasCheckConstraint("ck_staging_rubric_criterion_validation_status", "validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')");
            table.HasCheckConstraint("ck_staging_rubric_criterion_checksum", "row_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_staging_rubric_criterion");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.RubricCode).HasColumnName("rubric_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CriterionCode).HasColumnName("criterion_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.MaxScore).HasColumnName("max_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10);
        builder.Property(x => x.ResolvedRubricCriterionId).HasColumnName("resolved_rubric_criterion_id").HasColumnType("uuid");
        builder.Property(x => x.ValidationStatus).HasColumnName("validation_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.RowChecksum).HasColumnName("row_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_rubric_criterion_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_rubric_criterion_raw_record");
        builder.HasOne(x => x.ResolvedRubricCriterion).WithMany().HasForeignKey(x => x.ResolvedRubricCriterionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_rubric_criterion_resolved_criterion");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_staging_rubric_criterion_batch_row");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_staging_rubric_criterion_raw_record");
    }
}
