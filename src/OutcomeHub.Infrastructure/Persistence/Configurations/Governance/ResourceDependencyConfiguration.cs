using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class ResourceDependencyConfiguration : IEntityTypeConfiguration<ResourceDependency>
{
    public void Configure(EntityTypeBuilder<ResourceDependency> builder)
    {
        builder.ToTable("resource_dependency", "governance", table =>
            {
                table.HasCheckConstraint("ck_resource_dependency_not_self", "parent_governed_resource_id <> child_governed_resource_id");
            });
        builder.HasKey(x => new { x.ParentGovernedResourceId, x.ChildGovernedResourceId, x.DependencyRole }).HasName("pk_resource_dependency");
        builder.Property(x => x.ParentGovernedResourceId).HasColumnName("parent_governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ChildGovernedResourceId).HasColumnName("child_governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DependencyRole).HasColumnName("dependency_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();

        builder.HasIndex(x => x.ChildGovernedResourceId).HasDatabaseName("ix_resource_dependency_child");
        builder.HasOne(x => x.ParentGovernedResource).WithMany().HasForeignKey(x => x.ParentGovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_dependency_parent");
        builder.HasOne(x => x.ChildGovernedResource).WithMany().HasForeignKey(x => x.ChildGovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_resource_dependency_child");
    }
}

