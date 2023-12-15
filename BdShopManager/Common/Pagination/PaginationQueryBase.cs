namespace Common.Pagination
{
    public abstract class PaginationQueryBase
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
