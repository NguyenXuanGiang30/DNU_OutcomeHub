using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("chat_session", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_chat_session");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OwnerPrincipalId)
            .HasColumnName("owner_principal_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AccessScopeId)
            .HasColumnName("access_scope_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Title)
            .HasColumnName("title")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.LastActivityAt)
            .HasColumnName("last_activity_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_chat_session_governed_resource");

        builder.HasIndex(entity => new { entity.OwnerPrincipalId, entity.LastActivityAt })
            .HasDatabaseName("ix_chat_session_owner_activity");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_chat_session_governed_resource");

        builder.HasOne(entity => entity.OwnerPrincipal)
            .WithMany()
            .HasForeignKey(entity => entity.OwnerPrincipalId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_chat_session_owner_principal");

        builder.HasOne(entity => entity.AccessScope)
            .WithMany()
            .HasForeignKey(entity => entity.AccessScopeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_chat_session_access_scope");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_chat_session_status", "status IN ('ACTIVE','CLOSED','ARCHIVED')");
            tableBuilder.HasCheckConstraint("ck_chat_session_activity", "last_activity_at >= created_at");
        });
    }
}
