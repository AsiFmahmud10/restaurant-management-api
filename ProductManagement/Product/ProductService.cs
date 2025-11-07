using ProductManagement.Exception;
using ProductManagement.Product.Dto;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Product;

using Category;

public class ProductService(IProductRepository productRepository, ICategoryService categoryService) : IProductService
{
    public PaginationResult<ProductResponse> GetProductsByCategory(Guid categoryId,PageData pageData)
    {
        return productRepository.GetProductsByCategory(categoryId,pageData);
    }

    public void AddProduct(CreateProductRequest request)
    {
        var isProductExistBySameNameOrCode = productRepository.Exist(product =>
            product.Name.Equals(request.Name) || product.Code.Equals(request.Code));

        if (isProductExistBySameNameOrCode is true)
        {
            throw new BadRequestException("Product name and code required to be unique");
        }

        Category? existedCategory = categoryService.FindById(request.CategoryId);

        if (existedCategory is null)
        {
            throw new ResourceNotFoundException("Category not found");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            Rating = request.Rating,
            Stock = request.Stock,
            Tags = request.Tags,
            Category = existedCategory
        };

        productRepository.save(product);
    }

    public void UpdateProduct(Guid productId, UpdateProductRequest request)
    {
        var product = productRepository.FindById(productId) ?? throw new ResourceNotFoundException("Product not found");

        if (request.Name is not null)
        {
            if (productRepository.Exist(p => p.Name.Equals(request.Name)))
            {
                throw new BadRequestException("Product name already exists");
            }

            ;
            product.Name = request.Name;
        }

        if (request.Code is not null)
        {
            if (productRepository.Exist(p => p.Code.Equals(request.Code)))
            {
                throw new BadRequestException("Product code already exists");
            }

            ;
            product.Code = request.Code.Value;
        }

        if (request.Description is not null)
        {
            product.Description = request.Description;
        }

        if (request.Price.HasValue)
        {
            product.Price = request.Price.Value;
        }

        if (request.Quantity.HasValue)
        {
            product.Quantity = request.Quantity.Value;
        }

        if (request.Stock.HasValue)
        {
            product.Stock = request.Stock.Value;
        }

        if (request.Tags is not null)
        {
            product.Tags = request.Tags;
        }

        if (request.Rating is not null)
        {
            product.Rating = request.Rating.Value;
        }

        productRepository.Update(product);
    }
}