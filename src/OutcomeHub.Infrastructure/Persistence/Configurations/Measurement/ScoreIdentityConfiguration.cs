using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class ScoreIdentityConfiguration : IEntityTypeConfiguration<ScoreIdentity>
{
    public void Configure(EntityTypeBuilder<ScoreIdentity> builder)
    {
        builder.ToTable("score_identity", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_score_identity");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ScoreDatasetId)
            .HasColumnName("score_dataset_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SyllabusVersionId)
            .HasColumnName("syllabus_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AttemptNo)
            .HasColumnName("attempt_no")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.EnrollmentId)
            .HasColumnName("enrollment_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssessmentItemId)
            .HasColumnName("assessment_item_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RubricCriterionId)
            .HasColumnName("rubric_criterion_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.AssessmentQuestionId)
            .HasColumnName("assessment_question_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ScoreLevel)
            .HasColumnName("score_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.Id })
            .IsUnique()
            .HasDatabaseName("uq_score_identity_1");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint("ck_score_identity_shape", "(score_level = 'ASSESSMENT' AND rubric_criterion_id IS NULL AND assessment_question_id IS NULL) OR (score_level = 'CRITERION' AND rubric_criterion_id IS NOT NULL AND assessment_question_id IS NULL) OR (score_level = 'QUESTION' AND rubric_criterion_id IS NULL AND assessment_question_id IS NOT NULL)"));

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_score_identity_attempt_no", "attempt_no > 0");
            table.HasCheckConstraint("ck_score_identity_level", "score_level IN ('ASSESSMENT','CRITERION','QUESTION')");
        });
        builder.HasIndex(entity => new { entity.ScoreDatasetId, entity.StudentId, entity.AssessmentItemId, entity.RubricCriterionId, entity.AssessmentQuestionId, entity.AttemptNo }).IsUnique().AreNullsDistinct(false).HasDatabaseName("uq_score_identity_logical");
        builder.HasAlternateKey(entity => new { entity.AcademicYearStart, entity.Id, entity.StudentId, entity.CourseOfferingId }).HasName("uq_score_identity_scope");
        builder.HasOne(entity => entity.ScoreDataset).WithMany(entity => entity.ScoreIdentities).HasForeignKey(entity => new { entity.ScoreDatasetId, entity.CourseOfferingId, entity.AcademicYearStart }).HasPrincipalKey(entity => new { entity.Id, entity.CourseOfferingId, entity.AcademicYearStart }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_dataset_scope");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_student");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => new { entity.CourseOfferingId, entity.ProgramVersionId, entity.SyllabusVersionId, entity.AcademicYearStart }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId, entity.SyllabusVersionId, entity.AcademicYearStart }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_offering_binding");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_program_version");
        builder.HasOne(entity => entity.SyllabusVersion).WithMany().HasForeignKey(entity => entity.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_syllabus_version");
        builder.HasOne(entity => entity.Enrollment).WithMany().HasForeignKey(entity => new { entity.EnrollmentId, entity.StudentId, entity.CourseOfferingId, entity.AttemptNo }).HasPrincipalKey(entity => new { entity.Id, entity.StudentId, entity.CourseOfferingId, entity.AttemptNo }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_enrollment");
        builder.HasOne(entity => entity.AssessmentItem).WithMany().HasForeignKey(entity => new { entity.AssessmentItemId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_assessment");
        builder.HasOne(entity => entity.RubricCriterion).WithMany().HasForeignKey(entity => new { entity.RubricCriterionId, entity.AssessmentItemId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.AssessmentItemId, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_criterion");
        builder.HasOne(entity => entity.AssessmentQuestion).WithMany().HasForeignKey(entity => new { entity.AssessmentQuestionId, entity.AssessmentItemId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.AssessmentItemId, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_identity_question");
    }
}
