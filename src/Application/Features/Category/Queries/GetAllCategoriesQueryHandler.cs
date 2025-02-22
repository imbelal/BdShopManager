using Common.Cache;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Category.Queries
{
    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, List<Domain.Entities.Category>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly string _cacheKey;
        public GetAllCategoriesQueryHandler(IReadOnlyApplicationDbContext context,
            ICacheService cacheService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _cacheService = cacheService;
            string tenantId = currentUserService?.GetUser()?.Claims?.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            _cacheKey = $"allCategory_{tenantId}";

        }

        public async Task<IResponse<List<Domain.Entities.Category>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Category> categories = new();
            bool cacheFound = _cacheService.TryGet<List<Domain.Entities.Category>>(_cacheKey, out categories);

            if (cacheFound)
            {
                return Response.Success(categories);
            }

            categories = await _context.Categories.OrderByDescending(c => c.CreatedUtcDate).ToListAsync(cancellationToken);
            if (categories.Count == 0)
                return Response.Fail<List<Domain.Entities.Category>>("No Category found!!");
            _cacheService.Set<List<Domain.Entities.Category>>(_cacheKey, categories, TimeSpan.FromDays(1));

            return Response.Success(categories);
        }
    }
}
