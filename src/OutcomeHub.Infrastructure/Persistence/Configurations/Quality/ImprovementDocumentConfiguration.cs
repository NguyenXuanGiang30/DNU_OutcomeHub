using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class ImprovementDocumentConfiguration : IEntityTypeConfiguration<ImprovementDocument>
{
    public void Configure(EntityTypeBuilder<ImprovementDocument> builder)
    {
        builder.ToTable("improvement_document", "quality");

        builder.HasKey(entity => new { entity.ImprovementPlanId, entity.DocumentVersionId, entity.DocumentRole })
            .HasName("pk_improvement_document");

        builder.Property(entity => entity.ImprovementPlanId)
            .HasColumnName("improvement_plan_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DocumentVersionId)
            .HasColumnName("document_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DocumentRole)
            .HasColumnName("document_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.HasOne(entity => entity.ImprovementPlan).WithMany().HasForeignKey(entity => entity.ImprovementPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_document_plan");
        builder.HasOne(entity => entity.DocumentVersion).WithMany().HasForeignKey(entity => entity.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_document_document_version");
    }
}
