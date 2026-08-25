using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class DocumentRenditionConfiguration : IEntityTypeConfiguration<DocumentRendition>
{
    public void Configure(EntityTypeBuilder<DocumentRendition> builder)
    {
        builder.ToTable("document_rendition", "document", table =>
            {
                table.HasCheckConstraint("ck_document_rendition_type", "rendition_type IN ('SOURCE','DOCX','PDF','XLSX','PREVIEW')");
                table.HasCheckConstraint("ck_document_rendition_checksum", "checksum ~ '^[0-9a-f]{64}$'");
                table.HasCheckConstraint("ck_document_rendition_template_checksum", "template_checksum IS NULL OR template_checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_document_rendition");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.DocumentVersionId).HasColumnName("document_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RenditionType).HasColumnName("rendition_type").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.FileObjectId).HasColumnName("file_object_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RendererName).HasColumnName("renderer_name").HasColumnType("varchar(127)").HasMaxLength(127).IsRequired();
        builder.Property(x => x.RendererVersion).HasColumnName("renderer_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.TemplateChecksum).HasColumnName("template_checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.DocumentVersionId, x.RenditionType }).IsUnique().HasDatabaseName("uq_document_rendition_version_type");
        builder.HasOne(x => x.DocumentVersion).WithMany(x => x.Renditions).HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_rendition_document_version");
        builder.HasOne(x => x.FileObject).WithMany().HasForeignKey(x => x.FileObjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_rendition_file_object");
    }
}

