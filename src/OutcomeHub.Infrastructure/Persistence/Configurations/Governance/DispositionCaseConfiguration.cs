using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class DispositionCaseConfiguration : IEntityTypeConfiguration<DispositionCase>
{
    public void Configure(EntityTypeBuilder<DispositionCase> builder)
    {
        builder.ToTable("disposition_case", "governance", table =>
            {
                table.HasCheckConstraint("ck_disposition_case_code", "case_code = upper(btrim(case_code)) AND char_length(case_code) > 0");
                table.HasCheckConstraint("ck_disposition_case_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','RUNNING','COMPLETED','FAILED','CANCELLED')");
                table.HasCheckConstraint("ck_disposition_case_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
                table.HasCheckConstraint("ck_disposition_case_certificate", "disposal_certificate_checksum IS NULL OR disposal_certificate_checksum ~ '^[0-9a-f]{64}$'");
            });
        builder.HasKey(x => x.Id).HasName("pk_disposition_case");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CaseCode).HasColumnName("case_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RequestedAction).HasColumnName("requested_action").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.DisposalCertificateChecksum).HasColumnName("disposal_certificate_checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();

        builder.HasIndex(x => x.CaseCode).IsUnique().HasDatabaseName("uq_disposition_case_code");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_disposition_case_status");
        builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_disposition_case_creator");
        builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_disposition_case_approver");
    }
}

