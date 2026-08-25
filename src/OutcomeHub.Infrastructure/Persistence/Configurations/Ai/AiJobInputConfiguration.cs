using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiJobInputConfiguration : IEntityTypeConfiguration<AiJobInput>
{
    public void Configure(EntityTypeBuilder<AiJobInput> builder)
    {
        builder.ToTable("ai_job_input", "ai");

        builder.HasKey(entity => new { entity.AiJobId, entity.SequenceNo })
            .HasName("pk_ai_job_input");

        builder.Property(entity => entity.AiJobId)
            .HasColumnName("ai_job_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SequenceNo)
            .HasColumnName("sequence_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.SourceSnapshotId)
            .HasColumnName("source_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.InputRole)
            .HasColumnName("input_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.SourceChecksum)
            .HasColumnName("source_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => entity.SourceSnapshotId)
            .HasDatabaseName("ix_ai_job_input_source_snapshot");

        builder.HasOne(entity => entity.AiJob)
            .WithMany()
            .HasForeignKey(entity => entity.AiJobId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_input_job");

        builder.HasOne(entity => entity.SourceSnapshot)
            .WithMany()
            .HasForeignKey(entity => new { entity.SourceSnapshotId, entity.SourceChecksum })
            .HasPrincipalKey(entity => new { entity.Id, entity.SourceChecksum })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_input_source_snapshot_checksum");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ai_job_input_sequence", "sequence_no > 0");
            tableBuilder.HasCheckConstraint("ck_ai_job_input_role", "input_role = upper(btrim(input_role)) AND char_length(input_role) > 0");
            tableBuilder.HasCheckConstraint("ck_ai_job_input_checksum", "source_checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
