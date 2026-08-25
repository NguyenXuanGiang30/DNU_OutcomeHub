using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocumentEntity = OutcomeHub.Domain.Entities.Document.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.ToTable("document", "document", table =>
            {
                table.HasCheckConstraint("ck_document_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
                table.HasCheckConstraint("ck_document_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','SUPERSEDED','ARCHIVED')");
            });
        builder.HasKey(x => x.Id).HasName("pk_document");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DocumentType).HasColumnName("document_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => x.GovernedResourceId).IsUnique().HasDatabaseName("uq_document_governed_resource");
        builder.HasIndex(x => new { x.OwnerOrgUnitId, x.DocumentType, x.Status }).HasDatabaseName("ix_document_owner_type_status");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_governed_resource");
        builder.HasOne(x => x.OwnerOrgUnit).WithMany().HasForeignKey(x => x.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_document_owner_org_unit");
    }
}
