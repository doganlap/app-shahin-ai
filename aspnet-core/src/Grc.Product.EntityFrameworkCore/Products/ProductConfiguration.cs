using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Grc.Product.Products;

namespace Grc.Product.EntityFrameworkCore.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "grc");
        builder.ConfigureByConvention();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Grc.Domain.Shared.LocalizedString>(v, (System.Text.Json.JsonSerializerOptions)null))
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(
                v => v != null ? System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null) : null,
                v => v != null ? System.Text.Json.JsonSerializer.Deserialize<Grc.Domain.Shared.LocalizedString>(v, (System.Text.Json.JsonSerializerOptions)null) : null);

        builder.Property(x => x.Category)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IconUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb");

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.IsActive);

        builder.HasMany(x => x.Features)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Quotas)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PricingPlans)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}


