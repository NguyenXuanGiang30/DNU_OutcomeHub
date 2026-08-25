using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Audit;

public sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        // RANGE partitioning by occurred_at, local unique indexes (chain_id, chain_sequence)/event_hash,
        // BRIN indexes, reject_mutation trigger, append_event SECURITY DEFINER function and grants
        // require reviewed operational SQL; EF's relational model cannot express these contracts fully.
        builder.ToTable("audit_event", "audit", table =>
        {
            table.HasCheckConstraint("ck_audit_event_outcome", "outcome IN ('SUCCESS', 'DENIED', 'FAILED')");
            table.HasCheckConstraint("ck_audit_event_classification", "classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')");
            table.HasCheckConstraint("ck_audit_event_chain_sequence", "chain_sequence > 0");
            table.HasCheckConstraint("ck_audit_event_previous_hash", "previous_hash IS NULL OR previous_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_audit_event_event_hash", "event_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_audit_event_canonicalization_version", "canonicalization_version > 0");
        });
        builder.HasKey(x => new { x.OccurredAt, x.Id }).HasName("pk_audit_event");
        builder.Property(x => x.OccurredAt).HasColumnName("occurred_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.RequestId).HasColumnName("request_id").HasColumnType("uuid");
        builder.Property(x => x.CorrelationId).HasColumnName("correlation_id").HasColumnType("uuid");
        builder.Property(x => x.TraceId).HasColumnName("trace_id").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.ActorPrincipalId).HasColumnName("actor_principal_id").HasColumnType("uuid");
        builder.Property(x => x.ActorKind).HasColumnName("actor_kind").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ImpersonatorPrincipalId).HasColumnName("impersonator_principal_id").HasColumnType("uuid");
        builder.Property(x => x.Action).HasColumnName("action").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Category).HasColumnName("category").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Outcome).HasColumnName("outcome").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ResourceId).HasColumnName("resource_id").HasColumnType("uuid");
        builder.Property(x => x.ResourceVersion).HasColumnName("resource_version").HasColumnType("bigint");
        builder.Property(x => x.OrgUnitId).HasColumnName("org_unit_id").HasColumnType("uuid");
        builder.Property(x => x.ProgramId).HasColumnName("program_id").HasColumnType("uuid");
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid");
        builder.Property(x => x.CohortId).HasColumnName("cohort_id").HasColumnType("uuid");
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid");
        builder.Property(x => x.CourseId).HasColumnName("course_id").HasColumnType("uuid");
        builder.Property(x => x.CourseOfferingId).HasColumnName("course_offering_id").HasColumnType("uuid");
        builder.Property(x => x.MeasurementPeriodId).HasColumnName("measurement_period_id").HasColumnType("uuid");
        builder.Property(x => x.StudentId).HasColumnName("student_id").HasColumnType("uuid");
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text");
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(x => x.UserAgentHash).HasColumnName("user_agent_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength();
        builder.Property(x => x.AuthMethod).HasColumnName("auth_method").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.BeforeData).HasColumnName("before_data").HasColumnType("jsonb");
        builder.Property(x => x.AfterData).HasColumnName("after_data").HasColumnType("jsonb");
        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
        builder.Property(x => x.ChainId).HasColumnName("chain_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ChainSequence).HasColumnName("chain_sequence").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.PreviousHash).HasColumnName("previous_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength();
        builder.Property(x => x.EventHash).HasColumnName("event_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.HashAlgorithm).HasColumnName("hash_algorithm").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CanonicalizationVersion).HasColumnName("canonicalization_version").HasColumnType("integer").IsRequired();
        builder.HasOne(x => x.ActorPrincipal).WithMany().HasForeignKey(x => x.ActorPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_actor_principal");
        builder.HasOne(x => x.ImpersonatorPrincipal).WithMany().HasForeignKey(x => x.ImpersonatorPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_impersonator_principal");
        builder.HasOne(x => x.OrgUnit).WithMany().HasForeignKey(x => x.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_org_unit");
        builder.HasOne(x => x.Program).WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_program");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_program_version");
        builder.HasOne(x => x.Cohort).WithMany().HasForeignKey(x => x.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_cohort");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => x.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_curriculum_path");
        builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_course");
        builder.HasOne(x => x.CourseOffering).WithMany().HasForeignKey(x => x.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_course_offering");
        builder.HasOne(x => x.MeasurementPeriod).WithMany().HasForeignKey(x => x.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_measurement_period");
        builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_audit_event_student");
        builder.HasIndex(x => x.OccurredAt).IsDescending().HasDatabaseName("ix_audit_event_occurred_at");
        builder.HasIndex(x => new { x.ActorPrincipalId, x.OccurredAt }).IsDescending(false, true).HasDatabaseName("ix_audit_event_actor_occurred_at");
        builder.HasIndex(x => new { x.ResourceType, x.ResourceId, x.OccurredAt }).IsDescending(false, false, true).HasDatabaseName("ix_audit_event_resource_occurred_at");
        builder.HasIndex(x => x.RequestId).HasDatabaseName("ix_audit_event_request_id");
        builder.HasIndex(x => new { x.ProgramVersionId, x.OccurredAt }).IsDescending(false, true).HasDatabaseName("ix_audit_event_program_version_occurred_at");
        builder.HasIndex(x => new { x.ChainId, x.ChainSequence }).IsUnique().HasDatabaseName("uq_audit_event_chain_sequence");
        builder.HasIndex(x => x.EventHash).IsUnique().HasDatabaseName("uq_audit_event_event_hash");
    }
}
