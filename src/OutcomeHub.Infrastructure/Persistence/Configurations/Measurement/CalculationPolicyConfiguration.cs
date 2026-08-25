using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class CalculationPolicyConfiguration : IEntityTypeConfiguration<CalculationPolicy>
{
    public void Configure(EntityTypeBuilder<CalculationPolicy> builder)
    {
        builder.ToTable("calculation_policy", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_calculation_policy");

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

        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => entity.Code)
            .IsUnique()
            .HasDatabaseName("uq_calculation_policy_1");

        builder.ToTable(table => table.HasCheckConstraint("ck_calculation_policy_code", "code = upper(btrim(code)) AND char_length(code) > 0"));
        builder.HasOne(entity => entity.OwnerOrgUnit).WithMany().HasForeignKey(entity => entity.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_calculation_policy_owner_org_unit");
    }
}
