using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class PrivacyRequestConfiguration : IEntityTypeConfiguration<PrivacyRequest>
{
    public void Configure(EntityTypeBuilder<PrivacyRequest> builder)
    {
        builder.ToTable("privacy_request", "governance", table =>
            {
                table.HasCheckConstraint("ck_privacy_request_status", "status IN ('RECEIVED','VERIFYING','VERIFIED','IN_REVIEW','APPROVED','REJECTED','PROCESSING','COMPLETED','CANCELLED')");
                table.HasCheckConstraint("ck_privacy_request_timeline", "(verified_at IS NULL OR verified_at >= requested_at) AND (completed_at IS NULL OR completed_at >= requested_at)");
            });
        builder.HasKey(x => x.Id).HasName("pk_privacy_request");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SubjectPersonId).HasColumnName("subject_person_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RequestType).HasColumnName("request_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.LegalBasis).HasColumnName("legal_basis").HasColumnType("text").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RequestedAt).HasColumnName("requested_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.VerifiedAt).HasColumnName("verified_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.DispositionCaseId).HasColumnName("disposition_case_id").HasColumnType("uuid").IsRequired(false);

        builder.HasIndex(x => new { x.SubjectPersonId, x.Status }).HasDatabaseName("ix_privacy_request_subject_status");
        builder.HasIndex(x => x.DispositionCaseId).IsUnique().HasFilter("disposition_case_id IS NOT NULL").HasDatabaseName("uq_privacy_request_disposition_case");
        builder.HasOne(x => x.SubjectPerson).WithMany().HasForeignKey(x => x.SubjectPersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_privacy_request_subject_person");
        builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_privacy_request_approver");
        builder.HasOne(x => x.DispositionCase).WithMany().HasForeignKey(x => x.DispositionCaseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_privacy_request_disposition_case");
    }
}

