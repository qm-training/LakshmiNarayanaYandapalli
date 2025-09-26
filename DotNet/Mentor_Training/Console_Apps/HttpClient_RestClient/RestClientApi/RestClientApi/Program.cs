namespace RestClientApi;
class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddScoped<RestClient>(_ => new RestClient("https://fakestoreapi.com/"));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ProductMenu>();

        var provider = services.BuildServiceProvider();

        var productMenu = provider.GetService<ProductMenu>();

        int choice;
        do
        {
            Console.WriteLine("==== FakeStore Menu ====");
            Console.WriteLine("1. Get All Products");
            Console.WriteLine("2. Get Product By Id");
            Console.WriteLine("3. Add Product");
            Console.WriteLine("4. Update Product");
            Console.WriteLine("5. Delete Product");
            Console.WriteLine("6. Exit");

            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1: await productMenu.GetAllProducts(); break;
                case 2: await productMenu.GetProductById(); break;
                case 3: await productMenu.AddProduct(); break;
                case 4: await productMenu.UpdateProduct(); break;
                case 5: await productMenu.DeleteProduct(); break;
                case 6: Console.WriteLine("Goodbye!"); break;
                default: Console.WriteLine("Invalid choice!"); break;
            }
        }
        while (choice != 6);
    }
}