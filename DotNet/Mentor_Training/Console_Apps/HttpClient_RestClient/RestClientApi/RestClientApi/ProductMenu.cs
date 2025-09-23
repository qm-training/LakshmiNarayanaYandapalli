namespace RestClientApi;

public class ProductMenu(IProductService productService)
{
    private readonly IProductService _productService = productService;

    public async Task GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        Console.WriteLine("--- Products ---");
        foreach (var p in products.Take(5))
        {
            Console.WriteLine($"{p.Id}: {p.Title} - ${p.Price}");
        }
    }

    public async Task GetProductById()
    {
        Console.Write("Enter Product Id: ");
        int id = Convert.ToInt32(Console.ReadLine());
        var product = await _productService.GetProductByIdAsync(id);
        Console.WriteLine(product != null
            ? $"{product.Id}: {product.Title} - ${product.Price}"
            : "Product not found");
    }

    public async Task AddProduct()
    {
        var newProduct = new Product
        {
            Title = "Hardcoded Gaming Mouse",
            Price = 59.99,
            Description = "RGB Gaming Mouse",
            Category = "electronics",
            Image = "https://example.com/mouse.jpg"
        };

        var added = await _productService.AddProductAsync(newProduct);
        Console.WriteLine($"Added Product: {added.Id}, {added.Title}");
    }

    public async Task UpdateProduct()
    {
        Console.Write("Enter Product Id to Update: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var updatedProduct = new Product
        {
            Title = "Updated Hardcoded Keyboard",
            Price = 99.99,
            Description = "Mechanical Keyboard",
            Category = "electronics",
            Image = "https://example.com/keyboard.jpg"
        };

        var result = await _productService.UpdateProductAsync(id, updatedProduct);
        Console.WriteLine(result != null ? $"Updated {result.Title}" : "Product not found");
    }

    public async Task DeleteProduct()
    {
        Console.Write("Enter Product Id to Delete: ");
        int id = Convert.ToInt32(Console.ReadLine());

        bool success = await _productService.DeleteProductAsync(id);
        Console.WriteLine(success ? "Product deleted" : "Delete failed");
    }
}
