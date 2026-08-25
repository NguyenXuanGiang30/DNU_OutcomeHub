using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class PlanWaiverConfiguration : IEntityTypeConfiguration<PlanWaiver>
{
    public void Configure(EntityTypeBuilder<PlanWaiver> builder)
    {
        builder.ToTable("plan_waiver", "quality");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_plan_waiver");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.FindingId)
            .HasColumnName("finding_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Reason)
            .HasColumnName("reason")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.RequestedBy)
            .HasColumnName("requested_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_plan_waiver_reason", "char_length(btrim(reason)) > 0"));
        builder.HasOne(entity => entity.Finding).WithMany().HasForeignKey(entity => entity.FindingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_plan_waiver_finding");
        builder.HasOne(entity => entity.RequestedByPrincipal).WithMany().HasForeignKey(entity => entity.RequestedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_plan_waiver_requested_by");
        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<PlanWaiver>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_plan_waiver_workflow");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_plan_waiver_workflow");
    }
}
