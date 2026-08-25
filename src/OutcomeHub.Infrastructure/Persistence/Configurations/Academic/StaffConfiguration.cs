using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("staff", "academic", table =>
        {
            table.HasCheckConstraint("ck_staff_code", "staff_code = upper(btrim(staff_code)) AND char_length(staff_code) > 0");
            table.HasCheckConstraint("ck_staff_status", "current_status IN ('ACTIVE','INACTIVE','SUSPENDED','RETIRED','EXPIRED')");
        });
        builder.HasKey(x => x.PersonId).HasName("pk_staff");
        builder.Property(x => x.PersonId).HasColumnName("person_id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.StaffCode).HasColumnName("staff_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.HomeOrgUnitId).HasColumnName("home_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.StaffType).HasColumnName("staff_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CurrentStatus).HasColumnName("current_status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.StaffCode).IsUnique().HasDatabaseName("uq_staff_code");
        builder.HasOne(x => x.Person).WithOne().HasForeignKey<Staff>(x => x.PersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staff_person");
        builder.HasOne(x => x.HomeOrgUnit).WithMany().HasForeignKey(x => x.HomeOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_staff_home_org_unit");
    }
}
