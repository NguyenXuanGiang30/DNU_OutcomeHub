using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class ModelDeploymentConfiguration : IEntityTypeConfiguration<ModelDeployment>
{
    public void Configure(EntityTypeBuilder<ModelDeployment> builder)
    {
        builder.ToTable("model_deployment", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_model_deployment");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.OwnerOrgUnitId)
            .HasColumnName("owner_org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.HasIndex(entity => entity.Code)
            .IsUnique()
            .HasDatabaseName("uq_model_deployment_code");

        builder.HasIndex(entity => entity.OwnerOrgUnitId)
            .HasDatabaseName("ix_model_deployment_owner_org_unit");

        builder.HasOne(entity => entity.OwnerOrgUnit)
            .WithMany()
            .HasForeignKey(entity => entity.OwnerOrgUnitId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_model_deployment_owner_org_unit");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint(
            "ck_model_deployment_code",
            "code = upper(btrim(code)) AND char_length(code) > 0"));
    }
}
