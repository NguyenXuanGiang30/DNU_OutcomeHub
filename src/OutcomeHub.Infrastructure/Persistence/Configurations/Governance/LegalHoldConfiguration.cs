using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class LegalHoldConfiguration : IEntityTypeConfiguration<LegalHold>
{
    public void Configure(EntityTypeBuilder<LegalHold> builder)
    {
        builder.ToTable("legal_hold", "governance", table =>
            {
                table.HasCheckConstraint("ck_legal_hold_code", "code = upper(btrim(code)) AND char_length(code) > 0");
                table.HasCheckConstraint("ck_legal_hold_status", "status IN ('DRAFT','ACTIVE','RELEASED','CANCELLED')");
                table.HasCheckConstraint("ck_legal_hold_release", "released_at IS NULL OR released_at >= effective_from");
            });
        builder.HasKey(x => x.Id).HasName("pk_legal_hold");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ReleasedAt).HasColumnName("released_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired(false);

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("uq_legal_hold_code");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_legal_hold_status");
        builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_legal_hold_creator");
        builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_legal_hold_approver");
    }
}

