using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class GraderAssignmentConfiguration : IEntityTypeConfiguration<GraderAssignment>
{
    public void Configure(EntityTypeBuilder<GraderAssignment> builder)
    {
        builder.ToTable("grader_assignment", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_grader_assignment");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SyllabusVersionId)
            .HasColumnName("syllabus_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssessmentItemId)
            .HasColumnName("assessment_item_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RubricCriterionId)
            .HasColumnName("rubric_criterion_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PrincipalId)
            .HasColumnName("principal_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssignmentRole)
            .HasColumnName("assignment_role")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.AssignedBy)
            .HasColumnName("assigned_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint("ck_grader_assignment_assignment_role", "assignment_role IN ('SCORER', 'CHECKER', 'APPROVER')"));

        builder.ToTable(table => table.HasCheckConstraint("ck_grader_assignment_effective_range", "effective_to IS NULL OR effective_to > effective_from"));
        builder.HasIndex(entity => new { entity.MeasurementPeriodId, entity.CourseOfferingId, entity.AssessmentItemId, entity.RubricCriterionId, entity.PrincipalId, entity.AssignmentRole, entity.EffectiveFrom }).IsUnique().HasDatabaseName("uq_grader_assignment_scope");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_period");
        builder.HasOne(entity => entity.PeriodOffering).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.CourseOfferingId }).HasPrincipalKey(entity => new { entity.MeasurementPeriodId, entity.CourseOfferingId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_period_offering");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_course_offering");
        builder.HasOne(entity => entity.SyllabusVersion).WithMany().HasForeignKey(entity => entity.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_syllabus_version");
        builder.HasOne(entity => entity.AssessmentItem).WithMany().HasForeignKey(entity => new { entity.AssessmentItemId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_assessment");
        builder.HasOne(entity => entity.RubricCriterion).WithMany().HasForeignKey(entity => new { entity.RubricCriterionId, entity.AssessmentItemId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.AssessmentItemId, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_criterion");
        builder.HasOne(entity => entity.Principal).WithMany().HasForeignKey(entity => entity.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_principal");
        builder.HasOne(entity => entity.Assigner).WithMany().HasForeignKey(entity => entity.AssignedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_grader_assignment_assigned_by");
    }
}
