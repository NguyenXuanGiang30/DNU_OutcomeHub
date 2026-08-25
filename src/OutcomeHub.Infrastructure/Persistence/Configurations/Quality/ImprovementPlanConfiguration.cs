using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class ImprovementPlanConfiguration : IEntityTypeConfiguration<ImprovementPlan>
{
    public void Configure(EntityTypeBuilder<ImprovementPlan> builder)
    {
        builder.ToTable("improvement_plan", "quality");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_improvement_plan");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId)
            .HasColumnName("org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Title)
            .HasColumnName("title")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.ProblemStatement)
            .HasColumnName("problem_statement")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.RootCauseSummary)
            .HasColumnName("root_cause_summary")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.BaselineValue)
            .HasColumnName("baseline_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.TargetValue)
            .HasColumnName("target_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.KpiDefinition)
            .HasColumnName("kpi_definition")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.OwnerPrincipalId)
            .HasColumnName("owner_principal_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DueDate)
            .HasColumnName("due_date")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.RowVersion)
            .HasColumnName("row_version")
            .HasColumnType("bigint")
            .HasDefaultValue(1L)
            .IsConcurrencyToken()
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_improvement_plan_1");

        builder.HasIndex(entity => new { entity.OrgUnitId, entity.Code })
            .IsUnique()
            .HasDatabaseName("uq_improvement_plan_2");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_improvement_plan_code", "code = btrim(code) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_improvement_plan_values", "(baseline_value IS NULL OR baseline_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (target_value IS NULL OR target_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric))");
            table.HasCheckConstraint("ck_improvement_plan_row_version", "row_version > 0");
        });
        builder.HasOne(entity => entity.GovernedResource).WithMany().HasForeignKey(entity => entity.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_plan_governed_resource");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_plan_org_unit");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_plan_program_version");
        builder.HasOne(entity => entity.OwnerPrincipal).WithMany().HasForeignKey(entity => entity.OwnerPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_plan_owner");
        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<ImprovementPlan>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_plan_workflow");
        builder.HasOne(entity => entity.CreatedByPrincipal).WithMany().HasForeignKey(entity => entity.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_plan_created_by");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_improvement_plan_workflow");
    }
}
