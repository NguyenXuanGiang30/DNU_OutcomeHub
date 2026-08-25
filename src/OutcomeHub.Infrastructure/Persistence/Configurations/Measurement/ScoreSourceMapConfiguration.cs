using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class ScoreSourceMapConfiguration : IEntityTypeConfiguration<ScoreSourceMap>
{
    public void Configure(EntityTypeBuilder<ScoreSourceMap> builder)
    {
        builder.ToTable("score_source_map", "measurement");

        builder.HasKey(entity => new { entity.SourceSystemId, entity.SourceRecordId, entity.SourceRevision })
            .HasName("pk_score_source_map");

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

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.ScoreRecordId)
            .HasColumnName("score_record_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PayloadChecksum)
            .HasColumnName("payload_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_score_source_map_checksum", "payload_checksum ~ '^[0-9a-f]{64}$'"));
        builder.HasOne(entity => entity.SourceSystem).WithMany().HasForeignKey(entity => entity.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_source_map_source_system");
        builder.HasOne(entity => entity.ScoreRecord).WithMany().HasForeignKey(entity => new { entity.AcademicYearStart, entity.ScoreRecordId }).HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_score_source_map_score_record");
    }
}
