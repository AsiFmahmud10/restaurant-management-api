namespace ProductManagement.Services.Paging.Model;

public class PageData(int pageNumber, int pageSize)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}