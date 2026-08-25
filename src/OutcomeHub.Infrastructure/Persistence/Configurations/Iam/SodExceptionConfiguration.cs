using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class SodExceptionConfiguration : IEntityTypeConfiguration<SodException>
{
    public void Configure(EntityTypeBuilder<SodException> builder)
    {
        builder.ToTable("sod_exception", "iam", table =>
        {
            table.HasCheckConstraint("ck_sod_exception_reason", "char_length(btrim(reason)) > 0");
            table.HasCheckConstraint("ck_sod_exception_effective_range", "effective_to > effective_from");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_sod_exception");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.RuleId).HasColumnName("rule_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.Reason).HasColumnName("reason").HasColumnType("text").IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.DecisionId).HasColumnName("decision_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired();

        builder.HasOne(entity => entity.Rule).WithMany(entity => entity.Exceptions).HasForeignKey(entity => entity.RuleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_exception_rule");
        builder.HasOne(entity => entity.Principal).WithMany().HasForeignKey(entity => entity.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_exception_principal");
        builder.HasOne(entity => entity.AccessScope).WithMany().HasForeignKey(entity => entity.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_exception_access_scope");
        builder.HasOne(entity => entity.Decision).WithMany().HasForeignKey(entity => entity.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_exception_decision");
        builder.HasOne(entity => entity.ApprovedByPrincipal).WithMany().HasForeignKey(entity => entity.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_exception_approved_by");
        builder.HasIndex(entity => new { entity.PrincipalId, entity.AccessScopeId, entity.EffectiveFrom, entity.EffectiveTo }).HasDatabaseName("ix_sod_exception_principal_scope_effective");
    }
}
