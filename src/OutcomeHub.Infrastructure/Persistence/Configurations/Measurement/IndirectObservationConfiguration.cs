using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class IndirectObservationConfiguration : IEntityTypeConfiguration<IndirectObservation>
{
    public void Configure(EntityTypeBuilder<IndirectObservation> builder)
    {
        builder.ToTable("indirect_observation", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_indirect_observation");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ResponseBatchId)
            .HasColumnName("response_batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.InstrumentVersionId)
            .HasColumnName("instrument_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ItemId)
            .HasColumnName("item_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RespondentKey)
            .HasColumnName("respondent_key")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.RawValue)
            .HasColumnName("raw_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.MaxValue)
            .HasColumnName("max_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.GroupDimension)
            .HasColumnName("group_dimension")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(entity => entity.RecordedAt)
            .HasColumnName("recorded_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_indirect_observation_value", "raw_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value > 0 AND raw_value >= 0 AND raw_value <= max_value");
            table.HasCheckConstraint("ck_indirect_observation_respondent", "char_length(btrim(respondent_key)) > 0");
        });
        builder.HasIndex(entity => new { entity.ResponseBatchId, entity.ItemId, entity.RespondentKey }).IsUnique().HasDatabaseName("uq_indirect_observation_response");
        builder.HasOne(entity => entity.ResponseBatch).WithMany(entity => entity.Observations).HasForeignKey(entity => new { entity.ResponseBatchId, entity.InstrumentVersionId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.InstrumentVersionId, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_observation_batch_binding");
        builder.HasOne(entity => entity.InstrumentVersion).WithMany().HasForeignKey(entity => entity.InstrumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_observation_instrument_version");
        builder.HasOne(entity => entity.Item).WithMany().HasForeignKey(entity => new { entity.ItemId, entity.InstrumentVersionId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.InstrumentVersionId, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_observation_item_binding");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_observation_student");
    }
}
