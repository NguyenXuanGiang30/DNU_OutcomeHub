using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class ValidationIssueConfiguration : IEntityTypeConfiguration<ValidationIssue>
{
    public void Configure(EntityTypeBuilder<ValidationIssue> builder)
    {
        builder.ToTable("validation_issue", "academic", table =>
        {
            table.HasCheckConstraint("ck_validation_issue_severity", "severity IN ('INFO','WARNING','ERROR','BLOCKING')");
            table.HasCheckConstraint("ck_validation_issue_rule_code", "rule_code = upper(btrim(rule_code)) AND char_length(rule_code) > 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_validation_issue");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ValidationRunId).HasColumnName("validation_run_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RuleCode).HasColumnName("rule_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Severity).HasColumnName("severity").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.EntityType).HasColumnName("entity_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.EntityId).HasColumnName("entity_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.FieldPath).HasColumnName("field_path").HasColumnType("varchar(512)").HasMaxLength(512).IsRequired(false);
        builder.Property(x => x.Message).HasColumnName("message").HasColumnType("text").IsRequired();
        builder.Property(x => x.Details).HasColumnName("details").HasColumnType("jsonb").IsRequired(false);
        builder.HasIndex(x => new { x.ValidationRunId, x.Severity, x.RuleCode }).HasDatabaseName("ix_validation_issue_run_severity_rule");
        builder.HasOne(x => x.ValidationRun).WithMany().HasForeignKey(x => x.ValidationRunId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_validation_issue_run");
    }
}
