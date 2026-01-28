namespace Application.Core;

public class PaginationParams<TCursor>
{
    private const int MaxPageSize = 50;
    public TCursor? Cursor { get; set; }

    public int PageSize
    {
        get;
        set => field = (value > MaxPageSize) ? MaxPageSize : value;
    }
}