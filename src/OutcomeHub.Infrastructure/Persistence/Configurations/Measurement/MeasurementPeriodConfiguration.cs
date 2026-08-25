using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class MeasurementPeriodConfiguration : IEntityTypeConfiguration<MeasurementPeriod>
{
    public void Configure(EntityTypeBuilder<MeasurementPeriod> builder)
    {
        builder.ToTable("measurement_period", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_measurement_period");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId)
            .HasColumnName("org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.TermCode)
            .HasColumnName("term_code")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPolicyBindingId)
            .HasColumnName("program_policy_binding_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CollectionOpenAt)
            .HasColumnName("collection_open_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.CollectionCloseAt)
            .HasColumnName("collection_close_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.DataCutoffAt)
            .HasColumnName("data_cutoff_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.RowVersion)
            .HasColumnName("row_version")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.OrgUnitId, entity.Code })
            .IsUnique()
            .HasDatabaseName("uq_measurement_period_1");

        builder.HasIndex(entity => new { entity.Id, entity.ProgramVersionId })
            .IsUnique()
            .HasDatabaseName("uq_measurement_period_2");

        builder.HasIndex(entity => new { entity.Id, entity.ProgramVersionId, entity.AcademicYearStart })
            .IsUnique()
            .HasDatabaseName("uq_measurement_period_3");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_measurement_period_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_measurement_period_academic_year", "academic_year_start BETWEEN 1900 AND 9999");
            table.HasCheckConstraint("ck_measurement_period_status", "status IN ('DRAFT','OPEN','COLLECTING','RECONCILING','CALCULATED','APPROVED','PUBLISHED','CLOSED','REOPENED')");
            table.HasCheckConstraint("ck_measurement_period_collection_window", "collection_close_at IS NULL OR collection_open_at IS NOT NULL AND collection_close_at > collection_open_at");
        });
        builder.Property(entity => entity.RowVersion).HasDefaultValue(1L).IsConcurrencyToken();
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_measurement_period_workflow");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_org_unit");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_program_version");
        builder.HasOne(entity => entity.ProgramPolicyBinding).WithMany().HasForeignKey(entity => new { entity.ProgramPolicyBindingId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_policy_binding");
        builder.HasOne(entity => entity.WorkflowInstance).WithMany().HasForeignKey(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_workflow");
    }
}
