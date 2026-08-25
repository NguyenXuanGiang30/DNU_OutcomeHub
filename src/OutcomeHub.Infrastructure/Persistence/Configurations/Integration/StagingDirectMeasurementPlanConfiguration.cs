using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class StagingDirectMeasurementPlanConfiguration : IEntityTypeConfiguration<StagingDirectMeasurementPlan>
{
    public void Configure(EntityTypeBuilder<StagingDirectMeasurementPlan> builder)
    {
        builder.ToTable("staging_direct_measurement_plan", "integration", table =>
        {
            table.HasCheckConstraint("ck_staging_direct_measurement_plan_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_staging_direct_measurement_plan_weight", "weight IS NULL OR (weight NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND weight >= 0 AND weight <= 1)");
            table.HasCheckConstraint("ck_staging_direct_measurement_plan_validation_status", "validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')");
            table.HasCheckConstraint("ck_staging_direct_measurement_plan_checksum", "row_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_staging_direct_measurement_plan");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.CourseCode).HasColumnName("course_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.PiCode).HasColumnName("pi_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AssessmentCode).HasColumnName("assessment_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CriterionCode).HasColumnName("criterion_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.Weight).HasColumnName("weight").HasColumnType("numeric(12,10)").HasPrecision(12, 10);
        builder.Property(x => x.ResolvedDirectMeasurementPlanId).HasColumnName("resolved_direct_measurement_plan_id").HasColumnType("uuid");
        builder.Property(x => x.ValidationStatus).HasColumnName("validation_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.RowChecksum).HasColumnName("row_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_direct_measurement_plan_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_direct_measurement_plan_raw_record");
        builder.HasOne(x => x.ResolvedDirectMeasurementPlan).WithMany().HasForeignKey(x => x.ResolvedDirectMeasurementPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staging_direct_measurement_plan_resolved_plan");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_staging_direct_measurement_plan_batch_row");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_staging_direct_measurement_plan_raw_record");
    }
}
