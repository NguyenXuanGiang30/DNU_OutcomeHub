using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramVersionCohortConfiguration : IEntityTypeConfiguration<ProgramVersionCohort>
{
    public void Configure(EntityTypeBuilder<ProgramVersionCohort> builder)
    {
        builder.ToTable("program_version_cohort", "academic", table =>
            table.HasCheckConstraint("ck_program_version_cohort_range", "effective_to IS NULL OR effective_to > effective_from"));
        builder.HasKey(x => new { x.ProgramVersionId, x.CohortId }).HasName("pk_program_version_cohort");
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CohortId).HasColumnName("cohort_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.IsDefault).HasColumnName("is_default").HasColumnType("boolean").IsRequired();
        builder.HasIndex(x => new { x.CohortId, x.IsDefault }).HasDatabaseName("ix_program_version_cohort_default");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_program_version_cohort_default_range", "cohort_id WITH =, daterange(effective_from, effective_to, '[)') WITH && WHERE (is_default)");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_cohort_version");
        builder.HasOne(x => x.Cohort).WithMany().HasForeignKey(x => x.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_cohort_cohort");
    }
}
