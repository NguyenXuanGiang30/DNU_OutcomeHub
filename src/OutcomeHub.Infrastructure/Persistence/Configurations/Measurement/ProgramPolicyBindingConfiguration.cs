using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class ProgramPolicyBindingConfiguration : IEntityTypeConfiguration<ProgramPolicyBinding>
{
    public void Configure(EntityTypeBuilder<ProgramPolicyBinding> builder)
    {
        builder.ToTable("program_policy_binding", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_program_policy_binding");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.DecisionId)
            .HasColumnName("decision_id")
            .HasColumnType("uuid")
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

        builder.HasIndex(entity => new { entity.Id, entity.ProgramVersionId })
            .IsUnique()
            .HasDatabaseName("uq_program_policy_binding_1");

        builder.HasIndex(entity => new { entity.Id, entity.ProgramVersionId, entity.PolicyVersionId })
            .IsUnique()
            .HasDatabaseName("uq_program_policy_binding_2");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_program_policy_binding_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_program_policy_binding_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_program_policy_binding_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_program_policy_binding_active_range", "program_version_id WITH =, daterange(effective_from, effective_to, '[)') WITH && WHERE (status = 'ACTIVE')");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_program_policy_binding_workflow");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_binding_program_version");
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_binding_policy_version");
        builder.HasOne(entity => entity.Decision).WithMany().HasForeignKey(entity => entity.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_binding_decision");
        builder.HasOne(entity => entity.WorkflowInstance).WithMany().HasForeignKey(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_policy_binding_workflow");
    }
}
