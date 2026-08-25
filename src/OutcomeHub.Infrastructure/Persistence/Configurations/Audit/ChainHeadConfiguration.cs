using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Audit;

public sealed class ChainHeadConfiguration : IEntityTypeConfiguration<ChainHead>
{
    public void Configure(EntityTypeBuilder<ChainHead> builder)
    {
        // append_event locks this row with FOR UPDATE and increments both sequence and row_version.
        builder.ToTable("chain_head", "audit", table =>
        {
            table.HasCheckConstraint("ck_chain_head_last_sequence", "last_sequence >= 0");
            table.HasCheckConstraint("ck_chain_head_last_hash", "last_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_chain_head_row_version", "row_version > 0");
        });
        builder.HasKey(x => new { x.PartitionStart, x.ChainId }).HasName("pk_chain_head");
        builder.Property(x => x.PartitionStart).HasColumnName("partition_start").HasColumnType("date").IsRequired();
        builder.Property(x => x.ChainId).HasColumnName("chain_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LastSequence).HasColumnName("last_sequence").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.LastHash).HasColumnName("last_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").HasColumnType("bigint").HasDefaultValue(1L).IsConcurrencyToken().IsRequired();
    }
}
