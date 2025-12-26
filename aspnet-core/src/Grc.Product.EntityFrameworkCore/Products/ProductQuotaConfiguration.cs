using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Grc.Product.Products;

namespace Grc.Product.EntityFrameworkCore.Products;

public class ProductQuotaConfiguration : IEntityTypeConfiguration<ProductQuota>
{
    public void Configure(EntityTypeBuilder<ProductQuota> builder)
    {
        builder.ToTable("product_quotas", "grc");
        builder.ConfigureByConvention();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.QuotaType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Limit)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.IsEnforced)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.QuotaType);

        builder.HasIndex(x => new { x.ProductId, x.QuotaType })
            .IsUnique();
    }
}


