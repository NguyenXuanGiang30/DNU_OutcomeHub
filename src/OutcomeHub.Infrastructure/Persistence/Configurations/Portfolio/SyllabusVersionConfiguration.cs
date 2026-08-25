using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusVersionConfiguration : IEntityTypeConfiguration<SyllabusVersion>
{
    public void Configure(EntityTypeBuilder<SyllabusVersion> builder)
    {
        builder.ToTable("syllabus_version", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_version_version_no", "version_no > 0");
                table.HasCheckConstraint("ck_syllabus_version_applicable_range", "applicable_to IS NULL OR applicable_to > applicable_from");
                table.HasCheckConstraint("ck_syllabus_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
                table.HasCheckConstraint("ck_syllabus_version_content_checksum", "content_checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusId).HasColumnName("syllabus_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramCourseId).HasColumnName("program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.InstitutionTemplateVersionId).HasColumnName("institution_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CourseVersionId).HasColumnName("course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusTemplateVersionId).HasColumnName("syllabus_template_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.ApplicableFrom).HasColumnName("applicable_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.ApplicableTo).HasColumnName("applicable_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.SharedSyllabusCoreVersionId).HasColumnName("shared_syllabus_core_version_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ContentChecksum).HasColumnName("content_checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.ProgramCourseId, x.ProgramVersionId }).HasName("uq_syllabus_version_id_course_program");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_syllabus_version_id_program_version");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramCourseId, x.CourseVersionId }).HasName("uq_syllabus_version_id_program_course_course_version");
        builder.HasAlternateKey(x => new { x.Id, x.SyllabusTemplateVersionId }).HasName("uq_syllabus_version_id_template");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramCourseId, x.ProgramVersionId, x.CourseVersionId }).HasName("uq_syllabus_version_full_binding");
        builder.HasIndex(x => new { x.SyllabusId, x.VersionNo }).IsUnique().HasDatabaseName("uq_syllabus_version_syllabus_no");
        builder.HasIndex(x => new { x.ProgramVersionId, x.ProgramCourseId, x.VersionNo }).IsUnique().HasDatabaseName("uq_syllabus_version_program_course_no");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasFilter("workflow_instance_id IS NOT NULL").HasDatabaseName("uq_syllabus_version_workflow");
        builder.Property(x => x.RowVersion).HasDefaultValue(1L).IsConcurrencyToken();
        builder.HasOne(x => x.Syllabus).WithMany(x => x.Versions).HasForeignKey(x => new { x.SyllabusId, x.ProgramCourseId }).HasPrincipalKey(x => new { x.Id, x.ProgramCourseId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_syllabus_program_course");
        builder.HasOne(x => x.ProgramCourse).WithMany().HasForeignKey(x => new { x.ProgramCourseId, x.ProgramVersionId, x.CourseVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId, x.CourseVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_program_course_binding");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => new { x.ProgramVersionId, x.InstitutionTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.InstitutionTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_program_template");
        builder.HasOne(x => x.InstitutionTemplateVersion).WithMany().HasForeignKey(x => x.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_institution_template_version");
        builder.HasOne(x => x.CourseVersion).WithMany().HasForeignKey(x => x.CourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_course_version");
        builder.HasOne(x => x.SyllabusTemplateVersion).WithMany().HasForeignKey(x => new { x.SyllabusTemplateVersionId, x.InstitutionTemplateVersionId }).HasPrincipalKey(x => new { x.Id, x.InstitutionTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_syllabus_template");
        builder.HasOne(x => x.SharedSyllabusCoreVersion).WithMany().HasForeignKey(x => new { x.SharedSyllabusCoreVersionId, x.CourseVersionId }).HasPrincipalKey(x => new { x.Id, x.CourseVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_shared_core");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_workflow");
        builder.HasOne(x => x.Supersedes).WithMany(x => x.Successors).HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_version_supersedes");
    }
}
