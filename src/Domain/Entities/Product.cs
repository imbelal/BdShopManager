using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private List<ProductTag> productTags = new();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public ProductStatus Status { get; set; }
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
    }
}
