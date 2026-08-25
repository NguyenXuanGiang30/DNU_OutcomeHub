using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class EvaluationResultConfiguration : IEntityTypeConfiguration<EvaluationResult>
{
    public void Configure(EntityTypeBuilder<EvaluationResult> builder)
    {
        builder.ToTable("evaluation_result", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_evaluation_result");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.RunId)
            .HasColumnName("run_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CaseId)
            .HasColumnName("case_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ActualOutput)
            .HasColumnName("actual_output")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.FieldPrecision)
            .HasColumnName("field_precision")
            .HasColumnType("numeric(5,4)")
            .HasPrecision(5, 4)
            .IsRequired(true);

        builder.Property(entity => entity.FieldRecall)
            .HasColumnName("field_recall")
            .HasColumnType("numeric(5,4)")
            .HasPrecision(5, 4)
            .IsRequired(true);

        builder.Property(entity => entity.CitationAccuracy)
            .HasColumnName("citation_accuracy")
            .HasColumnType("numeric(5,4)")
            .HasPrecision(5, 4)
            .IsRequired(true);

        builder.Property(entity => entity.SchemaValid)
            .HasColumnName("schema_valid")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.Passed)
            .HasColumnName("passed")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.Classification)
            .HasColumnName("classification")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.RunId, entity.CaseId })
            .IsUnique()
            .HasDatabaseName("uq_evaluation_result_run_case");

        builder.HasIndex(entity => new { entity.RunId, entity.Passed })
            .HasDatabaseName("ix_evaluation_result_run_passed");

        builder.HasOne(entity => entity.Run)
            .WithMany()
            .HasForeignKey(entity => entity.RunId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_result_run");

        builder.HasOne(entity => entity.Case)
            .WithMany()
            .HasForeignKey(entity => entity.CaseId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_result_case");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_evaluation_result_metrics", "field_precision >= 0 AND field_precision <= 1 AND field_recall >= 0 AND field_recall <= 1 AND citation_accuracy >= 0 AND citation_accuracy <= 1 AND field_precision <> 'NaN'::numeric AND field_recall <> 'NaN'::numeric AND citation_accuracy <> 'NaN'::numeric AND field_precision NOT IN ('Infinity'::numeric, '-Infinity'::numeric) AND field_recall NOT IN ('Infinity'::numeric, '-Infinity'::numeric) AND citation_accuracy NOT IN ('Infinity'::numeric, '-Infinity'::numeric)");
            tableBuilder.HasCheckConstraint("ck_evaluation_result_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
            tableBuilder.HasCheckConstraint("ck_evaluation_result_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
