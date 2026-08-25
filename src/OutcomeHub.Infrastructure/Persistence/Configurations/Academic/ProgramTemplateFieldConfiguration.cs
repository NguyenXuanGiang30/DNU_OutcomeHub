using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramTemplateFieldConfiguration : IEntityTypeConfiguration<ProgramTemplateField>
{
    public void Configure(EntityTypeBuilder<ProgramTemplateField> builder)
    {
        builder.ToTable("program_template_field", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_template_field_code", "field_code = upper(btrim(field_code)) AND char_length(field_code) > 0");
            table.HasCheckConstraint("ck_program_template_field_lock_mode", "lock_mode IN ('LOCKED','OVERRIDABLE','OPEN')");
            table.HasCheckConstraint("ck_program_template_field_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_template_field");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramTemplateSectionId).HasColumnName("program_template_section_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.FieldCode).HasColumnName("field_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DataType).HasColumnName("data_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Required).HasColumnName("required").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.LockMode).HasColumnName("lock_mode").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DefaultValue).HasColumnName("default_value").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.ValidationSchema).HasColumnName("validation_schema").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.ProgramTemplateSectionId, x.FieldCode }).IsUnique().HasDatabaseName("uq_program_template_field_section_code");
        builder.HasOne(x => x.ProgramTemplateSection).WithMany().HasForeignKey(x => x.ProgramTemplateSectionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_template_field_section");
    }
}
