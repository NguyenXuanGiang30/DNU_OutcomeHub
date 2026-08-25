using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class PublicationAudienceConfiguration : IEntityTypeConfiguration<PublicationAudience>
{
    public void Configure(EntityTypeBuilder<PublicationAudience> builder)
    {
        builder.ToTable("publication_audience", "result");

        builder.HasKey(entity => new { entity.PublicationId, entity.AccessScopeId, entity.AudienceRole })
            .HasName("pk_publication_audience");

        builder.Property(entity => entity.PublicationId)
            .HasColumnName("publication_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AccessScopeId)
            .HasColumnName("access_scope_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AudienceRole)
            .HasColumnName("audience_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.AllowStudentDetail)
            .HasColumnName("allow_student_detail")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.HasOne(entity => entity.Publication).WithMany().HasForeignKey(entity => entity.PublicationId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_audience_publication");
        builder.HasOne(entity => entity.AccessScope).WithMany().HasForeignKey(entity => entity.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_publication_audience_access_scope");
    }
}
