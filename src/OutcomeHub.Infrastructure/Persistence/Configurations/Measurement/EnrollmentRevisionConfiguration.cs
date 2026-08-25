using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class EnrollmentRevisionConfiguration : IEntityTypeConfiguration<EnrollmentRevision>
{
    public void Configure(EntityTypeBuilder<EnrollmentRevision> builder)
    {
        builder.ToTable("enrollment_revision", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_enrollment_revision");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.EnrollmentId)
            .HasColumnName("enrollment_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RevisionNo)
            .HasColumnName("revision_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.EnrollmentStatus)
            .HasColumnName("enrollment_status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.RepeatFlag)
            .HasColumnName("repeat_flag")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.ImprovementFlag)
            .HasColumnName("improvement_flag")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.SourceUpdatedAt)
            .HasColumnName("source_updated_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.IngestionBatchId)
            .HasColumnName("ingestion_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SupersedesId)
            .HasColumnName("supersedes_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.RecordedAt)
            .HasColumnName("recorded_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.EnrollmentId, entity.Id })
            .IsUnique()
            .HasDatabaseName("uq_enrollment_revision_1");

        builder.HasIndex(entity => new { entity.EnrollmentId, entity.RevisionNo })
            .IsUnique()
            .HasDatabaseName("uq_enrollment_revision_2");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_enrollment_revision_no", "revision_no > 0");
            table.HasCheckConstraint("ck_enrollment_revision_status", "enrollment_status IN ('ENROLLED','COMPLETED','ABSENT','DEFERRED','WITHDRAWN','CANCELLED','RECOGNIZED')");
            table.HasCheckConstraint("ck_enrollment_revision_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_enrollment_revision_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_enrollment_revision_effective_range", "enrollment_id WITH =, tstzrange(effective_from, effective_to, '[)') WITH &&");
        builder.HasOne(entity => entity.Enrollment).WithMany(entity => entity.Revisions).HasForeignKey(entity => entity.EnrollmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_enrollment_revision_enrollment");
        builder.HasOne(entity => entity.IngestionBatch).WithMany().HasForeignKey(entity => entity.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_enrollment_revision_ingestion_batch");
        builder.HasOne(entity => entity.Supersedes).WithMany(entity => entity.Successors).HasForeignKey(entity => new { entity.EnrollmentId, entity.SupersedesId }).HasPrincipalKey(entity => new { entity.EnrollmentId, entity.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_enrollment_revision_supersedes");
    }
}
