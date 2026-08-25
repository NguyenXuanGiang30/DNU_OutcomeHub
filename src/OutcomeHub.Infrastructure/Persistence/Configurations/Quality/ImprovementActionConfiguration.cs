using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class ImprovementActionConfiguration : IEntityTypeConfiguration<ImprovementAction>
{
    public void Configure(EntityTypeBuilder<ImprovementAction> builder)
    {
        builder.ToTable("improvement_action", "quality");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_improvement_action");

        builder.HasAlternateKey(entity => new { entity.Id, entity.ImprovementPlanId }).HasName("uq_improvement_action_id_plan");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ImprovementPlanId)
            .HasColumnName("improvement_plan_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ActionNo)
            .HasColumnName("action_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.OwnerPrincipalId)
            .HasColumnName("owner_principal_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OwnerOrgUnitId)
            .HasColumnName("owner_org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StartDate)
            .HasColumnName("start_date")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.DueDate)
            .HasColumnName("due_date")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.CompletionRatio)
            .HasColumnName("completion_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.RowVersion)
            .HasColumnName("row_version")
            .HasColumnType("bigint")
            .HasDefaultValue(1L)
            .IsConcurrencyToken()
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.ImprovementPlanId, entity.ActionNo })
            .IsUnique()
            .HasDatabaseName("uq_improvement_action_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_improvement_action_number", "action_no > 0");
            table.HasCheckConstraint("ck_improvement_action_dates", "due_date >= start_date");
            table.HasCheckConstraint("ck_improvement_action_completion", "completion_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND completion_ratio >= 0 AND completion_ratio <= 1");
            table.HasCheckConstraint("ck_improvement_action_row_version", "row_version > 0");
        });
        builder.HasOne(entity => entity.ImprovementPlan).WithMany().HasForeignKey(entity => entity.ImprovementPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_action_plan");
        builder.HasOne(entity => entity.OwnerPrincipal).WithMany().HasForeignKey(entity => entity.OwnerPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_action_owner");
        builder.HasOne(entity => entity.OwnerOrgUnit).WithMany().HasForeignKey(entity => entity.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_action_owner_org_unit");
    }
}
