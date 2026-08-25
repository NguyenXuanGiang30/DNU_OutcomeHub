using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTemplateSectionConfiguration : IEntityTypeConfiguration<SyllabusTemplateSection>
{
    public void Configure(EntityTypeBuilder<SyllabusTemplateSection> builder)
    {
        builder.ToTable("syllabus_template_section", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_template_section_code", "section_code = upper(btrim(section_code)) AND char_length(section_code) > 0");
                table.HasCheckConstraint("ck_syllabus_template_section_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_template_section");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusTemplateVersionId).HasColumnName("syllabus_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SectionCode).HasColumnName("section_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Required).HasColumnName("required").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.ContentType).HasColumnName("content_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Locked).HasColumnName("locked").HasColumnType("boolean").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusTemplateVersionId }).HasName("uq_syllabus_template_section_id_version");
        builder.HasIndex(x => new { x.SyllabusTemplateVersionId, x.SectionCode }).IsUnique().HasDatabaseName("uq_syllabus_template_section_code");
        builder.HasOne(x => x.SyllabusTemplateVersion).WithMany(x => x.Sections).HasForeignKey(x => x.SyllabusTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_section_version");
    }
}

