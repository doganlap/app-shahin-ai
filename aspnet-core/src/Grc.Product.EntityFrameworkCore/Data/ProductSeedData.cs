using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Domain.Shared;
using Grc.Enums;
using Grc.Product.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Grc.Product.EntityFrameworkCore.Data;

/// <summary>
/// Seed data for default products (Trial, Standard, Professional, Enterprise)
/// </summary>
public class ProductSeedData : ITransientDependency
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IServiceProvider _serviceProvider;

    public ProductSeedData(
        IRepository<Product, Guid> productRepository,
        IServiceProvider serviceProvider)
    {
        _productRepository = productRepository;
        _serviceProvider = serviceProvider;
    }

    [UnitOfWork]
    public async Task SeedAsync()
    {
        var existingProducts = await _productRepository.GetListAsync();
        if (existingProducts.Count > 0)
        {
            return; // Already seeded
        }

        var products = new List<Product>();

        // Trial Product
        var trial = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            "Trial",
            new LocalizedString
            {
                En = "Trial",
                Ar = "تجريبي"
            },
            ProductCategory.Subscription)
        {
            Description = new LocalizedString
            {
                En = "Free trial for 14 days with limited features",
                Ar = "تجربة مجانية لمدة 14 يومًا مع ميزات محدودة"
            },
            DisplayOrder = 1,
            IsActive = true
        };

        // Add Trial features
        trial.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            trial.Id,
            "ASSESSMENTS",
            new LocalizedString { En = "Assessments", Ar = "التقييمات" },
            FeatureType.Limit)
        {
            Value = "1",
            Description = new LocalizedString { En = "1 assessment", Ar = "تقييم واحد" }
        });

        trial.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            trial.Id,
            QuotaType.Assessments,
            1));

        trial.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            trial.Id,
            QuotaType.EvidenceStorageMB,
            100));

        trial.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            trial.Id,
            QuotaType.Users,
            3));

        trial.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            trial.Id,
            BillingPeriod.Monthly,
            0)
        {
            TrialDays = 14
        });

        products.Add(trial);

        // Standard Product
        var standard = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            "Standard",
            new LocalizedString
            {
                En = "Standard",
                Ar = "قياسي"
            },
            ProductCategory.Subscription)
        {
            Description = new LocalizedString
            {
                En = "Standard plan for small to medium organizations",
                Ar = "الخطة القياسية للمنظمات الصغيرة والمتوسطة"
            },
            DisplayOrder = 2,
            IsActive = true
        };

        standard.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            standard.Id,
            "ASSESSMENTS",
            new LocalizedString { En = "Unlimited Assessments", Ar = "تقييمات غير محدودة" },
            FeatureType.Unlimited));

        standard.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            standard.Id,
            QuotaType.Assessments,
            null)); // Unlimited

        standard.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            standard.Id,
            QuotaType.EvidenceStorageMB,
            5000));

        standard.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            standard.Id,
            QuotaType.Users,
            25));

        standard.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            standard.Id,
            BillingPeriod.Monthly,
            2999)); // 2999 SAR/month

        standard.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            standard.Id,
            BillingPeriod.Yearly,
            29990)); // 29990 SAR/year (2 months free)

        products.Add(standard);

        // Professional Product
        var professional = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000003"),
            "Professional",
            new LocalizedString
            {
                En = "Professional",
                Ar = "احترافي"
            },
            ProductCategory.Subscription)
        {
            Description = new LocalizedString
            {
                En = "Professional plan for growing organizations with advanced features",
                Ar = "الخطة الاحترافية للمنظمات المتنامية مع ميزات متقدمة"
            },
            DisplayOrder = 3,
            IsActive = true
        };

        professional.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            professional.Id,
            "ASSESSMENTS",
            new LocalizedString { En = "Unlimited Assessments", Ar = "تقييمات غير محدودة" },
            FeatureType.Unlimited));

        professional.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            professional.Id,
            "AI_FEATURES",
            new LocalizedString { En = "AI-Powered Recommendations", Ar = "توصيات مدعومة بالذكاء الاصطناعي" },
            FeatureType.Boolean)
        {
            Value = "true"
        });

        professional.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            professional.Id,
            QuotaType.Assessments,
            null));

        professional.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            professional.Id,
            QuotaType.EvidenceStorageMB,
            20000));

        professional.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            professional.Id,
            QuotaType.Users,
            100));

        professional.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            professional.Id,
            BillingPeriod.Monthly,
            7999)); // 7999 SAR/month

        professional.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            professional.Id,
            BillingPeriod.Yearly,
            79990)); // 79990 SAR/year

        products.Add(professional);

        // Enterprise Product
        var enterprise = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000004"),
            "Enterprise",
            new LocalizedString
            {
                En = "Enterprise",
                Ar = "مؤسسي"
            },
            ProductCategory.Subscription)
        {
            Description = new LocalizedString
            {
                En = "Enterprise plan with unlimited resources and dedicated support",
                Ar = "الخطة المؤسسية مع موارد غير محدودة ودعم مخصص"
            },
            DisplayOrder = 4,
            IsActive = true
        };

        enterprise.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            enterprise.Id,
            "ASSESSMENTS",
            new LocalizedString { En = "Unlimited Assessments", Ar = "تقييمات غير محدودة" },
            FeatureType.Unlimited));

        enterprise.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            enterprise.Id,
            "AI_FEATURES",
            new LocalizedString { En = "AI-Powered Recommendations", Ar = "توصيات مدعومة بالذكاء الاصطناعي" },
            FeatureType.Boolean)
        {
            Value = "true"
        });

        enterprise.Features.Add(new ProductFeature(
            Guid.NewGuid(),
            enterprise.Id,
            "DEDICATED_SUPPORT",
            new LocalizedString { En = "Dedicated Support", Ar = "دعم مخصص" },
            FeatureType.Boolean)
        {
            Value = "true"
        });

        enterprise.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            enterprise.Id,
            QuotaType.Assessments,
            null)); // Unlimited

        enterprise.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            enterprise.Id,
            QuotaType.EvidenceStorageMB,
            null)); // Unlimited

        enterprise.Quotas.Add(new ProductQuota(
            Guid.NewGuid(),
            enterprise.Id,
            QuotaType.Users,
            null)); // Unlimited

        enterprise.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            enterprise.Id,
            BillingPeriod.Monthly,
            19999)); // 19999 SAR/month

        enterprise.PricingPlans.Add(new PricingPlan(
            Guid.NewGuid(),
            enterprise.Id,
            BillingPeriod.Yearly,
            199990)); // 199990 SAR/year

        products.Add(enterprise);

        // Insert all products
        foreach (var product in products)
        {
            await _productRepository.InsertAsync(product);
        }
    }
}
