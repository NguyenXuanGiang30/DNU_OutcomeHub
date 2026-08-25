using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class PolicyThresholdConfiguration : IEntityTypeConfiguration<PolicyThreshold>
{
    public void Configure(EntityTypeBuilder<PolicyThreshold> builder)
    {
        builder.ToTable("policy_threshold", "measurement");

        builder.HasKey(entity => new { entity.PolicyVersionId, entity.OutcomeLevel })
            .HasName("pk_policy_threshold");

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeLevel)
            .HasColumnName("outcome_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ThetaInd)
            .HasColumnName("theta_ind")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.ThetaCoh)
            .HasColumnName("theta_coh")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.NearThreshold)
            .HasColumnName("near_threshold")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.MinSampleSize)
            .HasColumnName("min_sample_size")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_policy_threshold_level", "outcome_level IN ('CLO','PI','PLO')");
            table.HasCheckConstraint("ck_policy_threshold_values", "theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND min_sample_size > 0");
        });
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_policy_threshold_policy_version");
    }
}
