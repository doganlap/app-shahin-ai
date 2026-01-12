using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Grc.EntityFrameworkCore;
using Grc.Product;
using Grc.Product.Products;
using Grc.Product.Subscriptions;

namespace Grc.Product.EntityFrameworkCore;

[DependsOn(
    typeof(GrcProductDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class GrcProductEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<GrcDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddRepository<Product, ProductRepository>();
            options.AddRepository<TenantSubscription, TenantSubscriptionRepository>();
            options.AddRepository<QuotaUsage, QuotaUsageRepository>();
        });
    }
}

