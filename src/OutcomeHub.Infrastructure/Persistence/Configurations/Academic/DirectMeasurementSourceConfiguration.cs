using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class DirectMeasurementSourceConfiguration : IEntityTypeConfiguration<DirectMeasurementSource>
{
    public void Configure(EntityTypeBuilder<DirectMeasurementSource> builder)
    {
        builder.ToTable("direct_measurement_source", "academic", table =>
        {
            table.HasCheckConstraint("ck_direct_measurement_source_term", "planned_term IS NULL OR planned_term > 0");
            table.HasCheckConstraint("ck_direct_measurement_source_weight", "source_weight_ratio > 0 AND source_weight_ratio <= 1 AND source_weight_ratio <> 'NaN'::numeric AND source_weight_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric)");
            table.HasCheckConstraint("ck_direct_measurement_source_role", "source_role IN ('OFFICIAL','COMPARISON')");
            table.HasCheckConstraint("ck_direct_measurement_source_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_direct_measurement_source");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.DirectMeasurementPlanId).HasColumnName("direct_measurement_plan_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramPiId).HasColumnName("program_pi_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CoursePiMappingId).HasColumnName("course_pi_mapping_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PlannedTerm).HasColumnName("planned_term").HasColumnType("integer").IsRequired(false);
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SourceWeightRatio).HasColumnName("source_weight_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired();
        builder.Property(x => x.SourceRole).HasColumnName("source_role").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.DirectMeasurementPlanId, x.CoursePiMappingId }).IsUnique().HasDatabaseName("uq_direct_measurement_source_plan_mapping");
        builder.HasOne(x => x.DirectMeasurementPlan).WithMany().HasForeignKey(x => new { x.DirectMeasurementPlanId, x.ProgramVersionId, x.CurriculumPathId, x.ProgramPiId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId, x.CurriculumPathId, x.ProgramPiId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_source_plan_binding");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_source_version");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => new { x.CurriculumPathId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_source_path_version");
        builder.HasOne(x => x.ProgramPi).WithMany().HasForeignKey(x => new { x.ProgramPiId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_source_pi_version");
        builder.HasOne(x => x.CoursePiMapping).WithMany().HasForeignKey(x => new { x.CoursePiMappingId, x.ProgramVersionId, x.ProgramPiId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId, x.ProgramPiId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_source_mapping_binding");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_direct_measurement_source_owner_org_unit");
    }
}
