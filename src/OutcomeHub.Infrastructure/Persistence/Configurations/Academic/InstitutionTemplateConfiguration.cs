using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class InstitutionTemplateConfiguration : IEntityTypeConfiguration<InstitutionTemplate>
{
    public void Configure(EntityTypeBuilder<InstitutionTemplate> builder)
    {
        builder.ToTable("institution_template", "academic", table =>
            table.HasCheckConstraint("ck_institution_template_code", "code = upper(btrim(code)) AND char_length(code) > 0"));
        builder.HasKey(x => x.Id).HasName("pk_institution_template");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("uq_institution_template_code");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_institution_template_owner_org_unit");
    }
}
