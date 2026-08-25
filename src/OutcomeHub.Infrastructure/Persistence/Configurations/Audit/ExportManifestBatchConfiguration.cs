using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Audit;

public sealed class ExportManifestBatchConfiguration : IEntityTypeConfiguration<ExportManifestBatch>
{
    public void Configure(EntityTypeBuilder<ExportManifestBatch> builder)
    {
        builder.ToTable("export_manifest_batch", "audit");
        builder.HasKey(x => new { x.ExportManifestId, x.ResultBatchId }).HasName("pk_export_manifest_batch");
        builder.Property(x => x.ExportManifestId).HasColumnName("export_manifest_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ResultBatchId).HasColumnName("result_batch_id").HasColumnType("uuid").IsRequired();
        builder.HasOne(x => x.ExportManifest).WithMany(x => x.ResultBatches).HasForeignKey(x => x.ExportManifestId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_export_manifest_batch_manifest");
        builder.HasOne(x => x.ResultBatch).WithMany().HasForeignKey(x => x.ResultBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_export_manifest_batch_result_batch");
        builder.HasIndex(x => x.ResultBatchId).HasDatabaseName("ix_export_manifest_batch_result_batch");
    }
}
