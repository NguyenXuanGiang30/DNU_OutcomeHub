using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTemplateFieldConfiguration : IEntityTypeConfiguration<SyllabusTemplateField>
{
    public void Configure(EntityTypeBuilder<SyllabusTemplateField> builder)
    {
        builder.ToTable("syllabus_template_field", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_template_field_code", "field_code = upper(btrim(field_code)) AND char_length(field_code) > 0");
                table.HasCheckConstraint("ck_syllabus_template_field_lock_mode", "lock_mode IN ('LOCKED','OVERRIDABLE','OPEN')");
                table.HasCheckConstraint("ck_syllabus_template_field_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_template_field");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusTemplateSectionId).HasColumnName("syllabus_template_section_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusTemplateVersionId).HasColumnName("syllabus_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.FieldCode).HasColumnName("field_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DataType).HasColumnName("data_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Required).HasColumnName("required").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.LockMode).HasColumnName("lock_mode").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DefaultValue).HasColumnName("default_value").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.ValidationSchema).HasColumnName("validation_schema").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusTemplateVersionId }).HasName("uq_syllabus_template_field_id_version");
        builder.HasIndex(x => new { x.SyllabusTemplateSectionId, x.FieldCode }).IsUnique().HasDatabaseName("uq_syllabus_template_field_section_code");
        builder.HasOne(x => x.SyllabusTemplateSection).WithMany(x => x.Fields).HasForeignKey(x => new { x.SyllabusTemplateSectionId, x.SyllabusTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_field_section_version");
        builder.HasOne(x => x.SyllabusTemplateVersion).WithMany().HasForeignKey(x => x.SyllabusTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_field_version");
    }
}

