using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class AccessScopeConfiguration : IEntityTypeConfiguration<AccessScope>
{
    public void Configure(EntityTypeBuilder<AccessScope> builder)
    {
        builder.ToTable("access_scope", "iam", table =>
        {
            table.HasCheckConstraint(
                "ck_access_scope_type",
                "scope_type IN ('SYSTEM', 'ORG_UNIT', 'PROGRAM', 'PROGRAM_VERSION', 'COHORT', 'CURRICULUM_PATH', 'COURSE', 'OFFERING', 'MEASUREMENT_PERIOD', 'SELF')");
            table.HasCheckConstraint(
                "ck_access_scope_anchor",
                "(scope_type = 'SYSTEM' AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'ORG_UNIT' AND org_unit_id IS NOT NULL AND num_nonnulls(program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'PROGRAM' AND program_id IS NOT NULL AND num_nonnulls(org_unit_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'PROGRAM_VERSION' AND program_version_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'COHORT' AND cohort_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'CURRICULUM_PATH' AND curriculum_path_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'COURSE' AND course_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'OFFERING' AND course_offering_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, measurement_period_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'MEASUREMENT_PERIOD' AND measurement_period_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, subject_principal_id) = 0) OR " +
                "(scope_type = 'SELF' AND subject_principal_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id) = 0)");
            table.HasCheckConstraint("ck_access_scope_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_access_scope");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.ScopeType).HasColumnName("scope_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.OrgUnitId).HasColumnName("org_unit_id").HasColumnType("uuid");
        builder.Property(entity => entity.ProgramId).HasColumnName("program_id").HasColumnType("uuid");
        builder.Property(entity => entity.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid");
        builder.Property(entity => entity.CohortId).HasColumnName("cohort_id").HasColumnType("uuid");
        builder.Property(entity => entity.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid");
        builder.Property(entity => entity.CourseId).HasColumnName("course_id").HasColumnType("uuid");
        builder.Property(entity => entity.CourseOfferingId).HasColumnName("course_offering_id").HasColumnType("uuid");
        builder.Property(entity => entity.MeasurementPeriodId).HasColumnName("measurement_period_id").HasColumnType("uuid");
        builder.Property(entity => entity.SubjectPrincipalId).HasColumnName("subject_principal_id").HasColumnType("uuid");
        builder.Property(entity => entity.IncludeDescendants).HasColumnName("include_descendants").HasColumnType("boolean").IsRequired();
        builder.Property(entity => entity.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_program_version");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_curriculum_path");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_course");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_course_offering");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_measurement_period");
        builder.HasOne(entity => entity.SubjectPrincipal).WithMany().HasForeignKey(entity => entity.SubjectPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_access_scope_subject_principal");

        builder.HasIndex(entity => new
        {
            entity.ScopeType,
            entity.OrgUnitId,
            entity.ProgramId,
            entity.ProgramVersionId,
            entity.CohortId,
            entity.CurriculumPathId,
            entity.CourseId,
            entity.CourseOfferingId,
            entity.MeasurementPeriodId,
            entity.SubjectPrincipalId,
            entity.IncludeDescendants
        }).IsUnique().AreNullsDistinct(false).HasDatabaseName("uq_access_scope_anchor");
    }
}
