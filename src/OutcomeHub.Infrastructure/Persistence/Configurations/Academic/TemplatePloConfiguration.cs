using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class TemplatePloConfiguration : IEntityTypeConfiguration<TemplatePlo>
{
    public void Configure(EntityTypeBuilder<TemplatePlo> builder)
    {
        builder.ToTable("template_plo", "academic", table =>
        {
            table.HasCheckConstraint("ck_template_plo_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_template_plo_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_template_plo");
        builder.HasAlternateKey(x => new { x.Id, x.InstitutionTemplateVersionId }).HasName("uq_template_plo_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.Domain).HasColumnName("domain").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.BloomLevel).HasColumnName("bloom_level").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.Property(x => x.IsLocked).HasColumnName("is_locked").HasColumnType("boolean").IsRequired();
        builder.HasIndex(x => new { x.InstitutionTemplateVersionId, x.Code }).IsUnique().HasDatabaseName("uq_template_plo_version_code");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_template_plo_version");
    }
}
