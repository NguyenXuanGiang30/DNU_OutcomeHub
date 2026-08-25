using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class QuarantineRecordConfiguration : IEntityTypeConfiguration<QuarantineRecord>
{
    public void Configure(EntityTypeBuilder<QuarantineRecord> builder)
    {
        builder.ToTable("quarantine_record", "integration", table =>
        {
            table.HasCheckConstraint("ck_quarantine_record_row_version", "row_version > 0");
            table.HasCheckConstraint("ck_quarantine_record_resolution", "(resolved_by IS NULL AND resolved_at IS NULL) OR (resolved_by IS NOT NULL AND resolved_at IS NOT NULL AND char_length(btrim(resolution_reason)) > 0)");
        });
        builder.HasKey(x => x.Id).HasName("pk_quarantine_record");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RawRecordId).HasColumnName("raw_record_id").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.ReasonCode).HasColumnName("reason_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.OwnerPrincipalId).HasColumnName("owner_principal_id").HasColumnType("uuid");
        builder.Property(x => x.CurrentCorrectionId).HasColumnName("current_correction_id").HasColumnType("uuid");
        builder.Property(x => x.ResolutionReason).HasColumnName("resolution_reason").HasColumnType("text");
        builder.Property(x => x.ResolvedBy).HasColumnName("resolved_by").HasColumnType("uuid");
        builder.Property(x => x.ResolvedAt).HasColumnName("resolved_at").HasColumnType("timestamptz");
        builder.Property(x => x.ReprocessBatchId).HasColumnName("reprocess_batch_id").HasColumnType("uuid");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").HasDefaultValue(1L).IsConcurrencyToken().IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_record_ingestion_batch");
        builder.HasOne(x => x.RawRecord).WithMany().HasForeignKey(x => x.RawRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_record_raw_record");
        builder.HasOne(x => x.OwnerPrincipal).WithMany().HasForeignKey(x => x.OwnerPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_record_owner");
        builder.HasOne(x => x.CurrentCorrection).WithOne().HasForeignKey<QuarantineRecord>(x => x.CurrentCorrectionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_record_current_correction");
        builder.HasOne(x => x.ResolvedByPrincipal).WithMany().HasForeignKey(x => x.ResolvedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_record_resolved_by");
        builder.HasOne(x => x.ReprocessBatch).WithMany().HasForeignKey(x => x.ReprocessBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_record_reprocess_batch");
        builder.HasIndex(x => x.RawRecordId).IsUnique().HasDatabaseName("uq_quarantine_record_raw_record");
        builder.HasIndex(x => x.CurrentCorrectionId).IsUnique().HasDatabaseName("uq_quarantine_record_current_correction");
        builder.HasIndex(x => new { x.Status, x.OwnerPrincipalId }).HasDatabaseName("ix_quarantine_record_status_owner");
    }
}
