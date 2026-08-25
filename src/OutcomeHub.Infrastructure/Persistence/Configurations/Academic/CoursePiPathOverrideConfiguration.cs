using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CoursePiPathOverrideConfiguration : IEntityTypeConfiguration<CoursePiPathOverride>
{
    public void Configure(EntityTypeBuilder<CoursePiPathOverride> builder)
    {
        builder.ToTable("course_pi_path_override", "academic", table =>
            table.HasCheckConstraint("ck_course_pi_path_override_contribution", "contribution_level IN ('I','R','M')"));
        builder.HasKey(x => x.Id).HasName("pk_course_pi_path_override");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CoursePiMappingId).HasColumnName("course_pi_mapping_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ContributionLevel).HasColumnName("contribution_level").HasColumnType("char(1)").HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.DirectAssessmentEnabled).HasColumnName("direct_assessment_enabled").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.ExceptionDecisionId).HasColumnName("exception_decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired();
        builder.HasIndex(x => new { x.CoursePiMappingId, x.CurriculumPathId }).IsUnique().HasDatabaseName("uq_course_pi_path_override_mapping_path");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_path_override_version");
        builder.HasOne(x => x.CoursePiMapping).WithMany().HasForeignKey(x => new { x.CoursePiMappingId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_path_override_mapping_version");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => new { x.CurriculumPathId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_path_override_path_version");
        builder.HasOne(x => x.ExceptionDecision).WithMany().HasForeignKey(x => x.ExceptionDecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_path_override_decision");
    }
}
