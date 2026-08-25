using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusTraceabilityConfiguration : IEntityTypeConfiguration<SyllabusTraceability>
{
    public void Configure(EntityTypeBuilder<SyllabusTraceability> builder)
    {
        builder.ToTable("syllabus_traceability", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_traceability_data_role", "data_role IN ('DIRECT_PI','SUPPORT_PI','CLO_ONLY')");
                table.HasCheckConstraint("ck_syllabus_traceability_pi_binding", "(data_role = 'CLO_ONLY' AND course_pi_mapping_id IS NULL) OR (data_role IN ('DIRECT_PI','SUPPORT_PI') AND course_pi_mapping_id IS NOT NULL)");
                table.HasCheckConstraint("ck_syllabus_traceability_allocation", "allocation_ratio IS NULL OR (allocation_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND allocation_ratio > 0 AND allocation_ratio <= 1)");
                table.HasCheckConstraint("ck_syllabus_traceability_exception", "exception_decision_id IS NULL OR (allocation_ratio IS NOT NULL AND rationale IS NOT NULL AND char_length(btrim(rationale)) > 0)");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_traceability");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramCourseId).HasColumnName("program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CloId).HasColumnName("clo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CoursePiMappingId).HasColumnName("course_pi_mapping_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.RubricCriterionId).HasColumnName("rubric_criterion_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DataRole).HasColumnName("data_role").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.EvidenceRequirement).HasColumnName("evidence_requirement").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.AllocationRatio).HasColumnName("allocation_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired(false);
        builder.Property(x => x.ExceptionDecisionId).HasColumnName("exception_decision_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);

        builder.HasIndex(x => new { x.SyllabusVersionId, x.RubricCriterionId, x.CoursePiMappingId }).IsUnique().HasFilter("course_pi_mapping_id IS NOT NULL").HasDatabaseName("uq_syllabus_traceability_criterion_pi");
        builder.HasIndex(x => new { x.SyllabusVersionId, x.CloId }).HasDatabaseName("ix_syllabus_traceability_version_clo");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => new { x.SyllabusVersionId, x.ProgramCourseId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramCourseId, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_syllabus_binding");
        builder.HasOne(x => x.ProgramCourse).WithMany().HasForeignKey(x => new { x.ProgramCourseId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_program_course");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_program_version");
        builder.HasOne(x => x.Clo).WithMany().HasForeignKey(x => new { x.CloId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_clo_version");
        builder.HasOne(x => x.CoursePiMapping).WithMany().HasForeignKey(x => new { x.CoursePiMappingId, x.ProgramCourseId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramCourseId, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_course_pi_mapping");
        builder.HasOne(x => x.RubricCriterion).WithMany().HasForeignKey(x => new { x.RubricCriterionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_criterion_version");
        builder.HasOne(x => x.ExceptionDecision).WithMany().HasForeignKey(x => x.ExceptionDecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_traceability_exception_decision");
    }
}
