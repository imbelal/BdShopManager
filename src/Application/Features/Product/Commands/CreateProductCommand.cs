using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommand : ICommand<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnit Unit { get; set; }
        public List<Guid> TagIds { get; set; }

        public CreateProductCommand(string title, string description, Guid categoryId, int stockQuantity, ProductUnit unit, List<Guid> tagIds)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            StockQuantity = stockQuantity;
            Unit = unit;
            TagIds = tagIds;
        }
    }
}
