using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class ValidationIssueConfiguration : IEntityTypeConfiguration<ValidationIssue>
{
    public void Configure(EntityTypeBuilder<ValidationIssue> builder)
    {
        // staging_table/staging_row_id is an intentionally controlled polymorphic locator;
        // the allow-list and target existence check require trigger SQL in a migration.
        builder.ToTable("validation_issue", "integration", table =>
        {
            table.HasCheckConstraint("ck_validation_issue_staging_locator", "num_nonnulls(staging_table, staging_row_id) IN (0, 2)");
            table.HasCheckConstraint("ck_validation_issue_severity", "severity IN ('INFO', 'WARNING', 'ERROR', 'BLOCKING')");
            table.HasCheckConstraint("ck_validation_issue_resolution", "(resolved_by IS NULL AND resolved_at IS NULL) OR (resolved_by IS NOT NULL AND resolved_at IS NOT NULL)");
        });
        builder.HasKey(x => x.Id).HasName("pk_validation_issue");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint");
        builder.Property(x => x.StagingTable).HasColumnName("staging_table").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.StagingRowId).HasColumnName("staging_row_id").HasColumnType("uuid");
        builder.Property(x => x.FieldName).HasColumnName("field_name").HasColumnType("varchar(128)").HasMaxLength(128);
        builder.Property(x => x.ErrorCode).HasColumnName("error_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Severity).HasColumnName("severity").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Message).HasColumnName("message").HasColumnType("text").IsRequired();
        builder.Property(x => x.SuggestedAction).HasColumnName("suggested_action").HasColumnType("text");
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ResolvedBy).HasColumnName("resolved_by").HasColumnType("uuid");
        builder.Property(x => x.ResolvedAt).HasColumnName("resolved_at").HasColumnType("timestamptz");
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_validation_issue_ingestion_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_validation_issue_raw_record");
        builder.HasOne(x => x.ResolvedByPrincipal).WithMany().HasForeignKey(x => x.ResolvedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_validation_issue_resolved_by");
        builder.HasIndex(x => new { x.IngestionBatchId, x.Severity, x.Status }).HasDatabaseName("ix_validation_issue_batch_severity_status");
        builder.HasIndex(x => new { x.StagingTable, x.StagingRowId }).HasDatabaseName("ix_validation_issue_staging_locator");
    }
}
