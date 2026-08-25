using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class ScoreRecordConfiguration : IEntityTypeConfiguration<ScoreRecord>
{
    public void Configure(EntityTypeBuilder<ScoreRecord> builder)
    {
        builder.ToTable("score_record", "measurement");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.Id })
            .HasName("pk_score_record");

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ScoreIdentityId)
            .HasColumnName("score_identity_id")
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

        builder.Property(entity => entity.OrgUnitId)
            .HasColumnName("org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramId)
            .HasColumnName("program_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseId)
            .HasColumnName("course_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RevisionNo)
            .HasColumnName("revision_no")
            .HasColumnType("integer")
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

        builder.Property(entity => entity.SourceSystemId)
            .HasColumnName("source_system_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceRecordId)
            .HasColumnName("source_record_id")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.SourceRevision)
            .HasColumnName("source_revision")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.IngestionBatchId)
            .HasColumnName("ingestion_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SupersedesId)
            .HasColumnName("supersedes_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.CorrectionReason)
            .HasColumnName("correction_reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.RecordedBy)
            .HasColumnName("recorded_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RecordedAt)
            .HasColumnName("recorded_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.ScoreIdentityId, entity.Id })
            .IsUnique()
            .HasDatabaseName("uq_score_record_1");

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.ScoreIdentityId, entity.RevisionNo })
            .IsUnique()
            .HasDatabaseName("uq_score_record_2");

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.SourceSystemId, entity.SourceRecordId, entity.SourceRevision })
            .IsUnique()
            .HasDatabaseName("uq_score_record_3");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_score_record_revision_no", "revision_no > 0");
            table.HasCheckConstraint("ck_score_record_max_score", "max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0");
            table.HasCheckConstraint("ck_score_record_status", "score_status IN ('SCORED','ABSENT','EXCUSED','NOT_SUBMITTED','DEFERRED','WITHDRAWN','MISSING')");
            table.HasCheckConstraint("ck_score_record_value_shape", "(score_status = 'SCORED' AND raw_score IS NOT NULL AND raw_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND raw_score >= 0 AND raw_score <= max_score) OR (score_status <> 'SCORED' AND raw_score IS NULL)");
            table.HasCheckConstraint("ck_score_record_correction", "supersedes_id IS NULL OR (correction_reason IS NOT NULL AND char_length(btrim(correction_reason)) > 0)");
            table.HasCheckConstraint("ck_score_record_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasOne(entity => entity.ScoreIdentity).WithMany(entity => entity.Records).HasForeignKey(entity => new { entity.AcademicYearStart, entity.ScoreIdentityId, entity.StudentId, entity.CourseOfferingId }).HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.StudentId, entity.CourseOfferingId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_identity_scope");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_student");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_course_offering");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_program_version");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_course");
        builder.HasOne(entity => entity.SourceSystem).WithMany().HasForeignKey(entity => entity.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_source_system");
        builder.HasOne(entity => entity.IngestionBatch).WithMany().HasForeignKey(entity => entity.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_ingestion_batch");
        builder.HasOne(entity => entity.Supersedes).WithMany(entity => entity.Successors).HasForeignKey(entity => new { entity.AcademicYearStart, entity.ScoreIdentityId, entity.SupersedesId }).HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.ScoreIdentityId, entity.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_supersedes");
        builder.HasOne(entity => entity.Recorder).WithMany().HasForeignKey(entity => entity.RecordedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_record_recorder");
    }
}
