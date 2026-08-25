using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class PolicyPopulationRuleConfiguration : IEntityTypeConfiguration<PolicyPopulationRule>
{
    public void Configure(EntityTypeBuilder<PolicyPopulationRule> builder)
    {
        builder.ToTable("policy_population_rule", "measurement");

        builder.HasKey(entity => new { entity.PolicyVersionId, entity.EnrollmentStatus })
            .HasName("pk_policy_population_rule");

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EnrollmentStatus)
            .HasColumnName("enrollment_status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.DenominatorAction)
            .HasColumnName("denominator_action")
            .HasColumnType("varchar(40)")
            .HasMaxLength(40)
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_policy_population_rule_status", "enrollment_status IN ('ENROLLED','COMPLETED','ABSENT','DEFERRED','WITHDRAWN','CANCELLED','RECOGNIZED')"));
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_policy_population_rule_policy_version");
    }
}
