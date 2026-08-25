using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusDocumentConfiguration : IEntityTypeConfiguration<SyllabusDocument>
{
    public void Configure(EntityTypeBuilder<SyllabusDocument> builder)
    {
        builder.ToTable("syllabus_document", "portfolio");
        builder.HasKey(x => new { x.SyllabusVersionId, x.DocumentVersionId, x.DocumentRole }).HasName("pk_syllabus_document");
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DocumentVersionId).HasColumnName("document_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DocumentRole).HasColumnName("document_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();

        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_document_syllabus_version");
        builder.HasOne(x => x.DocumentVersion).WithMany().HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_document_document_version");
    }
}

