using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class PublicationRevocationConfiguration : IEntityTypeConfiguration<PublicationRevocation>
{
    public void Configure(EntityTypeBuilder<PublicationRevocation> builder)
    {
        builder.ToTable("publication_revocation", "result");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_publication_revocation");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.PublicationId)
            .HasColumnName("publication_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Reason)
            .HasColumnName("reason")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.RevokedBy)
            .HasColumnName("revoked_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.DecisionId)
            .HasColumnName("decision_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.HasIndex(entity => entity.PublicationId)
            .IsUnique()
            .HasDatabaseName("uq_publication_revocation_1");

        builder.HasOne(entity => entity.Publication).WithOne().HasForeignKey<PublicationRevocation>(entity => entity.PublicationId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_revocation_publication");
        builder.HasOne(entity => entity.RevokedByPrincipal).WithMany().HasForeignKey(entity => entity.RevokedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_revocation_revoked_by");
        builder.HasOne(entity => entity.Decision).WithMany().HasForeignKey(entity => entity.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_revocation_decision");
        builder.ToTable(table => table.HasCheckConstraint("ck_publication_revocation_reason", "char_length(btrim(reason)) > 0"));
    }
}
