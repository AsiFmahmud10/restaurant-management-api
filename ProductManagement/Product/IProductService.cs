using System.Linq.Expressions;
using ProductManagement.Product.Dto;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Product;

public interface IProductService 
{
    void AddProduct(CreateProductRequest request);
    void UpdateProduct(Guid categoryId, UpdateProductRequest request);
    Product? FindByProductId(Guid productId,params Expression<Func<Product,object?>>[] includes);
    PaginationResult<ProductResponse> GetProductsByCategory(Guid categoryId,PageData pageData);
}