using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ProgramVersionCrosswalkConfiguration : IEntityTypeConfiguration<ProgramVersionCrosswalk>
{
    public void Configure(EntityTypeBuilder<ProgramVersionCrosswalk> builder)
    {
        builder.ToTable("program_version_crosswalk", "academic", table =>
        {
            table.HasCheckConstraint("ck_program_version_crosswalk_distinct", "from_program_version_id <> to_program_version_id");
            table.HasCheckConstraint("ck_program_version_crosswalk_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
        });
        builder.HasKey(x => x.Id).HasName("pk_program_version_crosswalk");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.FromProgramVersionId).HasColumnName("from_program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ToProgramVersionId).HasColumnName("to_program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasIndex(x => new { x.FromProgramVersionId, x.ToProgramVersionId }).IsUnique().HasDatabaseName("uq_program_version_crosswalk_pair");
        builder.HasOne(x => x.FromProgramVersion).WithMany().HasForeignKey(x => x.FromProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_crosswalk_from");
        builder.HasOne(x => x.ToProgramVersion).WithMany().HasForeignKey(x => x.ToProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_crosswalk_to");
        builder.HasOne(x => x.Decision).WithMany().HasForeignKey(x => x.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_program_version_crosswalk_decision");
    }
}
