using AutoMapper;
using Grc.Product.Products;
using Grc.Product.Subscriptions;

namespace Grc.Product;

public class GrcProductApplicationAutoMapperProfile : Profile
{
    public GrcProductApplicationAutoMapperProfile()
    {
        /* Create your AutoMapper object mappings here */

        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductDetailDto>();
        // CreateProductInput mapping is handled manually in ProductAppService
        // UpdateProductInput mapping is handled manually in ProductAppService

        // ProductFeature mappings
        CreateMap<ProductFeature, ProductFeatureDto>();

        // ProductQuota mappings
        CreateMap<ProductQuota, ProductQuotaDto>()
            .ForMember(dest => dest.IsUnlimited, opt => opt.MapFrom(src => src.IsUnlimited));

        // PricingPlan mappings
        CreateMap<PricingPlan, PricingPlanDto>();

        // Subscription mappings
        CreateMap<TenantSubscription, TenantSubscriptionDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired));
        CreateMap<TenantSubscription, SubscriptionDetailDto>()
            .IncludeBase<TenantSubscription, TenantSubscriptionDto>();

        // QuotaUsage mappings - handled in SubscriptionAppService with QuotaUsageInfo
    }
}

