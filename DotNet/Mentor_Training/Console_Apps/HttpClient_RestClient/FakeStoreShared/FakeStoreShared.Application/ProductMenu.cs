namespace FakeStoreShared.Application;
public class ProductMenu(IProductService productService)
{
    private readonly IProductService _productService = productService;

    public async Task GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        Console.WriteLine("--- Product List ---");
        foreach (var product in products.Take(5))
        {
            Console.WriteLine($"{product.Id}: {product.Title} - ${product.Price}");
        }
    }

    public async Task GetProductById()
    {
        Console.Write("Enter Product Id: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var product = await _productService.GetProductByIdAsync(id);
        if (product != null)
        {
            Console.WriteLine($"{product.Id}: {product.Title} - ${product.Price}");
        }
        else
        {
            Console.WriteLine("Product not found");
        }
    }

    public async Task AddProduct()
    {
        var newProduct = new Product
        {
            Title = "Gaming Laptop",
            Price = 1999.99,
            Description = "A powerful gaming laptop",
            Category = "electronics",
            Image = "https://example.com/laptop.jpg"
        };

        var added = await _productService.AddProductAsync(newProduct);
        Console.WriteLine($"Added Product -> Id: {added.Id}, Title: {added.Title}");
    }

    public async Task UpdateProduct()
    {
        Console.Write("Enter Product Id to Update: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var updatedProduct = new Product
        {
            Title = "Updated Laptop",
            Price = 1499.99,
            Description = "Updated specs and features",
            Category = "electronics",
            Image = "https://example.com/updated.jpg"
        };

        var result = await _productService.UpdateProductAsync(id, updatedProduct);
        if (result != null)
        {
            Console.WriteLine($"Updated -> Id: {result.Id}, Title: {result.Title}");
        }
        else
        {
            Console.WriteLine("Product not found");
        }
    }

    public async Task DeleteProduct()
    {
        Console.Write("Enter Product Id to Delete: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var success = await _productService.DeleteProductAsync(id);
        Console.WriteLine(success ? "Product deleted successfully" : "Product not found");
    }
}