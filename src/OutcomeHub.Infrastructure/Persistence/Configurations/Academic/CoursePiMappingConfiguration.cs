using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CoursePiMappingConfiguration : IEntityTypeConfiguration<CoursePiMapping>
{
    public void Configure(EntityTypeBuilder<CoursePiMapping> builder)
    {
        builder.ToTable("course_pi_mapping", "academic", table =>
        {
            table.HasCheckConstraint("ck_course_pi_mapping_contribution", "contribution_level IN ('I','R','M')");
            table.HasCheckConstraint("ck_course_pi_mapping_source", "source_type IN ('TEMPLATE','PROGRAM','APPENDIX')");
            table.HasCheckConstraint("ck_course_pi_mapping_source_lock", "source_shared_mapping_id IS NULL OR is_locked");
            table.HasCheckConstraint("ck_course_pi_mapping_exception", "source_type <> 'APPENDIX' OR exception_decision_id IS NOT NULL");
        });
        builder.HasKey(x => x.Id).HasName("pk_course_pi_mapping");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_course_pi_mapping_id_version");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramCourseId, x.ProgramVersionId }).HasName("uq_course_pi_mapping_id_course_version");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId, x.ProgramPiId }).HasName("uq_course_pi_mapping_id_version_pi");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramCourseId).HasColumnName("program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramPiId).HasColumnName("program_pi_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ContributionLevel).HasColumnName("contribution_level").HasColumnType("char(1)").HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.IsDirectAssessment).HasColumnName("is_direct_assessment").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.SourceType).HasColumnName("source_type").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.SourceSharedMappingId).HasColumnName("source_shared_mapping_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.IsLocked).HasColumnName("is_locked").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.ExceptionDecisionId).HasColumnName("exception_decision_id").HasColumnType("uuid").IsRequired(false);
        builder.HasIndex(x => new { x.ProgramVersionId, x.ProgramCourseId, x.ProgramPiId }).IsUnique().HasDatabaseName("uq_course_pi_mapping_version_course_pi");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_mapping_version");
        builder.HasOne(x => x.ProgramCourse).WithMany().HasForeignKey(x => new { x.ProgramCourseId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_mapping_course_version");
        builder.HasOne(x => x.ProgramPi).WithMany().HasForeignKey(x => new { x.ProgramPiId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_mapping_pi_version");
        builder.HasOne(x => x.SourceSharedMapping).WithMany().HasForeignKey(x => x.SourceSharedMappingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_mapping_shared_source");
        builder.HasOne(x => x.ExceptionDecision).WithMany().HasForeignKey(x => x.ExceptionDecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_mapping_exception_decision");
    }
}
