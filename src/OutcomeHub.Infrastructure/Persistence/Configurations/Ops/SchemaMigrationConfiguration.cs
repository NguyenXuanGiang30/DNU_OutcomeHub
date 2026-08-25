using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ops;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ops;

public sealed class SchemaMigrationConfiguration : IEntityTypeConfiguration<SchemaMigration>
{
    public void Configure(EntityTypeBuilder<SchemaMigration> builder)
    {
        // Advisory-lock acquisition and transactional/operational script execution are runner concerns.
        builder.ToTable("schema_migration", "ops", table =>
        {
            table.HasCheckConstraint("ck_schema_migration_checksum", "checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_schema_migration_transaction_mode", "transaction_mode IN ('TRANSACTIONAL', 'OPERATIONAL')");
            table.HasCheckConstraint("ck_schema_migration_status", "status IN ('PENDING', 'RUNNING', 'APPLIED', 'FAILED')");
            table.HasCheckConstraint("ck_schema_migration_applied_at", "applied_at IS NULL OR applied_at >= started_at");
        });
        builder.HasKey(x => x.Id).HasName("pk_schema_migration");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.MigrationName).HasColumnName("migration_name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.TransactionMode).HasColumnName("transaction_mode").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.AppliedAt).HasColumnName("applied_at").HasColumnType("timestamptz");
        builder.Property(x => x.RunnerVersion).HasColumnName("runner_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ErrorCode).HasColumnName("error_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.HasIndex(x => x.MigrationName).IsUnique().HasDatabaseName("uq_schema_migration_name");
    }
}
