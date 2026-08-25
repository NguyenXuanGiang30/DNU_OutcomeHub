using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class QuarantineCorrectionConfiguration : IEntityTypeConfiguration<QuarantineCorrection>
{
    public void Configure(EntityTypeBuilder<QuarantineCorrection> builder)
    {
        // Append-only mutation protection is installed by operational migration SQL.
        builder.ToTable("quarantine_correction", "integration", table =>
        {
            table.HasCheckConstraint("ck_quarantine_correction_revision", "revision_no > 0");
            table.HasCheckConstraint("ck_quarantine_correction_reason", "char_length(btrim(reason)) > 0");
            table.HasCheckConstraint("ck_quarantine_correction_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_quarantine_correction");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.QuarantineRecordId).HasColumnName("quarantine_record_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RevisionNo).HasColumnName("revision_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.NormalizedPayload).HasColumnName("normalized_payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text").IsRequired();
        builder.Property(x => x.CorrectedBy).HasColumnName("corrected_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CorrectedAt).HasColumnName("corrected_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasOne(x => x.QuarantineRecord).WithMany(x => x.Corrections).HasForeignKey(x => x.QuarantineRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_correction_record");
        builder.HasOne(x => x.CorrectedByPrincipal).WithMany().HasForeignKey(x => x.CorrectedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_quarantine_correction_corrected_by");
        builder.HasIndex(x => new { x.QuarantineRecordId, x.RevisionNo }).IsUnique().HasDatabaseName("uq_quarantine_correction_record_revision");
    }
}
