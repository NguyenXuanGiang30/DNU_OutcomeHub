using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class PoCompetencyMappingConfiguration : IEntityTypeConfiguration<PoCompetencyMapping>
{
    public void Configure(EntityTypeBuilder<PoCompetencyMapping> builder)
    {
        builder.ToTable("po_competency_mapping", "academic", table =>
            table.HasCheckConstraint("ck_po_competency_mapping_level", "mapping_level IN ('L','M','H')"));
        builder.HasKey(x => new { x.ProgramObjectiveId, x.CompetencyId }).HasName("pk_po_competency_mapping");
        builder.Property(x => x.ProgramObjectiveId).HasColumnName("program_objective_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CompetencyId).HasColumnName("competency_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.MappingLevel).HasColumnName("mapping_level").HasColumnType("char(1)").HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasOne(x => x.ProgramObjective).WithMany().HasForeignKey(x => new { x.ProgramObjectiveId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_po_competency_mapping_objective_version");
        builder.HasOne(x => x.Competency).WithMany().HasForeignKey(x => new { x.CompetencyId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_po_competency_mapping_competency_version");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_po_competency_mapping_version");
    }
}
