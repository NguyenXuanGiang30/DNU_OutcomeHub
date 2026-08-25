using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Workflow;

public sealed class WorkflowCommentConfiguration : IEntityTypeConfiguration<WorkflowComment>
{
    public void Configure(EntityTypeBuilder<WorkflowComment> builder)
    {
        builder.ToTable("comment", "workflow", table =>
        {
            table.HasCheckConstraint("ck_comment_body", "char_length(btrim(body)) > 0");
            table.HasCheckConstraint("ck_comment_resolved_at", "resolved_at IS NULL OR resolved_at >= created_at");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_comment");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.InstanceId).HasColumnName("instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.AuthorPrincipalId).HasColumnName("author_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.TargetLocator).HasColumnName("target_locator").HasColumnType("jsonb");
        builder.Property(entity => entity.Body).HasColumnName("body").HasColumnType("text").IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.ResolvedAt).HasColumnName("resolved_at").HasColumnType("timestamptz");

        builder.HasOne(entity => entity.Instance).WithMany(entity => entity.Comments).HasForeignKey(entity => entity.InstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_comment_instance");
        builder.HasOne(entity => entity.AuthorPrincipal).WithMany().HasForeignKey(entity => entity.AuthorPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_comment_author_principal");
        builder.HasIndex(entity => new { entity.InstanceId, entity.CreatedAt }).HasDatabaseName("ix_comment_instance_created_at");
    }
}
