using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusConfiguration : IEntityTypeConfiguration<Syllabus>
{
    public void Configure(EntityTypeBuilder<Syllabus> builder)
    {
        builder.ToTable("syllabus", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_syllabus_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_syllabus");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ProgramCourseId).HasColumnName("program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.ProgramCourseId }).HasName("uq_syllabus_id_program_course");
        builder.HasIndex(x => x.ProgramCourseId).IsUnique().HasDatabaseName("uq_syllabus_program_course");
        builder.HasOne(x => x.ProgramCourse).WithMany().HasForeignKey(x => x.ProgramCourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_program_course");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_owner_org_unit");
    }
}

