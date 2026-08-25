using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("document_version", "document", table =>
            {
                table.HasCheckConstraint("ck_document_version_no", "version_no > 0");
                table.HasCheckConstraint("ck_document_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
                table.HasCheckConstraint("ck_document_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','SUPERSEDED','ARCHIVED')");
                table.HasCheckConstraint("ck_document_version_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
            });
        builder.HasKey(x => x.Id).HasName("pk_document_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DocumentId).HasColumnName("document_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.FileObjectId).HasColumnName("file_object_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SourceDocumentVersionId).HasColumnName("source_document_version_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.GenerationProvenance).HasColumnName("generation_provenance").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.StructuredContent).HasColumnName("structured_content").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.ContentSchemaVersion).HasColumnName("content_schema_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired(false);
        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => x.GovernedResourceId).IsUnique().HasDatabaseName("uq_document_version_governed_resource");
        builder.HasIndex(x => new { x.DocumentId, x.VersionNo }).IsUnique().HasDatabaseName("uq_document_version_document_no");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasFilter("workflow_instance_id IS NOT NULL").HasDatabaseName("uq_document_version_workflow");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_governed_resource");
        builder.HasOne(x => x.Document).WithMany(x => x.Versions).HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_document");
        builder.HasOne(x => x.FileObject).WithMany().HasForeignKey(x => x.FileObjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_file_object");
        builder.HasOne(x => x.SourceDocumentVersion).WithMany().HasForeignKey(x => x.SourceDocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_source");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany().HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_supersedes");
        builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_approver");
        builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_version_creator");
    }
}

