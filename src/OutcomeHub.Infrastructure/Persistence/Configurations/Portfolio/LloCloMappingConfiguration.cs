using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class LloCloMappingConfiguration : IEntityTypeConfiguration<LloCloMapping>
{
    public void Configure(EntityTypeBuilder<LloCloMapping> builder)
    {
        builder.ToTable("llo_clo_mapping", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_llo_clo_mapping_contribution", "contribution_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND contribution_ratio > 0 AND contribution_ratio <= 1");
            });
        builder.HasKey(x => new { x.LloId, x.CloId }).HasName("pk_llo_clo_mapping");
        builder.Property(x => x.LloId).HasColumnName("llo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CloId).HasColumnName("clo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ContributionRatio).HasColumnName("contribution_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);

        builder.HasOne(x => x.Llo).WithMany().HasForeignKey(x => new { x.LloId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_llo_clo_mapping_llo_version");
        builder.HasOne(x => x.Clo).WithMany().HasForeignKey(x => new { x.CloId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_llo_clo_mapping_clo_version");
    }
}
