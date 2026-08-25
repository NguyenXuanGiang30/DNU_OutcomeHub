using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class PoPloMappingConfiguration : IEntityTypeConfiguration<PoPloMapping>
{
    public void Configure(EntityTypeBuilder<PoPloMapping> builder)
    {
        builder.ToTable("po_plo_mapping", "academic", table =>
            table.HasCheckConstraint("ck_po_plo_mapping_level", "mapping_level IN ('L','M','H')"));
        builder.HasKey(x => new { x.ProgramObjectiveId, x.ProgramPloId }).HasName("pk_po_plo_mapping");
        builder.Property(x => x.ProgramObjectiveId).HasColumnName("program_objective_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramPloId).HasColumnName("program_plo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.MappingLevel).HasColumnName("mapping_level").HasColumnType("char(1)").HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasOne(x => x.ProgramObjective).WithMany().HasForeignKey(x => new { x.ProgramObjectiveId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_po_plo_mapping_objective_version");
        builder.HasOne(x => x.ProgramPlo).WithMany().HasForeignKey(x => new { x.ProgramPloId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_po_plo_mapping_plo_version");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_po_plo_mapping_version");
    }
}
