using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class InputSnapshotConfiguration : IEntityTypeConfiguration<InputSnapshot>
{
    public void Configure(EntityTypeBuilder<InputSnapshot> builder)
    {
        builder.ToTable("input_snapshot", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_input_snapshot");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId)
            .HasColumnName("org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SnapshotNo)
            .HasColumnName("snapshot_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPolicyBindingId)
            .HasColumnName("program_policy_binding_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.InstitutionTemplateVersionId)
            .HasColumnName("institution_template_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.SchemaVersion)
            .HasColumnName("schema_version")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.HashAlgorithm)
            .HasColumnName("hash_algorithm")
            .HasColumnType("varchar(16)")
            .HasMaxLength(16)
            .IsRequired(true);

        builder.Property(entity => entity.ManifestChecksum)
            .HasColumnName("manifest_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.PopulationCount)
            .HasColumnName("population_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.ScoreCount)
            .HasColumnName("score_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.ParentSnapshotId)
            .HasColumnName("parent_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.SealedBy)
            .HasColumnName("sealed_by")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.SealedAt)
            .HasColumnName("sealed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_input_snapshot_1");

        builder.HasIndex(entity => new { entity.MeasurementPeriodId, entity.SnapshotNo })
            .IsUnique()
            .HasDatabaseName("uq_input_snapshot_2");

        builder.HasAlternateKey(entity => new { entity.Id, entity.MeasurementPeriodId })
            .HasName("uq_input_snapshot_3");

        builder.HasAlternateKey(entity => new { entity.Id, entity.MeasurementPeriodId, entity.PolicyVersionId, entity.ProgramPolicyBindingId, entity.OrgUnitId, entity.ProgramVersionId, entity.AcademicYearStart })
            .HasName("uq_input_snapshot_4");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_input_snapshot_no", "snapshot_no > 0");
            table.HasCheckConstraint("ck_input_snapshot_status", "status IN ('BUILDING','SEALED','VOID')");
            table.HasCheckConstraint("ck_input_snapshot_hash_algorithm", "hash_algorithm = 'SHA-256'");
            table.HasCheckConstraint("ck_input_snapshot_manifest", "(status <> 'SEALED' OR (manifest_checksum IS NOT NULL AND manifest_checksum ~ '^[0-9a-f]{64}$' AND sealed_by IS NOT NULL AND sealed_at IS NOT NULL)) AND (manifest_checksum IS NULL OR manifest_checksum ~ '^[0-9a-f]{64}$') AND ((sealed_by IS NULL) = (sealed_at IS NULL))");
            table.HasCheckConstraint("ck_input_snapshot_counts", "population_count >= 0 AND score_count >= 0");
        });
        builder.Property(entity => entity.HashAlgorithm).HasDefaultValue("SHA-256");
        builder.HasOne(entity => entity.GovernedResource).WithMany().HasForeignKey(entity => entity.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_governed_resource");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.OrgUnitId, entity.ProgramVersionId, entity.AcademicYearStart }).HasPrincipalKey(entity => new { entity.Id, entity.OrgUnitId, entity.ProgramVersionId, entity.AcademicYearStart }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_period_scope");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_org_unit");
        builder.HasOne(entity => entity.ProgramPolicyBinding).WithMany().HasForeignKey(entity => new { entity.ProgramPolicyBindingId, entity.ProgramVersionId, entity.PolicyVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId, entity.PolicyVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_policy_binding");
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_policy_version");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.InstitutionTemplateVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.InstitutionTemplateVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_program_template");
        builder.HasOne(entity => entity.InstitutionTemplateVersion).WithMany().HasForeignKey(entity => entity.InstitutionTemplateVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_institution_template");
        builder.HasOne(entity => entity.ParentSnapshot).WithMany().HasForeignKey(entity => new { entity.ParentSnapshotId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { entity.Id, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_parent_period");
        builder.HasOne(entity => entity.Creator).WithMany().HasForeignKey(entity => entity.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_creator");
        builder.HasOne(entity => entity.Sealer).WithMany().HasForeignKey(entity => entity.SealedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_input_snapshot_sealer");
    }
}
