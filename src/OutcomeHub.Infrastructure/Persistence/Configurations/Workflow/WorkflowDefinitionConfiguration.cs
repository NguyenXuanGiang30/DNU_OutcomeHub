using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Workflow;

public sealed class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.ToTable("definition", "workflow", table =>
        {
            table.HasCheckConstraint("ck_definition_code", "code = btrim(code) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_definition_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_definition_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_definition_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_definition");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(entity => entity.SubjectType).HasColumnName("subject_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Configuration).HasColumnName("configuration").HasColumnType("jsonb").IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();

        builder.HasIndex(entity => new { entity.Code, entity.VersionNo }).IsUnique().HasDatabaseName("uq_definition_code_version_no");
    }
}
