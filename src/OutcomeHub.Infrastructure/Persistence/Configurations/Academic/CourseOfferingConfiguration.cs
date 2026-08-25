using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CourseOfferingConfiguration : IEntityTypeConfiguration<CourseOffering>
{
    public void Configure(EntityTypeBuilder<CourseOffering> builder)
    {
        builder.ToTable("course_offering", "academic", table =>
        {
            table.HasCheckConstraint("ck_course_offering_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_course_offering_year", "academic_year_start BETWEEN 1900 AND 9999");
            table.HasCheckConstraint("ck_course_offering_dates", "end_date >= start_date");
            table.HasCheckConstraint("ck_course_offering_source", "(source_system_id IS NULL) = (source_record_id IS NULL)");
            table.HasCheckConstraint("ck_course_offering_status", "status IN ('PLANNED','OPEN','ACTIVE','COMPLETED','CANCELLED','ARCHIVED')");
        });
        builder.HasKey(x => x.Id).HasName("pk_course_offering");
        builder.HasAlternateKey(x => new { x.Id, x.AcademicYearStart }).HasName("uq_course_offering_id_year");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_course_offering_id_version");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId, x.AcademicYearStart }).HasName("uq_course_offering_id_version_year");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId, x.SyllabusVersionId, x.AcademicYearStart }).HasName("uq_course_offering_result_binding");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ProgramCourseId).HasColumnName("program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CourseVersionId).HasColumnName("course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AcademicYearStart).HasColumnName("academic_year_start").HasColumnType("smallint").IsRequired();
        builder.Property(x => x.TermCode).HasColumnName("term_code").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.OrgUnitId).HasColumnName("org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("date").IsRequired();
        builder.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("date").IsRequired();
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SourceRecordId).HasColumnName("source_record_id").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired(false);
        builder.HasIndex(x => new { x.SourceSystemId, x.SourceRecordId }).IsUnique().HasFilter("source_system_id IS NOT NULL AND source_record_id IS NOT NULL").HasDatabaseName("uq_course_offering_source_record");
        builder.HasIndex(x => new { x.SourceSystemId, x.AcademicYearStart, x.TermCode, x.Code }).IsUnique().HasFilter("source_system_id IS NOT NULL").HasDatabaseName("uq_course_offering_source_code");
        builder.HasIndex(x => new { x.AcademicYearStart, x.TermCode, x.Code }).IsUnique().HasFilter("source_system_id IS NULL").HasDatabaseName("uq_course_offering_manual_code");
        builder.HasOne(x => x.ProgramCourse).WithMany().HasForeignKey(x => new { x.ProgramCourseId, x.ProgramVersionId, x.CourseVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId, x.CourseVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_program_course_binding");
        builder.HasOne(x => x.CourseVersion).WithMany().HasForeignKey(x => x.CourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_course_version");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_program_version");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => new { x.SyllabusVersionId, x.ProgramCourseId, x.ProgramVersionId, x.CourseVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramCourseId, x.ProgramVersionId, x.CourseVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_syllabus_binding");
        builder.HasOne(x => x.OrgUnit).WithMany().HasForeignKey(x => x.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_org_unit");
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_source_system");
    }
}
