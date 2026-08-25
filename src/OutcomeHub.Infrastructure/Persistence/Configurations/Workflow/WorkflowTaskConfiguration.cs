using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Workflow;

public sealed class WorkflowTaskConfiguration : IEntityTypeConfiguration<WorkflowTask>
{
    public void Configure(EntityTypeBuilder<WorkflowTask> builder)
    {
        builder.ToTable("task", "workflow", table =>
        {
            table.HasCheckConstraint("ck_task_assignee", "num_nonnulls(assignee_principal_id, assignee_role_id) >= 1");
            table.HasCheckConstraint("ck_task_step_code", "step_code = btrim(step_code) AND char_length(step_code) > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_task");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.InstanceId).HasColumnName("instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.StepCode).HasColumnName("step_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.AssigneePrincipalId).HasColumnName("assignee_principal_id").HasColumnType("uuid");
        builder.Property(entity => entity.AssigneeRoleId).HasColumnName("assignee_role_id").HasColumnType("uuid");
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.DueAt).HasColumnName("due_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.Decision).HasColumnName("decision").HasColumnType("varchar(32)").HasMaxLength(32);
        builder.Property(entity => entity.DecisionReason).HasColumnName("decision_reason").HasColumnType("text");
        builder.Property(entity => entity.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");

        builder.HasOne(entity => entity.Instance).WithMany(entity => entity.Tasks).HasForeignKey(entity => entity.InstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_task_instance");
        builder.HasOne(entity => entity.AssigneePrincipal).WithMany().HasForeignKey(entity => entity.AssigneePrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_task_assignee_principal");
        builder.HasOne(entity => entity.AssigneeRole).WithMany().HasForeignKey(entity => entity.AssigneeRoleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_task_assignee_role");
        builder.HasIndex(entity => new { entity.InstanceId, entity.Status }).HasDatabaseName("ix_task_instance_status");
        builder.HasIndex(entity => new { entity.AssigneePrincipalId, entity.Status }).HasDatabaseName("ix_task_assignee_principal_status");
        builder.HasIndex(entity => new { entity.AssigneeRoleId, entity.Status }).HasDatabaseName("ix_task_assignee_role_status");
    }
}
