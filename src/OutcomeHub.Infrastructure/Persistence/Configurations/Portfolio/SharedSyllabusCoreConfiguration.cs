using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SharedSyllabusCoreConfiguration : IEntityTypeConfiguration<SharedSyllabusCore>
{
    public void Configure(EntityTypeBuilder<SharedSyllabusCore> builder)
    {
        builder.ToTable("shared_syllabus_core", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_shared_syllabus_core_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_shared_syllabus_core");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();

        builder.HasIndex(x => new { x.CourseId, x.Code }).IsUnique().HasDatabaseName("uq_shared_syllabus_core_course_code");
        builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_course");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_shared_syllabus_core_owner_org_unit");
    }
}

