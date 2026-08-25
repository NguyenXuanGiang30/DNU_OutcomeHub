using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class ProgramPolicyThresholdConfiguration : IEntityTypeConfiguration<ProgramPolicyThreshold>
{
    public void Configure(EntityTypeBuilder<ProgramPolicyThreshold> builder)
    {
        builder.ToTable("program_policy_threshold", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_program_policy_threshold");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPolicyBindingId)
            .HasColumnName("program_policy_binding_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeLevel)
            .HasColumnName("outcome_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.SyllabusVersionId)
            .HasColumnName("syllabus_version_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.CloId)
            .HasColumnName("clo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ThetaInd)
            .HasColumnName("theta_ind")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.ThetaCoh)
            .HasColumnName("theta_coh")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.NearThreshold)
            .HasColumnName("near_threshold")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.MinSampleSize)
            .HasColumnName("min_sample_size")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.ProgramPolicyBindingId, entity.OutcomeLevel, entity.SyllabusVersionId, entity.CloId, entity.ProgramPiId, entity.ProgramPloId })
            .IsUnique()
            .AreNullsDistinct(false)
            .HasDatabaseName("uq_program_policy_threshold_1");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint("ck_program_policy_threshold_outcome", "num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1"));

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_program_policy_threshold_level", "outcome_level IN ('CLO','PI','PLO')");
            table.HasCheckConstraint("ck_program_policy_threshold_shape", "(outcome_level = 'CLO' AND clo_id IS NOT NULL AND syllabus_version_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND syllabus_version_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND syllabus_version_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)");
            table.HasCheckConstraint("ck_program_policy_threshold_values", "theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND min_sample_size > 0");
        });
        builder.HasOne(entity => entity.ProgramPolicyBinding).WithMany().HasForeignKey(entity => entity.ProgramPolicyBindingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_threshold_binding");
        builder.HasOne(entity => entity.SyllabusVersion).WithMany().HasForeignKey(entity => entity.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_threshold_syllabus_version");
        builder.HasOne(entity => entity.Clo).WithMany().HasForeignKey(entity => new { entity.CloId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_threshold_clo_syllabus");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_threshold_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_threshold_program_plo");
    }
}
