using Microsoft.EntityFrameworkCore;

namespace Common.Pagination
{
    public class Pagination<T>
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public List<T> Items { get; private set; }

        public Pagination()
        {

        }

        public Pagination(IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = queryable.Count();

            Items = queryable.Skip((PageNumber - 1) * PageSize)
                             .Take(PageSize)
                             .ToList();
        }

        public async Task<Pagination<T>> CreateAsync(IQueryable<T> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = await queryable.CountAsync(cancellationToken);

            Items = await queryable.Skip((PageNumber - 1) * PageSize)
                                   .Take(PageSize)
                                   .ToListAsync(cancellationToken);

            return this;
        }
    }
}
