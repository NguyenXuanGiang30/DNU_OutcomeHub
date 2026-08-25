using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusSectionContentConfiguration : IEntityTypeConfiguration<SyllabusSectionContent>
{
    public void Configure(EntityTypeBuilder<SyllabusSectionContent> builder)
    {
        builder.ToTable("syllabus_section_content", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_section_content_value", "num_nonnulls(content_text, content_jsonb) = 1");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_section_content");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusTemplateVersionId).HasColumnName("syllabus_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.TemplateFieldId).HasColumnName("template_field_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ContentText).HasColumnName("content_text").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.ContentJsonb).HasColumnName("content_jsonb").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.SourceKind).HasColumnName("source_kind").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.IsInherited).HasColumnName("is_inherited").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.LastEditedBy).HasColumnName("last_edited_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").IsRequired();

        builder.HasIndex(x => new { x.SyllabusVersionId, x.TemplateFieldId }).IsUnique().HasDatabaseName("uq_syllabus_section_content_version_field");
        builder.Property(x => x.RowVersion).HasDefaultValue(1L).IsConcurrencyToken();
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => new { x.SyllabusVersionId, x.SyllabusTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_section_content_syllabus_template");
        builder.HasOne(x => x.SyllabusTemplateVersion).WithMany().HasForeignKey(x => x.SyllabusTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_section_content_template_version");
        builder.HasOne(x => x.TemplateField).WithMany().HasForeignKey(x => new { x.TemplateFieldId, x.SyllabusTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_section_content_template_field");
        builder.HasOne(x => x.LastEditor).WithMany().HasForeignKey(x => x.LastEditedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_section_content_last_editor");
    }
}

