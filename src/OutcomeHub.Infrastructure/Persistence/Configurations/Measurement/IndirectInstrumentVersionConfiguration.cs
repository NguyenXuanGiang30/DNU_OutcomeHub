using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class IndirectInstrumentVersionConfiguration : IEntityTypeConfiguration<IndirectInstrumentVersion>
{
    public void Configure(EntityTypeBuilder<IndirectInstrumentVersion> builder)
    {
        builder.ToTable("indirect_instrument_version", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_indirect_instrument_version");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.InstrumentId)
            .HasColumnName("instrument_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.ScaleMin)
            .HasColumnName("scale_min")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.ScaleMax)
            .HasColumnName("scale_max")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.InstrumentId, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_indirect_instrument_version_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_indirect_instrument_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_indirect_instrument_version_scale", "scale_min NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND scale_max NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND scale_min < scale_max");
            table.HasCheckConstraint("ck_indirect_instrument_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_indirect_instrument_version_workflow");
        builder.HasOne(entity => entity.Instrument).WithMany(entity => entity.Versions).HasForeignKey(entity => entity.InstrumentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_instrument_version_instrument");
        builder.HasOne(entity => entity.WorkflowInstance).WithMany().HasForeignKey(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_instrument_version_workflow");
    }
}
