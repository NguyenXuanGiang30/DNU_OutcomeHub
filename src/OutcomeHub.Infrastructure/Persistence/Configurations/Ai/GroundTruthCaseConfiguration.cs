using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class GroundTruthCaseConfiguration : IEntityTypeConfiguration<GroundTruthCase>
{
    public void Configure(EntityTypeBuilder<GroundTruthCase> builder)
    {
        builder.ToTable("ground_truth_case", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ground_truth_case");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.SuiteVersionId)
            .HasColumnName("suite_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CaseCode)
            .HasColumnName("case_code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.InputSourceSnapshotId)
            .HasColumnName("input_source_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ExpectedOutput)
            .HasColumnName("expected_output")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.AcceptanceRule)
            .HasColumnName("acceptance_rule")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.Classification)
            .HasColumnName("classification")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.SuiteVersionId, entity.CaseCode })
            .IsUnique()
            .HasDatabaseName("uq_ground_truth_case_suite_case_code");

        builder.HasIndex(entity => entity.InputSourceSnapshotId)
            .HasDatabaseName("ix_ground_truth_case_input_snapshot");

        builder.HasOne(entity => entity.SuiteVersion)
            .WithMany()
            .HasForeignKey(entity => entity.SuiteVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ground_truth_case_suite_version");

        builder.HasOne(entity => entity.InputSourceSnapshot)
            .WithMany()
            .HasForeignKey(entity => entity.InputSourceSnapshotId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ground_truth_case_input_snapshot");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ground_truth_case_code", "case_code = upper(btrim(case_code)) AND char_length(case_code) > 0");
            tableBuilder.HasCheckConstraint("ck_ground_truth_case_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
            tableBuilder.HasCheckConstraint("ck_ground_truth_case_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
