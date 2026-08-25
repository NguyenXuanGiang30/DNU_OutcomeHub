using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Workflow;

public sealed class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("instance", "workflow", table =>
        {
            table.HasCheckConstraint("ck_instance_current_state", "current_state = btrim(current_state) AND char_length(current_state) > 0");
            table.HasCheckConstraint("ck_instance_completion", "completed_at IS NULL OR completed_at >= started_at");
            table.HasCheckConstraint("ck_instance_row_version", "row_version > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_instance");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.DefinitionId).HasColumnName("definition_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.CurrentState).HasColumnName("current_state").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.StartedBy).HasColumnName("started_by").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.RowVersion).HasColumnName("row_version").HasColumnType("bigint").HasDefaultValue(1L).IsConcurrencyToken().IsRequired();

        builder.HasOne(entity => entity.Definition).WithMany(entity => entity.Instances).HasForeignKey(entity => entity.DefinitionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_instance_definition");
        builder.HasOne(entity => entity.StartedByPrincipal).WithMany().HasForeignKey(entity => entity.StartedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_instance_started_by");
        builder.HasIndex(entity => new { entity.DefinitionId, entity.CurrentState }).HasDatabaseName("ix_instance_definition_state");
    }
}
