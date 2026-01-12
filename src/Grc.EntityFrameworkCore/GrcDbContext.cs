using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Grc.Product.Products;
using Grc.Product.Subscriptions;

namespace Grc.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class GrcDbContext : AbpDbContext<GrcDbContext>
{
    /* Add DbSet properties for your entities here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    // Product Module DbSets
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductFeature> ProductFeatures { get; set; }
    public DbSet<ProductQuota> ProductQuotas { get; set; }
    public DbSet<PricingPlan> PricingPlans { get; set; }
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<QuotaUsage> QuotaUsages { get; set; }

    public GrcDbContext(DbContextOptions<GrcDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigureProduct();
        builder.ConfigureSubscription();
    }
}

