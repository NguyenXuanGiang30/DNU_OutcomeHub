using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Workflow;

public sealed class WorkflowTransitionConfiguration : IEntityTypeConfiguration<WorkflowTransition>
{
    public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
    {
        builder.ToTable("transition", "workflow", table =>
        {
            table.HasCheckConstraint("ck_transition_states", "char_length(btrim(from_state)) > 0 AND char_length(btrim(to_state)) > 0");
            table.HasCheckConstraint("ck_transition_event_code", "event_code = btrim(event_code) AND char_length(event_code) > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_transition");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.InstanceId).HasColumnName("instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.FromState).HasColumnName("from_state").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.ToState).HasColumnName("to_state").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.EventCode).HasColumnName("event_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.ActorPrincipalId).HasColumnName("actor_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.Reason).HasColumnName("reason").HasColumnType("text");
        builder.Property(entity => entity.OccurredAt).HasColumnName("occurred_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.RequestId).HasColumnName("request_id").HasColumnType("uuid").IsRequired();

        builder.HasOne(entity => entity.Instance).WithMany(entity => entity.Transitions).HasForeignKey(entity => entity.InstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_transition_instance");
        builder.HasOne(entity => entity.ActorPrincipal).WithMany().HasForeignKey(entity => entity.ActorPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_transition_actor_principal");
        builder.HasIndex(entity => new { entity.InstanceId, entity.OccurredAt }).HasDatabaseName("ix_transition_instance_occurred_at");
        builder.HasIndex(entity => entity.RequestId).HasDatabaseName("ix_transition_request_id");
    }
}
