using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CurriculumBlockConfiguration : IEntityTypeConfiguration<CurriculumBlock>
{
    public void Configure(EntityTypeBuilder<CurriculumBlock> builder)
    {
        builder.ToTable("curriculum_block", "academic", table =>
        {
            table.HasCheckConstraint("ck_curriculum_block_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_curriculum_block_credits", "required_credits >= 0 AND (maximum_credits IS NULL OR maximum_credits >= required_credits)");
            table.HasCheckConstraint("ck_curriculum_block_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_curriculum_block");
        builder.HasAlternateKey(x => new { x.Id, x.CurriculumPlanId }).HasName("uq_curriculum_block_id_plan");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.CurriculumPlanId).HasColumnName("curriculum_plan_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ParentId).HasColumnName("parent_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.BlockType).HasColumnName("block_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RequiredCredits).HasColumnName("required_credits").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.MaximumCredits).HasColumnName("maximum_credits").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.CurriculumPlanId, x.Code }).IsUnique().HasDatabaseName("uq_curriculum_block_plan_code");
        builder.HasOne(x => x.CurriculumPlan).WithMany().HasForeignKey(x => x.CurriculumPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_block_plan");
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => new { x.ParentId, x.CurriculumPlanId }).HasPrincipalKey(x => new { x.Id, x.CurriculumPlanId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_block_parent_plan");
    }
}
