using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.MultiTenancy;
using Grc.Product.Subscriptions;

namespace Grc.Product.EntityFrameworkCore.Subscriptions;

public class QuotaUsageConfiguration : IEntityTypeConfiguration<QuotaUsage>
{
    public void Configure(EntityTypeBuilder<QuotaUsage> builder)
    {
        builder.ToTable("quota_usages", "grc");
        builder.ConfigureByConvention();
        builder.ConfigureMultiTenant();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.QuotaType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CurrentUsage)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.ResetDate);

        builder.Property(x => x.LastUpdated)
            .IsRequired();

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.QuotaType);
        builder.HasIndex(x => x.ResetDate)
            .HasFilter("[ResetDate] IS NOT NULL");

        builder.HasIndex(x => new { x.TenantId, x.QuotaType })
            .IsUnique();
    }
}


