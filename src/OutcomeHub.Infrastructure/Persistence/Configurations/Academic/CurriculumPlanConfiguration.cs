using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CurriculumPlanConfiguration : IEntityTypeConfiguration<CurriculumPlan>
{
    public void Configure(EntityTypeBuilder<CurriculumPlan> builder)
    {
        builder.ToTable("curriculum_plan", "academic", table =>
        {
            table.HasCheckConstraint("ck_curriculum_plan_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_curriculum_plan_credits", "declared_total_credits > 0 AND declared_total_credits <> 'NaN'::numeric AND declared_total_credits NOT IN ('Infinity'::numeric, '-Infinity'::numeric)");
            table.HasCheckConstraint("ck_curriculum_plan_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_curriculum_plan_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_curriculum_plan");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_curriculum_plan_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DeclaredTotalCredits).HasColumnName("declared_total_credits").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.HasIndex(x => x.ProgramVersionId).IsUnique().HasDatabaseName("uq_curriculum_plan_program_version");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_plan_program_version");
    }
}
