using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class EvidenceConfiguration : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.ToTable("evidence", "document", table =>
            {
                table.HasCheckConstraint("ck_evidence_code", "code = upper(btrim(code)) AND char_length(code) > 0");
                table.HasCheckConstraint("ck_evidence_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
                table.HasCheckConstraint("ck_evidence_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','SUPERSEDED','ARCHIVED')");
            });
        builder.HasKey(x => x.Id).HasName("pk_evidence");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.EvidenceType).HasColumnName("evidence_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.OwnerPrincipalId).HasColumnName("owner_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("uq_evidence_code");
        builder.HasIndex(x => new { x.OwnerOrgUnitId, x.Status }).HasDatabaseName("ix_evidence_owner_status");
        builder.HasOne(x => x.OwnerPrincipal).WithMany().HasForeignKey(x => x.OwnerPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_owner_principal");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_owner_org_unit");
    }
}

