using Common.Repositories.Interfaces;

namespace Domain.Interfaces
{
    public interface IProductRepository : IRepository<Domain.Entities.Product>
    {
        Task<Domain.Entities.Product> GetByPhotoIdAsync(Guid photoId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Product> GetByIdWithPrimaryPhotoAsync(Guid Id, CancellationToken cancellationToken = default);
    }
}
