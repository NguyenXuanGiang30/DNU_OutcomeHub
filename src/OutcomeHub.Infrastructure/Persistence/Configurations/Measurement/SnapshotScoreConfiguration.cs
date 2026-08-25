using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotScoreConfiguration : IEntityTypeConfiguration<SnapshotScore>
{
    public void Configure(EntityTypeBuilder<SnapshotScore> builder)
    {
        builder.ToTable("snapshot_score", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.AcademicYearStart, entity.ScoreRecordId })
            .HasName("pk_snapshot_score");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.ScoreRecordId)
            .HasColumnName("score_record_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RawScore)
            .HasColumnName("raw_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.MaxScore)
            .HasColumnName("max_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.ScoreStatus)
            .HasColumnName("score_status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.NormalizedScore)
            .HasColumnName("normalized_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasAlternateKey(entity => new { entity.InputSnapshotId, entity.AcademicYearStart, entity.ScoreRecordId, entity.StudentId, entity.CourseOfferingId })
            .HasName("uq_snapshot_score_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_score_status", "score_status IN ('SCORED','ABSENT','EXCUSED','NOT_SUBMITTED','DEFERRED','WITHDRAWN','MISSING')");
            table.HasCheckConstraint("ck_snapshot_score_values", "max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0 AND ((score_status = 'SCORED' AND raw_score IS NOT NULL AND raw_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND raw_score >= 0 AND raw_score <= max_score AND normalized_score IS NOT NULL AND normalized_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_score BETWEEN 0 AND 100) OR (score_status <> 'SCORED' AND raw_score IS NULL AND normalized_score IS NULL))");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_score_input_snapshot");
        builder.HasOne(entity => entity.ScoreRecord).WithMany().HasForeignKey(entity => new { entity.AcademicYearStart, entity.ScoreRecordId, entity.StudentId, entity.CourseOfferingId }).HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.StudentId, entity.CourseOfferingId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_score_record_scope");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_score_student");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_score_course_offering");
    }
}
