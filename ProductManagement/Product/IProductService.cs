using ProductManagement.Product.Dto;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Product;

public interface IProductService 
{
    void AddProduct(CreateProductRequest request);
    void UpdateProduct(Guid categoryId, UpdateProductRequest request);
    PaginationResult<ProductResponse> GetProductsByCategory(Guid categoryId,PageData pageData);
}