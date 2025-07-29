using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private List<ProductTag> productTags = new();
        private List<ProductPhoto> productPhotos = new();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public ProductStatus Status { get; set; }
        public int StockQuantity { get; private set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        public Product() : base()
        {

        }

        public Product(string title, string description, Guid categoryId, ProductUnit unit, List<Guid> tagIds) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Unit = unit;
            Status = ProductStatus.Active;
            AddTags(tagIds);
        }

        public IReadOnlyCollection<ProductTag> ProductTags
        {
            get => productTags;
        }

        public IReadOnlyCollection<ProductPhoto> ProductPhotos
        {
            get => productPhotos;
        }

        private void AddTags(List<Guid> tagIds)
        {
            productTags.AddRange(tagIds.Select(tagId => new ProductTag(this.Id, tagId)).ToList());
        }

        private void RemoveTags(List<Guid> tagIds)
        {
            productTags = productTags.Where(pt => !tagIds.Contains(pt.TagId)).ToList();
        }

        public void UpdateTags(List<Guid> tagIds)
        {
            List<Guid> existingTagIds = productTags.Select(pt => pt.TagId).ToList();
            List<Guid> tagsToBeAdded = tagIds.Except(existingTagIds).ToList();
            List<Guid> tagsToBeDeleted = existingTagIds.Except(tagIds).ToList();
            AddTags(tagsToBeAdded);
            RemoveTags(tagsToBeDeleted);
        }

        public void AddPhoto(ProductPhoto photo)
        {
            productPhotos.Add(photo);
        }

        public void RemovePhoto(Guid photoId)
        {
            var photo = productPhotos.FirstOrDefault(p => p.Id == photoId);
            if (photo != null)
            {
                productPhotos.Remove(photo);
            }
        }

        public void SetPrimaryPhoto(Guid photoId)
        {
            // Remove primary flag from all photos
            foreach (var photo in productPhotos)
            {
                if (photo.IsPrimary)
                {
                    photo.SetAsPrimary(false);
                }
            }

            // Set the specified photo as primary
            var targetPhoto = productPhotos.FirstOrDefault(p => p.Id == photoId);
            if (targetPhoto != null)
            {
                targetPhoto.SetAsPrimary(true);
            }
        }

        public void IncreaseStockQuantity(int quantity)
        {
            if (quantity > 0)
            {
                StockQuantity += quantity;
            }
        }

        public void DecreaseStockQuantity(int quantity)
        {
            if (quantity > 0 && StockQuantity >= quantity)
            {
                StockQuantity -= quantity;
            }
        }
    }
}
