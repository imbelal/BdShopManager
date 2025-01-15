using Common.Repositories.Interfaces;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<List<Inventory>> GetByProductIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
    }
}
