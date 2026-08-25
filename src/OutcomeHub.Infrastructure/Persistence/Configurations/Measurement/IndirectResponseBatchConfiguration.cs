using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class IndirectResponseBatchConfiguration : IEntityTypeConfiguration<IndirectResponseBatch>
{
    public void Configure(EntityTypeBuilder<IndirectResponseBatch> builder)
    {
        builder.ToTable("indirect_response_batch", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_indirect_response_batch");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.InstrumentVersionId)
            .HasColumnName("instrument_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.Id, entity.InstrumentVersionId, entity.ProgramVersionId })
            .IsUnique()
            .HasDatabaseName("uq_indirect_response_batch_1");

        builder.ToTable(table => table.HasCheckConstraint("ck_indirect_response_batch_checksum", "checksum ~ '^[0-9a-f]{64}$'"));
        builder.HasOne(entity => entity.InstrumentVersion).WithMany().HasForeignKey(entity => entity.InstrumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_response_batch_instrument_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_response_batch_period_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_response_batch_program_version");
    }
}
