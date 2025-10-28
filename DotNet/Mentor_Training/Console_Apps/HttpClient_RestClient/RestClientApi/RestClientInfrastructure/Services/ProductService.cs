namespace RestClientInfrastructure.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _repository = productRepository;

    public Task<List<Product>> GetAllProductsAsync()
    {
        return _repository.GetAllProductsAsync();
    }
    public Task<Product> GetProductByIdAsync(int id)
    {
        return _repository.GetProductByIdAsync(id);
    }
    public Task<Product> AddProductAsync(Product product)
    {
        return _repository.AddProductAsync(product);
    }
    public Task<Product> UpdateProductAsync(int id, Product product)
    {
        return _repository.UpdateProductAsync(id, product);
    }
    public Task<bool> DeleteProductAsync(int id)
    {
        return _repository.DeleteProductAsync(id);
    }
}
