namespace HttpClientInfrastructure.Repositories;
public class ProductRepository(HttpClient httpClient) : IProductRepository
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>("products");
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Product>($"products/{id}");
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("products", product);
        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<Product> UpdateProductAsync(int id, Product product)
    {
        var response = await _httpClient.PutAsJsonAsync($"products/{id}", product);
        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"products/{id}");
        return response.IsSuccessStatusCode;
    }
}
