using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.MultiTenancy;
using Grc.Product.Subscriptions;

namespace Grc.Product.EntityFrameworkCore.Subscriptions;

public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        builder.ToTable("tenant_subscriptions", "grc");
        builder.ConfigureByConvention();
        builder.ConfigureMultiTenant();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate);

        builder.Property(x => x.TrialEndDate);

        builder.Property(x => x.AutoRenew)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CancelledAt);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(2000);

        builder.Property(x => x.StripeSubscriptionId)
            .HasMaxLength(200);

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.TenantId, x.Status })
            .HasFilter("[Status] = 'Active'");
    }
}


