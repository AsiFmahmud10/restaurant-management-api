using Microsoft.EntityFrameworkCore;
using ProductManagement.Db;
using ProductManagement.Product.Dto;
using ProductManagement.Services.Paging.Model;
using ProductManagement.User;

namespace ProductManagement.Product;

public class ProductRepository(ApplicationDbContext dbContext)
    : GenericDbOperation<Product>(dbContext), IProductRepository
{
    public PaginationResult<ProductResponse> GetProductsByCategory(Guid categoryId, PageData pageData)
    {
        var pageNumber = pageData.PageNumber;
        var pageSize = pageData.PageSize;

        var query = dbContext.Products
            .Include(p => p.Category)
            .Where(product => product.CategoryId.Equals(categoryId));

        var totalRow = query.Count();
        var totalPages = (int)Math.Ceiling(totalRow / (double)pageSize);

        var result = query
            .Select(product => new ProductResponse()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Stock = product.Stock,
                Quantity = product.Quantity,
                Rating = product.Rating,
                Code = product.Code,
            })
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();

        return new PaginationResult<ProductResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPage = totalPages,
            Result = result,
            DataCount = result.Count()
        };
    }
}