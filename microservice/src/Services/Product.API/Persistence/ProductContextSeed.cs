using Product.API.Entities;
using ILogger = Serilog.ILogger;
namespace Product.API.Persistence;

public class ProductContextSeed
{
    public static async Task SeedProductAsync(ProductContext productContext, ILogger logger)
    {
        if (!productContext.Products.Any())
        {
            await productContext.AddRangeAsync(getCatalogProducts());
            await productContext.SaveChangesAsync();
            logger.Information("Seeded data for product db associated with context {DbContextName}",
                nameof(productContext));
        }
    }

    private static IEnumerable<CatalogProduct> getCatalogProducts()
    {
        return new List<CatalogProduct>
        {
            new()
            {
                No = "Lotus",
                Name = "Esprit",
                Summary = "NonDisplaced fracture of greateer trochanter of right femur",
                Description = "NonDisplaced fracture of greateer trochanter of right femur",
                Price = (decimal)177940.90
            },
            new()
            {
                No = "cadilac",
                Name = "CTS",
                Summary = "Carbuncle of trunk",
                Description = "Carbuncle of trunk",
                Price = (decimal)177940.90
            }
        };
    }
}