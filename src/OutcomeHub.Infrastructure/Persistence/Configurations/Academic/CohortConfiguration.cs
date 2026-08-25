using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CohortConfiguration : IEntityTypeConfiguration<Cohort>
{
    public void Configure(EntityTypeBuilder<Cohort> builder)
    {
        builder.ToTable("cohort", "academic", table =>
        {
            table.HasCheckConstraint("ck_cohort_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_cohort_admission_year", "admission_year BETWEEN 1900 AND 9999");
            table.HasCheckConstraint("ck_cohort_date_range", "end_date IS NULL OR end_date >= start_date");
        });
        builder.HasKey(x => x.Id).HasName("pk_cohort");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramId }).HasName("uq_cohort_id_program");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramId).HasColumnName("program_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.AdmissionYear).HasColumnName("admission_year").HasColumnType("integer").IsRequired();
        builder.Property(x => x.StartDate).HasColumnName("start_date").HasColumnType("date").IsRequired();
        builder.Property(x => x.EndDate).HasColumnName("end_date").HasColumnType("date").IsRequired(false);
        builder.HasIndex(x => new { x.ProgramId, x.Code }).IsUnique().HasDatabaseName("uq_cohort_program_code");
        builder.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_program");
    }
}
