using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class DirectMeasurementPlanConfiguration : IEntityTypeConfiguration<DirectMeasurementPlan>
{
    public void Configure(EntityTypeBuilder<DirectMeasurementPlan> builder)
    {
        builder.ToTable("direct_measurement_plan", "academic", table =>
        {
            table.HasCheckConstraint("ck_direct_measurement_plan_version", "version_no > 0");
            table.HasCheckConstraint("ck_direct_measurement_plan_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_direct_measurement_plan_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_direct_measurement_plan_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_direct_measurement_plan");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId, x.CurriculumPathId, x.ProgramPiId }).HasName("uq_direct_measurement_plan_binding");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramPiId).HasColumnName("program_pi_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasIndex(x => new { x.ProgramVersionId, x.CurriculumPathId, x.ProgramPiId, x.VersionNo }).IsUnique().HasDatabaseName("uq_direct_measurement_plan_version");
        builder.HasIndex(x => x.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_direct_measurement_plan_workflow");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_direct_measurement_plan_active_range", "program_version_id WITH =, curriculum_path_id WITH =, program_pi_id WITH =, daterange(effective_from, effective_to, '[)') WITH && WHERE (status = 'ACTIVE')");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_plan_version");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => new { x.CurriculumPathId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_plan_path_version");
        builder.HasOne(x => x.ProgramPi).WithMany().HasForeignKey(x => new { x.ProgramPiId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_plan_pi_version");
        builder.HasOne(x => x.WorkflowInstance).WithMany().HasForeignKey(x => x.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_plan_workflow");
        builder.HasOne(x => x.Supersedes).WithMany().HasForeignKey(x => x.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_plan_supersedes");
    }
}
