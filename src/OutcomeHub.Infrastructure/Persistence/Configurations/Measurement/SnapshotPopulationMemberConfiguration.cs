using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotPopulationMemberConfiguration : IEntityTypeConfiguration<SnapshotPopulationMember>
{
    public void Configure(EntityTypeBuilder<SnapshotPopulationMember> builder)
    {
        builder.ToTable("snapshot_population_member", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.StudentId })
            .HasName("pk_snapshot_population_member");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CohortId)
            .HasColumnName("cohort_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPathId)
            .HasColumnName("student_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CurriculumPathId)
            .HasColumnName("curriculum_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Decision)
            .HasColumnName("decision")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ExclusionReasonCode)
            .HasColumnName("exclusion_reason_code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_population_member_decision", "decision IN ('PENDING','INCLUDED','EXCLUDED')");
            table.HasCheckConstraint("ck_snapshot_population_member_exclusion", "(decision = 'EXCLUDED' AND exclusion_reason_code IS NOT NULL) OR (decision <> 'EXCLUDED' AND exclusion_reason_code IS NULL)");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_population_member_input_snapshot");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_population_member_student");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_population_member_cohort");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => new { entity.StudentPathId, entity.StudentId, entity.CurriculumPathId }).HasPrincipalKey(entity => new { entity.Id, entity.StudentId, entity.CurriculumPathId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_population_member_student_path");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_population_member_curriculum_path");
    }
}
