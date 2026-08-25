using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class AnchorAssessmentConfiguration : IEntityTypeConfiguration<AnchorAssessment>
{
    public void Configure(EntityTypeBuilder<AnchorAssessment> builder)
    {
        builder.ToTable("anchor_assessment", "academic", table =>
            table.HasCheckConstraint("ck_anchor_assessment_role", "anchor_role IN ('PRIMARY','SECONDARY','COMPARISON')"));
        builder.HasKey(x => x.Id).HasName("pk_anchor_assessment");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.DirectMeasurementSourceId).HasColumnName("direct_measurement_source_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AssessmentItemId).HasColumnName("assessment_item_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AnchorRole).HasColumnName("anchor_role").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.EvidenceRequirement).HasColumnName("evidence_requirement").HasColumnType("text").IsRequired();
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz").IsRequired(false);
        builder.HasIndex(x => new { x.DirectMeasurementSourceId, x.AssessmentItemId, x.AnchorRole }).IsUnique().HasDatabaseName("uq_anchor_assessment_source_item_role");
        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId, x.AssessmentItemId }).HasName("uq_anchor_assessment_id_version_item");
        builder.HasOne(x => x.DirectMeasurementSource).WithMany().HasForeignKey(x => x.DirectMeasurementSourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_anchor_assessment_source");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_anchor_assessment_syllabus_version");
        builder.HasOne(x => x.AssessmentItem).WithMany().HasForeignKey(x => new { x.AssessmentItemId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_anchor_assessment_item_version");
    }
}
