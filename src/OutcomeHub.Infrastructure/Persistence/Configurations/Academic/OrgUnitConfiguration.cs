using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class OrgUnitConfiguration : IEntityTypeConfiguration<OrgUnit>
{
    public void Configure(EntityTypeBuilder<OrgUnit> builder)
    {
        builder.ToTable("org_unit", "academic", table =>
        {
            table.HasCheckConstraint("ck_org_unit_unit_type", "unit_type IN ('UNIVERSITY','CAMPUS','FACULTY','INSTITUTE','DEPARTMENT','CENTER')");
            table.HasCheckConstraint("ck_org_unit_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_org_unit_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_org_unit_code", "code = upper(btrim(code)) AND char_length(code) > 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_org_unit");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ParentId).HasColumnName("parent_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.UnitType).HasColumnName("unit_type").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").HasDefaultValue(1L).IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("uq_org_unit_code");
        builder.HasIndex(x => x.ParentId).HasDatabaseName("ix_org_unit_parent_id");
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_org_unit_parent");
        builder.HasOne<Principal>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_org_unit_created_by");
        builder.HasOne<Principal>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_org_unit_updated_by");
    }
}
