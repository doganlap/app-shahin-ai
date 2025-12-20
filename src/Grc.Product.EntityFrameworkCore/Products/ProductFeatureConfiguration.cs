using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Grc.Product.Products;

namespace Grc.Product.EntityFrameworkCore.Products;

public class ProductFeatureConfiguration : IEntityTypeConfiguration<ProductFeature>
{
    public void Configure(EntityTypeBuilder<ProductFeature> builder)
    {
        builder.ToTable("product_features", "grc");
        builder.ConfigureByConvention();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.FeatureCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Grc.Domain.Shared.LocalizedString>(v, (System.Text.Json.JsonSerializerOptions)null))
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(
                v => v != null ? System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null) : null,
                v => v != null ? System.Text.Json.JsonSerializer.Deserialize<Grc.Domain.Shared.LocalizedString>(v, (System.Text.Json.JsonSerializerOptions)null) : null);

        builder.Property(x => x.FeatureType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Value)
            .HasMaxLength(200);

        builder.Property(x => x.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.FeatureCode);
    }
}


