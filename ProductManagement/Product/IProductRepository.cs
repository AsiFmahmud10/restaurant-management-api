using ProductManagement.Db;
using ProductManagement.Product.Dto;
using ProductManagement.Services.Paging.Model;
namespace ProductManagement.Product;

public interface IProductRepository : IGenericDbOperation<Product>
{
  public PaginationResult<ProductResponse> GetProductsByCategory(Guid categoryId, PageData pageData);
}