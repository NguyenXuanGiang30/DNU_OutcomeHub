using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class PromptVersionConfiguration : IEntityTypeConfiguration<PromptVersion>
{
    public void Configure(EntityTypeBuilder<PromptVersion> builder)
    {
        builder.ToTable("prompt_version", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_prompt_version");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.PromptId)
            .HasColumnName("prompt_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.SystemTemplate)
            .HasColumnName("system_template")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.InputContract)
            .HasColumnName("input_contract")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.OutputSchemaVersionId)
            .HasColumnName("output_schema_version_id")
            .HasColumnType("uuid")
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

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(entity => entity.ActivationDecisionId)
            .HasColumnName("activation_decision_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasAlternateKey(entity => new { entity.Id, entity.OutputSchemaVersionId })
            .HasName("uq_prompt_version_id_output_schema");

        builder.HasIndex(entity => new { entity.PromptId, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_prompt_version_prompt_version_no");

        builder.HasIndex(entity => entity.ActivationDecisionId)
            .IsUnique()
            .HasFilter("activation_decision_id IS NOT NULL")
            .HasDatabaseName("uq_prompt_version_activation_decision");

        builder.HasOne(entity => entity.Prompt)
            .WithMany()
            .HasForeignKey(entity => entity.PromptId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_prompt_version_prompt");

        builder.HasOne(entity => entity.OutputSchemaVersion)
            .WithMany()
            .HasForeignKey(entity => entity.OutputSchemaVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_prompt_version_output_schema");

        builder.HasOne(entity => entity.Approver)
            .WithMany()
            .HasForeignKey(entity => entity.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_prompt_version_approved_by");

        builder.HasOne(entity => entity.ActivationDecision)
            .WithMany()
            .HasForeignKey(entity => new { entity.ActivationDecisionId, entity.Id })
            .HasPrincipalKey(entity => new { entity.Id, entity.PromptVersionId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_prompt_version_exact_activation");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_prompt_version_no", "version_no > 0");
            tableBuilder.HasCheckConstraint("ck_prompt_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            tableBuilder.HasCheckConstraint("ck_prompt_version_range", "effective_to IS NULL OR effective_to > effective_from");
            tableBuilder.HasCheckConstraint("ck_prompt_version_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
            tableBuilder.HasCheckConstraint("ck_prompt_version_approved_state", "status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_prompt_version_activation", "status <> 'ACTIVE' OR activation_decision_id IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_prompt_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
