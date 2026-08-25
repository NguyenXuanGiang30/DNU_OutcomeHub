using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class AuthSessionConfiguration : IEntityTypeConfiguration<AuthSession>
{
    public void Configure(EntityTypeBuilder<AuthSession> builder)
    {
        builder.ToTable("auth_session", "iam", table =>
        {
            table.HasCheckConstraint("ck_auth_session_token_hash", "session_token_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_auth_session_idp_hash", "idp_session_hash IS NULL OR idp_session_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_auth_session_user_agent_hash", "user_agent_hash IS NULL OR user_agent_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_auth_session_times", "last_seen_at >= issued_at AND expires_at > issued_at AND (revoked_at IS NULL OR revoked_at >= issued_at)");
            table.HasCheckConstraint("ck_auth_session_auth_strength", "auth_strength = btrim(auth_strength) AND char_length(auth_strength) > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_auth_session");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.SessionTokenHash).HasColumnName("session_token_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(entity => entity.IdpSessionHash).HasColumnName("idp_session_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength();
        builder.Property(entity => entity.IssuedAt).HasColumnName("issued_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.LastSeenAt).HasColumnName("last_seen_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.RevokedAt).HasColumnName("revoked_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(entity => entity.UserAgentHash).HasColumnName("user_agent_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength();
        builder.Property(entity => entity.AuthStrength).HasColumnName("auth_strength").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.MfaUsed).HasColumnName("mfa_used").HasColumnType("boolean").IsRequired();

        builder.HasOne(entity => entity.Principal).WithMany().HasForeignKey(entity => entity.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_auth_session_principal");
        builder.HasIndex(entity => entity.SessionTokenHash).IsUnique().HasDatabaseName("uq_auth_session_token_hash");
        builder.HasIndex(entity => entity.IdpSessionHash).HasDatabaseName("ix_auth_session_idp_session_hash");
        builder.HasIndex(entity => new { entity.PrincipalId, entity.ExpiresAt }).HasDatabaseName("ix_auth_session_principal_expires_at");
    }
}
