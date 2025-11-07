namespace ProductManagement.Services.Paging.Model;

public class PaginationResult<T>
{
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public int PageNumber { get; set; }
    public int DataCount { get; set; }
    public List<T> Result { get; set; } = new List<T>();
}