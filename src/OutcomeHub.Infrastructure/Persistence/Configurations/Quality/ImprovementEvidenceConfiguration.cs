using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class ImprovementEvidenceConfiguration : IEntityTypeConfiguration<ImprovementEvidence>
{
    public void Configure(EntityTypeBuilder<ImprovementEvidence> builder)
    {
        builder.ToTable("improvement_evidence", "quality");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_improvement_evidence");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ImprovementPlanId)
            .HasColumnName("improvement_plan_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ImprovementActionId)
            .HasColumnName("improvement_action_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.EvidenceVersionId)
            .HasColumnName("evidence_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.LinkRole)
            .HasColumnName("link_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.VerifiedBy)
            .HasColumnName("verified_by")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.VerifiedAt)
            .HasColumnName("verified_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.ToTable(table => table.HasCheckConstraint("ck_improvement_evidence_verification", "num_nonnulls(verified_by, verified_at) IN (0, 2)"));
        builder.HasOne(entity => entity.ImprovementPlan).WithMany().HasForeignKey(entity => entity.ImprovementPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_evidence_plan");
        builder.HasOne(entity => entity.ImprovementAction).WithMany().HasForeignKey(entity => new { entity.ImprovementActionId, entity.ImprovementPlanId }).HasPrincipalKey(entity => new { ImprovementActionId = entity.Id, entity.ImprovementPlanId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_evidence_action_plan");
        builder.HasOne(entity => entity.EvidenceVersion).WithMany().HasForeignKey(entity => entity.EvidenceVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_evidence_evidence_version");
        builder.HasOne(entity => entity.VerifiedByPrincipal).WithMany().HasForeignKey(entity => entity.VerifiedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_evidence_verified_by");
        builder.HasIndex(entity => new { entity.ImprovementPlanId, entity.EvidenceVersionId, entity.LinkRole }).IsUnique().HasDatabaseName("uq_improvement_evidence_plan_version_role");
    }
}
