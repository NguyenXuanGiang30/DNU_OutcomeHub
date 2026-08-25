using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class StudentPathConfiguration : IEntityTypeConfiguration<StudentPath>
{
    public void Configure(EntityTypeBuilder<StudentPath> builder)
    {
        builder.ToTable("student_path", "academic", table =>
        {
            table.HasCheckConstraint("ck_student_path_status", "path_status IN ('ACTIVE','SUSPENDED','COMPLETED','TRANSFERRED','WITHDRAWN','EXPIRED')");
            table.HasCheckConstraint("ck_student_path_effective_range", "effective_to IS NULL OR effective_to > effective_from");
        });
        builder.HasKey(x => x.Id).HasName("pk_student_path");
        builder.HasAlternateKey(x => new { x.Id, x.StudentId }).HasName("uq_student_path_id_student");
        builder.HasAlternateKey(x => new { x.Id, x.StudentId, x.CurriculumPathId }).HasName("uq_student_path_id_student_curriculum_path");
        builder.HasAlternateKey(x => new { x.Id, x.StudentId, x.ProgramVersionId, x.CurriculumPathId }).HasName("uq_student_path_population_binding");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.StudentId).HasColumnName("student_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramId).HasColumnName("program_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PathStatus).HasColumnName("path_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").HasColumnType("boolean").IsRequired();
        builder.HasIndex(x => new { x.StudentId, x.ProgramId, x.IsPrimary, x.EffectiveFrom, x.EffectiveTo }).HasDatabaseName("ix_student_path_primary_period");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_student_path_primary_overlap", "student_id WITH =, program_id WITH =, daterange(effective_from, effective_to, '[)') WITH && WHERE (is_primary AND path_status = 'ACTIVE')");
        builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_path_student");
        builder.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_path_program");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => new { x.ProgramVersionId, x.ProgramId }).HasPrincipalKey(x => new { x.Id, x.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_path_program_version_program");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => new { x.CurriculumPathId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_path_curriculum_path_version");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_path_decision");
    }
}
