namespace UserManagerApi.DTO;

public class PagedResultDto<T>
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Products { get; set; } = new();
}
