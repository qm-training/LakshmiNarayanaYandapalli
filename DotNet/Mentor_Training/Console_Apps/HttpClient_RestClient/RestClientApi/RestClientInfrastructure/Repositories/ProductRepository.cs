namespace RestClientInfrastructure.Repositories;

public class ProductRepository(RestClient restClient) : IProductRepository
{
    private readonly RestClient _client = restClient;

    public async Task<List<Product>> GetAllProductsAsync()
    {
        var request = new RestRequest("products", Method.Get);
        var response = await _client.ExecuteAsync<List<Product>>(request);
        return response.Data != null ? response.Data : new List<Product>();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var request = new RestRequest($"products/{id}", Method.Get);
        var response = await _client.ExecuteAsync<Product>(request);
        return response.Data;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        var request = new RestRequest("products", Method.Post);
        request.AddJsonBody(product);
        var response = await _client.ExecuteAsync<Product>(request);
        return response.Data;
    }

    public async Task<Product> UpdateProductAsync(int id, Product product)
    {
        var request = new RestRequest($"products/{id}", Method.Put);
        request.AddJsonBody(product);
        var response = await _client.ExecuteAsync<Product>(request);
        return response.Data;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var request = new RestRequest($"products/{id}", Method.Delete);
        var response = await _client.ExecuteAsync(request);
        return response.IsSuccessful;
    }
}
