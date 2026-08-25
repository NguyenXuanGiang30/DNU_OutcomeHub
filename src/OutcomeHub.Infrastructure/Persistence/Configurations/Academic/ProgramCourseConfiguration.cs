using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramCourseConfiguration : IEntityTypeConfiguration<ProgramCourse>
{
    public void Configure(EntityTypeBuilder<ProgramCourse> builder)
    {
        builder.ToTable("program_course", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_course_catalog_role", "catalog_role IN ('REQUIRED','ELECTIVE','ORIENTATION','GRADUATION')");
            table.HasCheckConstraint("ck_program_course_credit_override", "credit_override IS NULL OR (credit_override > 0 AND credit_override <> 'NaN'::numeric AND credit_override NOT IN ('Infinity'::numeric, '-Infinity'::numeric))");
            table.HasCheckConstraint("ck_program_course_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_course");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_program_course_id_version");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId, x.CourseVersionId }).HasName("uq_program_course_id_version_course_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CourseVersionId).HasColumnName("course_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurriculumBlockId).HasColumnName("curriculum_block_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CatalogRole).HasColumnName("catalog_role").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreditOverride).HasColumnName("credit_override").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired(false);
        builder.Property(x => x.IsLocked).HasColumnName("is_locked").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.CourseVersionId }).IsUnique().HasDatabaseName("uq_program_course_version_course_version");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_course_program_version");
        builder.HasOne(x => x.CourseVersion).WithMany().HasForeignKey(x => x.CourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_course_course_version");
        builder.HasOne(x => x.CurriculumBlock).WithMany().HasForeignKey(x => x.CurriculumBlockId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_course_curriculum_block");
    }
}
