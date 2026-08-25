using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class ChatTurnConfiguration : IEntityTypeConfiguration<ChatTurn>
{
    public void Configure(EntityTypeBuilder<ChatTurn> builder)
    {
        builder.ToTable("chat_turn", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_chat_turn");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ChatSessionId)
            .HasColumnName("chat_session_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.TurnNo)
            .HasColumnName("turn_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.UserMessageCiphertext)
            .HasColumnName("user_message_ciphertext")
            .HasColumnType("bytea")
            .IsRequired(true);

        builder.Property(entity => entity.AiJobId)
            .HasColumnName("ai_job_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssistantArtifactId)
            .HasColumnName("assistant_artifact_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.DataAsOf)
            .HasColumnName("data_as_of")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.ChatSessionId, entity.TurnNo })
            .IsUnique()
            .HasDatabaseName("uq_chat_turn_session_turn_no");

        builder.HasIndex(entity => entity.AiJobId)
            .HasDatabaseName("ix_chat_turn_ai_job");

        builder.HasIndex(entity => entity.AssistantArtifactId)
            .IsUnique()
            .HasFilter("assistant_artifact_id IS NOT NULL")
            .HasDatabaseName("uq_chat_turn_assistant_artifact");

        builder.HasOne(entity => entity.ChatSession)
            .WithMany()
            .HasForeignKey(entity => entity.ChatSessionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_chat_turn_session");

        builder.HasOne(entity => entity.AiJob)
            .WithMany()
            .HasForeignKey(entity => entity.AiJobId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_chat_turn_ai_job");

        builder.HasOne(entity => entity.AssistantArtifact)
            .WithMany()
            .HasForeignKey(entity => new { entity.AssistantArtifactId, entity.AiJobId })
            .HasPrincipalKey(entity => new { entity.Id, entity.AiJobId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_chat_turn_assistant_artifact_job");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_chat_turn_no", "turn_no > 0");
            tableBuilder.HasCheckConstraint("ck_chat_turn_data_as_of", "data_as_of <= created_at");
        });
    }
}
