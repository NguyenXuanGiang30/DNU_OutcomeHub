using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class OutputSchemaVersionConfiguration : IEntityTypeConfiguration<OutputSchemaVersion>
{
    public void Configure(EntityTypeBuilder<OutputSchemaVersion> builder)
    {
        builder.ToTable("output_schema_version", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_output_schema_version");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.JsonSchema)
            .HasColumnName("json_schema")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ApprovedBy)
            .HasColumnName("approved_by")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ApprovedAt)
            .HasColumnName("approved_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.ActivationDecisionId)
            .HasColumnName("activation_decision_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.Code, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_output_schema_version_code_version_no");

        builder.HasIndex(entity => entity.ActivationDecisionId)
            .IsUnique()
            .HasFilter("activation_decision_id IS NOT NULL")
            .HasDatabaseName("uq_output_schema_version_activation_decision");

        builder.HasOne(entity => entity.Approver)
            .WithMany()
            .HasForeignKey(entity => entity.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_output_schema_version_approved_by");

        builder.HasOne(entity => entity.ActivationDecision)
            .WithMany()
            .HasForeignKey(entity => new { entity.ActivationDecisionId, entity.Id })
            .HasPrincipalKey(entity => new { entity.Id, entity.OutputSchemaVersionId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_output_schema_version_exact_activation");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_output_schema_version_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            tableBuilder.HasCheckConstraint("ck_output_schema_version_no", "version_no > 0");
            tableBuilder.HasCheckConstraint("ck_output_schema_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            tableBuilder.HasCheckConstraint("ck_output_schema_version_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
            tableBuilder.HasCheckConstraint("ck_output_schema_version_approved_state", "status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_output_schema_version_activation", "status <> 'ACTIVE' OR activation_decision_id IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_output_schema_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
