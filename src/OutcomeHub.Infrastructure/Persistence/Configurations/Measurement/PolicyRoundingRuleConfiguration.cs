using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class PolicyRoundingRuleConfiguration : IEntityTypeConfiguration<PolicyRoundingRule>
{
    public void Configure(EntityTypeBuilder<PolicyRoundingRule> builder)
    {
        builder.ToTable("policy_rounding_rule", "measurement");

        builder.HasKey(entity => new { entity.PolicyVersionId, entity.ResultLevel })
            .HasName("pk_policy_rounding_rule");

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ResultLevel)
            .HasColumnName("result_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.Scale)
            .HasColumnName("scale")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.RoundingMode)
            .HasColumnName("rounding_mode")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_policy_rounding_rule_scale", "scale BETWEEN 0 AND 10"));
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_policy_rounding_rule_policy_version");
    }
}
