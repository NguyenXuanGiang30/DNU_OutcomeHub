using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CourseCrosswalkConfiguration : IEntityTypeConfiguration<CourseCrosswalk>
{
    public void Configure(EntityTypeBuilder<CourseCrosswalk> builder)
    {
        builder.ToTable("course_crosswalk", "academic", table =>
        {
            table.HasCheckConstraint("ck_course_crosswalk_relation", "relation_type IN ('EQUIVALENT','REPLACED_BY','SPLIT_TO','MERGED_INTO','NO_EQUIVALENT')");
            table.HasCheckConstraint("ck_course_crosswalk_target", "(relation_type = 'NO_EQUIVALENT' AND to_program_course_id IS NULL) OR (relation_type <> 'NO_EQUIVALENT' AND to_program_course_id IS NOT NULL)");
            table.HasCheckConstraint("ck_course_crosswalk_ratio", "allocation_ratio IS NULL OR (allocation_ratio >= 0 AND allocation_ratio <= 1 AND allocation_ratio <> 'NaN'::numeric AND allocation_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric))");
        });
        builder.HasKey(x => x.Id).HasName("pk_course_crosswalk");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionCrosswalkId).HasColumnName("program_version_crosswalk_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.FromProgramCourseId).HasColumnName("from_program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ToProgramCourseId).HasColumnName("to_program_course_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.RelationType).HasColumnName("relation_type").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.AllocationRatio).HasColumnName("allocation_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired(false);
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasIndex(x => new { x.ProgramVersionCrosswalkId, x.FromProgramCourseId, x.ToProgramCourseId, x.RelationType }).IsUnique().HasDatabaseName("uq_course_crosswalk_line");
        builder.HasOne(x => x.ProgramVersionCrosswalk).WithMany().HasForeignKey(x => x.ProgramVersionCrosswalkId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_crosswalk_header");
        builder.HasOne(x => x.FromProgramCourse).WithMany().HasForeignKey(x => x.FromProgramCourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_crosswalk_from");
        builder.HasOne(x => x.ToProgramCourse).WithMany().HasForeignKey(x => x.ToProgramCourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_crosswalk_to");
    }
}
