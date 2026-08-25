using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class ModelDeploymentVersionConfiguration : IEntityTypeConfiguration<ModelDeploymentVersion>
{
    public void Configure(EntityTypeBuilder<ModelDeploymentVersion> builder)
    {
        builder.ToTable("model_deployment_version", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_model_deployment_version");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ModelDeploymentId)
            .HasColumnName("model_deployment_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.Provider)
            .HasColumnName("provider")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.ProviderModelId)
            .HasColumnName("provider_model_id")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.ProviderModelRevision)
            .HasColumnName("provider_model_revision")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(entity => entity.DeploymentName)
            .HasColumnName("deployment_name")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.Region)
            .HasColumnName("region")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Capability)
            .HasColumnName("capability")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.SecretReference)
            .HasColumnName("secret_reference")
            .HasColumnType("varchar(512)")
            .HasMaxLength(512)
            .IsRequired(true);

        builder.Property(entity => entity.Configuration)
            .HasColumnName("configuration")
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

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("date")
            .IsRequired(false);

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

        builder.HasIndex(entity => new { entity.ModelDeploymentId, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_model_deployment_version_deployment_version_no");

        builder.HasIndex(entity => entity.ActivationDecisionId)
            .IsUnique()
            .HasFilter("activation_decision_id IS NOT NULL")
            .HasDatabaseName("uq_model_deployment_version_activation_decision");

        builder.HasOne(entity => entity.ModelDeployment)
            .WithMany()
            .HasForeignKey(entity => entity.ModelDeploymentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_model_deployment_version_deployment");

        builder.HasOne(entity => entity.Approver)
            .WithMany()
            .HasForeignKey(entity => entity.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_model_deployment_version_approved_by");

        builder.HasOne(entity => entity.ActivationDecision)
            .WithMany()
            .HasForeignKey(entity => new { entity.ActivationDecisionId, entity.Id })
            .HasPrincipalKey(entity => new { entity.Id, entity.ModelDeploymentVersionId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_model_deployment_version_exact_activation");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_no", "version_no > 0");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_range", "effective_to IS NULL OR effective_to > effective_from");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_approved_state", "status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_activation", "status <> 'ACTIVE' OR activation_decision_id IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
            tableBuilder.HasCheckConstraint("ck_model_deployment_version_secret_reference", "secret_reference = btrim(secret_reference) AND char_length(secret_reference) > 0");
        });
    }
}
