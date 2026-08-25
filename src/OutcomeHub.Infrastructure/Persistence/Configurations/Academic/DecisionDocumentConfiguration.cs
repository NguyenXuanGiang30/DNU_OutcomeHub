using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class DecisionDocumentConfiguration : IEntityTypeConfiguration<DecisionDocument>
{
    public void Configure(EntityTypeBuilder<DecisionDocument> builder)
    {
        builder.ToTable("decision_document", "academic");
        builder.HasKey(x => new { x.DecisionRecordId, x.DocumentVersionId, x.DocumentRole }).HasName("pk_decision_document");
        builder.Property(x => x.DecisionRecordId).HasColumnName("decision_record_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DocumentVersionId).HasColumnName("document_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DocumentRole).HasColumnName("document_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.DocumentVersionId).HasDatabaseName("ix_decision_document_document_version");
        builder.HasOne(x => x.DecisionRecord).WithMany().HasForeignKey(x => x.DecisionRecordId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_decision_document_decision");
        builder.HasOne(x => x.DocumentVersion).WithMany().HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_decision_document_document_version");
    }
}
