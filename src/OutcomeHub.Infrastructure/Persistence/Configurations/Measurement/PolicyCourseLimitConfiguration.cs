using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class PolicyCourseLimitConfiguration : IEntityTypeConfiguration<PolicyCourseLimit>
{
    public void Configure(EntityTypeBuilder<PolicyCourseLimit> builder)
    {
        builder.ToTable("policy_course_limit", "measurement");

        builder.HasKey(entity => new { entity.PolicyVersionId, entity.CourseType })
            .HasName("pk_policy_course_limit");

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseType)
            .HasColumnName("course_type")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.MaxMCount)
            .HasColumnName("max_m_count")
            .HasColumnType("integer")
            .IsRequired(false);

        builder.Property(entity => entity.MaxDirectPiCount)
            .HasColumnName("max_direct_pi_count")
            .HasColumnType("integer")
            .IsRequired(false);

        builder.Property(entity => entity.ExceptionRequired)
            .HasColumnName("exception_required")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_policy_course_limit_counts", "(max_m_count IS NULL OR max_m_count >= 0) AND (max_direct_pi_count IS NULL OR max_direct_pi_count >= 0)"));
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_policy_course_limit_policy_version");
    }
}
