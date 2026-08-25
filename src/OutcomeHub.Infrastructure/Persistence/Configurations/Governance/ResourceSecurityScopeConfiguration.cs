using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class ResourceSecurityScopeConfiguration : IEntityTypeConfiguration<ResourceSecurityScope>
{
    public void Configure(EntityTypeBuilder<ResourceSecurityScope> builder)
    {
        builder.ToTable("resource_security_scope", "governance", table =>
            {
                table.HasCheckConstraint("ck_resource_security_scope_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
                table.HasCheckConstraint("ck_resource_security_scope_checksum", "derivation_checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_resource_security_scope");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.OrgUnitId).HasColumnName("org_unit_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ProgramId).HasColumnName("program_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.CohortId).HasColumnName("cohort_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.CourseOfferingId).HasColumnName("course_offering_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.MeasurementPeriodId).HasColumnName("measurement_period_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.StudentId).HasColumnName("student_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DerivationChecksum).HasColumnName("derivation_checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.GovernedResourceId, x.OrgUnitId, x.ProgramId, x.ProgramVersionId, x.CohortId, x.CurriculumPathId, x.CourseId, x.CourseOfferingId, x.MeasurementPeriodId, x.StudentId, x.Classification }).IsUnique().HasDatabaseName("uq_resource_security_scope_dimensions");
        builder.HasIndex(x => new { x.OrgUnitId, x.ProgramVersionId, x.CourseOfferingId, x.StudentId }).HasDatabaseName("ix_resource_security_scope_authorization");
        builder.HasOne(x => x.GovernedResource).WithMany(x => x.SecurityScopes).HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_resource");
        builder.HasOne(x => x.OrgUnit).WithMany().HasForeignKey(x => x.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_org_unit");
        builder.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_program");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_program_version");
        builder.HasOne(x => x.Cohort).WithMany().HasForeignKey(x => x.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_cohort");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => x.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_curriculum_path");
        builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_course");
        builder.HasOne(x => x.CourseOffering).WithMany().HasForeignKey(x => x.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_course_offering");
        builder.HasOne(x => x.MeasurementPeriod).WithMany().HasForeignKey(x => x.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_measurement_period");
        builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_security_scope_student");
    }
}

