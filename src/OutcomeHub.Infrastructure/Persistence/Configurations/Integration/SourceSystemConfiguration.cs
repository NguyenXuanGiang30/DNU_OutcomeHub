using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class SourceSystemConfiguration : IEntityTypeConfiguration<SourceSystem>
{
    public void Configure(EntityTypeBuilder<SourceSystem> builder)
    {
        builder.ToTable("source_system", "integration", table =>
        {
            table.HasCheckConstraint("ck_source_system_code", "code = btrim(code) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_source_system_status", "status IN ('ACTIVE', 'DISABLED')");
            table.HasCheckConstraint("ck_source_system_classification", "data_classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')");
        });
        builder.HasKey(x => x.Id).HasName("pk_source_system");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.SystemType).HasColumnName("system_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.BaseUrl).HasColumnName("base_url").HasColumnType("varchar(2048)").HasMaxLength(2048);
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ServicePrincipalId).HasColumnName("service_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DataClassification).HasColumnName("data_classification").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_source_system_owner_org_unit");
        builder.HasOne(x => x.ServiceAccount).WithMany().HasForeignKey(x => x.ServicePrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_source_system_service_account");
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("uq_source_system_code");
    }
}
