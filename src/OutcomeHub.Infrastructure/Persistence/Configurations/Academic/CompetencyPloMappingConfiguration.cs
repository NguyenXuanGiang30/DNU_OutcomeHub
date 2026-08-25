using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CompetencyPloMappingConfiguration : IEntityTypeConfiguration<CompetencyPloMapping>
{
    public void Configure(EntityTypeBuilder<CompetencyPloMapping> builder)
    {
        builder.ToTable("competency_plo_mapping", "academic", table =>
            table.HasCheckConstraint("ck_competency_plo_mapping_level", "mapping_level IN ('L','M','H')"));
        builder.HasKey(x => new { x.CompetencyId, x.ProgramPloId }).HasName("pk_competency_plo_mapping");
        builder.Property(x => x.CompetencyId).HasColumnName("competency_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramPloId).HasColumnName("program_plo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.MappingLevel).HasColumnName("mapping_level").HasColumnType("char(1)").HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasOne(x => x.Competency).WithMany().HasForeignKey(x => new { x.CompetencyId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_competency_plo_mapping_competency_version");
        builder.HasOne(x => x.ProgramPlo).WithMany().HasForeignKey(x => new { x.ProgramPloId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_competency_plo_mapping_plo_version");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_competency_plo_mapping_version");
    }
}
