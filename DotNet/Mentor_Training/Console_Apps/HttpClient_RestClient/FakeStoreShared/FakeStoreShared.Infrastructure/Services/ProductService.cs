namespace FakeStoreShared.Infrastructure.Services;
public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public Task<List<Product>> GetAllProductsAsync()
    {
        return _productRepository.GetAllProductsAsync();
    }

    public Task<Product> GetProductByIdAsync(int id)
    {
        return _productRepository.GetProductByIdAsync(id);
    }

    public Task<Product> AddProductAsync(Product product)
    {
        return _productRepository.AddProductAsync(product);
    }

    public Task<Product> UpdateProductAsync(int id, Product product)
    {
        return _productRepository.UpdateProductAsync(id, product);
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        return _productRepository.DeleteProductAsync(id);
    }
}