using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("student", "academic", table =>
        {
            table.HasCheckConstraint("ck_student_code", "student_code = upper(btrim(student_code)) AND char_length(student_code) > 0");
            table.HasCheckConstraint("ck_student_status", "current_status IN ('ACTIVE','SUSPENDED','GRADUATED','WITHDRAWN','EXPIRED')");
        });
        builder.HasKey(x => x.PersonId).HasName("pk_student");
        builder.HasAlternateKey(x => new { x.PersonId, x.AdmissionCohortId }).HasName("uq_student_person_cohort");
        builder.Property(x => x.PersonId).HasColumnName("person_id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.StudentCode).HasColumnName("student_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AdmissionCohortId).HasColumnName("admission_cohort_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurrentStatus).HasColumnName("current_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.StudentCode).IsUnique().HasDatabaseName("uq_student_code");
        builder.HasOne(x => x.Person).WithOne().HasForeignKey<Student>(x => x.PersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_person");
        builder.HasOne(x => x.AdmissionCohort).WithMany().HasForeignKey(x => x.AdmissionCohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_admission_cohort");
    }
}
