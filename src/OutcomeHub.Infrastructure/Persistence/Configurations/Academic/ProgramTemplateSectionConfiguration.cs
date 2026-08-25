using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramTemplateSectionConfiguration : IEntityTypeConfiguration<ProgramTemplateSection>
{
    public void Configure(EntityTypeBuilder<ProgramTemplateSection> builder)
    {
        builder.ToTable("program_template_section", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_template_section_code", "section_code = upper(btrim(section_code)) AND char_length(section_code) > 0");
            table.HasCheckConstraint("ck_program_template_section_lock_mode", "lock_mode IN ('LOCKED','OVERRIDABLE','OPEN')");
            table.HasCheckConstraint("ck_program_template_section_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_template_section");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SectionCode).HasColumnName("section_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Required).HasColumnName("required").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.LockMode).HasColumnName("lock_mode").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.HasIndex(x => new { x.InstitutionTemplateVersionId, x.SectionCode }).IsUnique().HasDatabaseName("uq_program_template_section_version_code");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_template_section_version");
    }
}
