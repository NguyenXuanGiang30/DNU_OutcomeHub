using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("user_account", "iam", table =>
        {
            table.HasCheckConstraint("ck_user_account_username", "username IS NULL OR (username = btrim(username::text) AND char_length(username::text) > 0)");
            table.HasCheckConstraint("ck_user_account_email_lookup_hash", "email_lookup_hash IS NULL OR email_lookup_hash ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => entity.PrincipalId).HasName("pk_user_account");
        builder.Property(entity => entity.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.PersonId).HasColumnName("person_id").HasColumnType("uuid");
        builder.Property(entity => entity.Username).HasColumnName("username").HasColumnType("citext").HasMaxLength(255);
        builder.Property(entity => entity.EmailCiphertext).HasColumnName("email_ciphertext").HasColumnType("bytea");
        builder.Property(entity => entity.EmailLookupHash).HasColumnName("email_lookup_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength();
        builder.Property(entity => entity.LastLoginAt).HasColumnName("last_login_at").HasColumnType("timestamptz");

        builder.HasOne(entity => entity.Principal).WithOne().HasForeignKey<UserAccount>(entity => entity.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_user_account_principal");
        builder.HasOne(entity => entity.Person).WithOne().HasForeignKey<UserAccount>(entity => entity.PersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_user_account_person");
        builder.HasIndex(entity => entity.PersonId).IsUnique().HasDatabaseName("uq_user_account_person_id");
        builder.HasIndex(entity => entity.Username).IsUnique().HasDatabaseName("uq_user_account_username");
        builder.HasIndex(entity => entity.EmailLookupHash).IsUnique().HasDatabaseName("uq_user_account_email_lookup_hash");
    }
}
