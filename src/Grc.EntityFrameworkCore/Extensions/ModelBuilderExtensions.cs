using Microsoft.EntityFrameworkCore;
using Grc.Product.EntityFrameworkCore.Products;
using Grc.Product.EntityFrameworkCore.Subscriptions;

namespace Grc.EntityFrameworkCore;

public static class GrcDbContextModelBuilderExtensions
{
    public static void ConfigureProduct(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new ProductFeatureConfiguration());
        builder.ApplyConfiguration(new ProductQuotaConfiguration());
        builder.ApplyConfiguration(new PricingPlanConfiguration());
    }

    public static void ConfigureSubscription(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TenantSubscriptionConfiguration());
        builder.ApplyConfiguration(new QuotaUsageConfiguration());
    }
}


