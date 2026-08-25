using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class ResultReportDocumentConfiguration : IEntityTypeConfiguration<ResultReportDocument>
{
    public void Configure(EntityTypeBuilder<ResultReportDocument> builder)
    {
        builder.ToTable("result_report_document", "result");

        builder.HasKey(entity => new { entity.BatchId, entity.DocumentVersionId, entity.ReportType })
            .HasName("pk_result_report_document");

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DocumentVersionId)
            .HasColumnName("document_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ReportType)
            .HasColumnName("report_type")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.FilterChecksum)
            .HasColumnName("filter_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => entity.BatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_report_document_batch");
        builder.HasOne(entity => entity.DocumentVersion).WithMany().HasForeignKey(entity => entity.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_report_document_document_version");
        builder.ToTable(table => table.HasCheckConstraint("ck_result_report_document_filter_checksum", "filter_checksum ~ '^[0-9a-f]{64}$'"));
    }
}
