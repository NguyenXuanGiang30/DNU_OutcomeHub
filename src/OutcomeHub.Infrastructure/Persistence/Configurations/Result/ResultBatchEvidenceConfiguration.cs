using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class ResultBatchEvidenceConfiguration : IEntityTypeConfiguration<ResultBatchEvidence>
{
    public void Configure(EntityTypeBuilder<ResultBatchEvidence> builder)
    {
        builder.ToTable("result_batch_evidence", "result");

        builder.HasKey(entity => new { entity.BatchId, entity.EvidenceVersionId, entity.LinkRole })
            .HasName("pk_result_batch_evidence");

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EvidenceVersionId)
            .HasColumnName("evidence_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.LinkRole)
            .HasColumnName("link_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => entity.BatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_evidence_batch");
        builder.HasOne(entity => entity.EvidenceVersion).WithMany().HasForeignKey(entity => entity.EvidenceVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_evidence_evidence_version");
    }
}
