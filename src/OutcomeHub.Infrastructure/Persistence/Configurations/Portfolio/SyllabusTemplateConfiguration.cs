using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTemplateConfiguration : IEntityTypeConfiguration<SyllabusTemplate>
{
    public void Configure(EntityTypeBuilder<SyllabusTemplate> builder)
    {
        builder.ToTable("syllabus_template", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_template_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_template");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired(false);

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("uq_syllabus_template_code");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_template_owner_org_unit");
    }
}

