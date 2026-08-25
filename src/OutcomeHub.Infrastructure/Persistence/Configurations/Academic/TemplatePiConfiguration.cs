using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class TemplatePiConfiguration : IEntityTypeConfiguration<TemplatePi>
{
    public void Configure(EntityTypeBuilder<TemplatePi> builder)
    {
        builder.ToTable("template_pi", "academic", table =>
        {
            table.HasCheckConstraint("ck_template_pi_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_template_pi_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_template_pi");
        builder.HasAlternateKey(x => new { x.Id, x.InstitutionTemplateVersionId }).HasName("uq_template_pi_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.TemplatePloId).HasColumnName("template_plo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.Property(x => x.IsLocked).HasColumnName("is_locked").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.IsCore).HasColumnName("is_core").HasColumnType("boolean").IsRequired();
        builder.HasIndex(x => new { x.InstitutionTemplateVersionId, x.Code }).IsUnique().HasDatabaseName("uq_template_pi_version_code");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_template_pi_version");
        builder.HasOne(x => x.TemplatePlo).WithMany().HasForeignKey(x => new { x.TemplatePloId, x.InstitutionTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.InstitutionTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_template_pi_plo_version");
    }
}
