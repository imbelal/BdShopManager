using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        private List<ProductTag> postTags = new();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Product()
        {
        }

        public Product(string title, string description, Guid categoryId, ProductUnit unit, List<Guid> tagIds)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Unit = unit;
            Status = ProductStatus.Active;
            AddTags(tagIds);
        }

        public IReadOnlyCollection<ProductTag> PostTags
        {
            get => postTags;
        }

        private void AddTags(List<Guid> tagIds)
        {
            postTags.AddRange(tagIds.Select(tagId => new ProductTag(this.Id, tagId)).ToList());
        }

        private void RemoveTags(List<Guid> tagIds)
        {
            postTags = postTags.Where(pt => !tagIds.Contains(pt.TagId)).ToList();
        }

        public void UpdateTags(List<Guid> tagIds)
        {
            List<Guid> existingTagIds = postTags.Select(pt => pt.TagId).ToList();
            List<Guid> tagsToBeAdded = tagIds.Except(existingTagIds).ToList();
            List<Guid> tagsToBeDeleted = existingTagIds.Except(tagIds).ToList();
            AddTags(tagsToBeAdded);
            RemoveTags(tagsToBeDeleted);
        }
    }
}
