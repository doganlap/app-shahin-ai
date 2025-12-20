using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Grc.Domain.Shared;
using Grc.Product.Products;

namespace Grc.Product.EntityFrameworkCore.Data;

public class ProductSeedData : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<ProductFeature, Guid> _featureRepository;
    private readonly IRepository<ProductQuota, Guid> _quotaRepository;
    private readonly IRepository<PricingPlan, Guid> _pricingPlanRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ProductSeedData(
        IRepository<Product, Guid> productRepository,
        IRepository<ProductFeature, Guid> featureRepository,
        IRepository<ProductQuota, Guid> quotaRepository,
        IRepository<PricingPlan, Guid> pricingPlanRepository,
        IGuidGenerator guidGenerator)
    {
        _productRepository = productRepository;
        _featureRepository = featureRepository;
        _quotaRepository = quotaRepository;
        _pricingPlanRepository = pricingPlanRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _productRepository.GetCountAsync() > 0)
            return; // Data already seeded

        await SeedTrialProductAsync();
        await SeedStandardProductAsync();
        await SeedProfessionalProductAsync();
        await SeedEnterpriseProductAsync();
    }

    private async Task SeedTrialProductAsync()
    {
        var trialProduct = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            "Trial",
            new LocalizedString("Trial", "تجريبي"),
            Enums.ProductCategory.Subscription
        );

        trialProduct.UpdateDescription(new LocalizedString(
            "14-day free trial with limited features",
            "نسخة تجريبية مجانية لمدة 14 يومًا مع ميزات محدودة"
        ));
        trialProduct.SetDisplayOrder(0);

        await _productRepository.InsertAsync(trialProduct);

        // Add features
        await _featureRepository.InsertAsync(new ProductFeature(
            _guidGenerator.Create(),
            trialProduct.Id,
            "FrameworkLibrary",
            new LocalizedString("Framework Library", "مكتبة الإطار"),
            Enums.FeatureType.Module
        ));

        await _featureRepository.InsertAsync(new ProductFeature(
            _guidGenerator.Create(),
            trialProduct.Id,
            "Assessment",
            new LocalizedString("Assessment Management", "إدارة التقييمات"),
            Enums.FeatureType.Module
        ));

        // Add quotas
        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            trialProduct.Id,
            Enums.QuotaType.MaxUsers,
            5
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            trialProduct.Id,
            Enums.QuotaType.MaxAssessments,
            1
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            trialProduct.Id,
            Enums.QuotaType.MaxStorageGB,
            1
        ));

        // Add pricing plan (free trial)
        var trialPlan = new PricingPlan(
            _guidGenerator.Create(),
            trialProduct.Id,
            Enums.BillingPeriod.Monthly,
            0
        );
        trialPlan.SetTrialDays(14);
        await _pricingPlanRepository.InsertAsync(trialPlan);
    }

    private async Task SeedStandardProductAsync()
    {
        var standardProduct = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            "Standard",
            new LocalizedString("Standard", "قياسي"),
            Enums.ProductCategory.Subscription
        );

        standardProduct.UpdateDescription(new LocalizedString(
            "Basic compliance features for small to medium organizations",
            "ميزات الامتثال الأساسية للمنظمات الصغيرة والمتوسطة"
        ));
        standardProduct.SetDisplayOrder(1);

        await _productRepository.InsertAsync(standardProduct);

        // Add features
        var features = new[]
        {
            ("FrameworkLibrary", "Framework Library", "مكتبة الإطار"),
            ("Assessment", "Assessment Management", "إدارة التقييمات"),
            ("Evidence", "Evidence Management", "إدارة الأدلة"),
            ("Dashboard", "Dashboard & Reports", "لوحة التحكم والتقارير")
        };

        foreach (var (code, en, ar) in features)
        {
            await _featureRepository.InsertAsync(new ProductFeature(
                _guidGenerator.Create(),
                standardProduct.Id,
                code,
                new LocalizedString(en, ar),
                Enums.FeatureType.Module
            ));
        }

        // Add quotas
        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            standardProduct.Id,
            Enums.QuotaType.MaxUsers,
            25
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            standardProduct.Id,
            Enums.QuotaType.MaxAssessments,
            5
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            standardProduct.Id,
            Enums.QuotaType.MaxStorageGB,
            10
        ));

        // Add pricing plans
        var standardMonthly = new PricingPlan(
            _guidGenerator.Create(),
            standardProduct.Id,
            Enums.BillingPeriod.Monthly,
            999
        );
        await _pricingPlanRepository.InsertAsync(standardMonthly);

        var standardAnnual = new PricingPlan(
            _guidGenerator.Create(),
            standardProduct.Id,
            Enums.BillingPeriod.Annual,
            9999
        );
        await _pricingPlanRepository.InsertAsync(standardAnnual);
    }

    private async Task SeedProfessionalProductAsync()
    {
        var proProduct = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000003"),
            "Professional",
            new LocalizedString("Professional", "احترافي"),
            Enums.ProductCategory.Subscription
        );

        proProduct.UpdateDescription(new LocalizedString(
            "Advanced features for growing organizations with comprehensive compliance needs",
            "ميزات متقدمة للمنظمات المتنامية مع احتياجات امتثال شاملة"
        ));
        proProduct.SetDisplayOrder(2);

        await _productRepository.InsertAsync(proProduct);

        // Add all features
        var features = new[]
        {
            ("FrameworkLibrary", "Framework Library", "مكتبة الإطار"),
            ("Assessment", "Assessment Management", "إدارة التقييمات"),
            ("Evidence", "Evidence Management", "إدارة الأدلة"),
            ("Dashboard", "Dashboard & Reports", "لوحة التحكم والتقارير"),
            ("Risk", "Risk Management", "إدارة المخاطر"),
            ("ActionPlan", "Action Plans", "خطط العمل"),
            ("Workflow", "Workflow Engine", "محرك سير العمل"),
            ("Audit", "Audit Management", "إدارة التدقيق")
        };

        foreach (var (code, en, ar) in features)
        {
            await _featureRepository.InsertAsync(new ProductFeature(
                _guidGenerator.Create(),
                proProduct.Id,
                code,
                new LocalizedString(en, ar),
                Enums.FeatureType.Module
            ));
        }

        // Add quotas
        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            proProduct.Id,
            Enums.QuotaType.MaxUsers,
            100
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            proProduct.Id,
            Enums.QuotaType.MaxAssessments,
            50
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            proProduct.Id,
            Enums.QuotaType.MaxStorageGB,
            100
        ));

        // Add pricing plans
        var proMonthly = new PricingPlan(
            _guidGenerator.Create(),
            proProduct.Id,
            Enums.BillingPeriod.Monthly,
            2999
        );
        await _pricingPlanRepository.InsertAsync(proMonthly);

        var proAnnual = new PricingPlan(
            _guidGenerator.Create(),
            proProduct.Id,
            Enums.BillingPeriod.Annual,
            29999
        );
        await _pricingPlanRepository.InsertAsync(proAnnual);
    }

    private async Task SeedEnterpriseProductAsync()
    {
        var enterpriseProduct = new Product(
            Guid.Parse("00000000-0000-0000-0000-000000000004"),
            "Enterprise",
            new LocalizedString("Enterprise", "المؤسسات"),
            Enums.ProductCategory.Subscription
        );

        enterpriseProduct.UpdateDescription(new LocalizedString(
            "Complete solution with unlimited quotas and custom features",
            "حل كامل مع حصص غير محدودة وميزات مخصصة"
        ));
        enterpriseProduct.SetDisplayOrder(3);

        await _productRepository.InsertAsync(enterpriseProduct);

        // Add all features
        var features = new[]
        {
            ("FrameworkLibrary", "Framework Library", "مكتبة الإطار"),
            ("Assessment", "Assessment Management", "إدارة التقييمات"),
            ("Evidence", "Evidence Management", "إدارة الأدلة"),
            ("Dashboard", "Dashboard & Reports", "لوحة التحكم والتقارير"),
            ("Risk", "Risk Management", "إدارة المخاطر"),
            ("ActionPlan", "Action Plans", "خطط العمل"),
            ("Workflow", "Workflow Engine", "محرك سير العمل"),
            ("Audit", "Audit Management", "إدارة التدقيق"),
            ("Reporting", "Advanced Reporting", "التقارير المتقدمة"),
            ("AI", "AI Engine", "محرك الذكاء الاصطناعي"),
            ("Integration", "Integration Hub", "محور التكامل"),
            ("Policy", "Policy Management", "إدارة السياسات")
        };

        foreach (var (code, en, ar) in features)
        {
            await _featureRepository.InsertAsync(new ProductFeature(
                _guidGenerator.Create(),
                enterpriseProduct.Id,
                code,
                new LocalizedString(en, ar),
                Enums.FeatureType.Module
            ));
        }

        // Add unlimited quotas (null = unlimited)
        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            enterpriseProduct.Id,
            Enums.QuotaType.MaxUsers,
            null // Unlimited
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            enterpriseProduct.Id,
            Enums.QuotaType.MaxAssessments,
            null // Unlimited
        ));

        await _quotaRepository.InsertAsync(new ProductQuota(
            _guidGenerator.Create(),
            enterpriseProduct.Id,
            Enums.QuotaType.MaxStorageGB,
            null // Unlimited
        ));

        // Enterprise pricing is typically custom/contact sales
        // But we'll add a high base price
        var enterprisePlan = new PricingPlan(
            _guidGenerator.Create(),
            enterpriseProduct.Id,
            Enums.BillingPeriod.Annual,
            99999
        );
        await _pricingPlanRepository.InsertAsync(enterprisePlan);
    }
}

