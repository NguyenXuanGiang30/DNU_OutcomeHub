using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class DecisionRecordConfiguration : IEntityTypeConfiguration<DecisionRecord>
{
    public void Configure(EntityTypeBuilder<DecisionRecord> builder)
    {
        builder.ToTable("decision_record", "academic", table =>
        {
            table.HasCheckConstraint("ck_decision_record_number", "decision_number = btrim(decision_number) AND char_length(decision_number) > 0");
            table.HasCheckConstraint("ck_decision_record_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
        });
        builder.HasKey(x => x.Id).HasName("pk_decision_record");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.DecisionNumber).HasColumnName("decision_number").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.IssuedOn).HasColumnName("issued_on").HasColumnType("date").IsRequired();
        builder.Property(x => x.IssuerOrgUnitId).HasColumnName("issuer_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DocumentVersionId).HasColumnName("document_version_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.IssuerOrgUnitId, x.DecisionNumber }).IsUnique().HasDatabaseName("uq_decision_record_issuer_number");
        builder.HasIndex(x => x.DocumentVersionId).HasDatabaseName("ix_decision_record_document_version_id");
        builder.HasOne(x => x.IssuerOrgUnit).WithMany().HasForeignKey(x => x.IssuerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_decision_record_issuer_org_unit");
        builder.HasOne(x => x.DocumentVersion).WithMany().HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_decision_record_document_version");
    }
}
