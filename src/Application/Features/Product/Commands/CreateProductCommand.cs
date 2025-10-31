using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommand : ICommand<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal SellingPrice { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();

        public CreateProductCommand(string title, string description, Guid categoryId, ProductUnit unit, decimal sellingPrice, List<Guid> tagIds)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Unit = unit;
            SellingPrice = sellingPrice;
            TagIds = tagIds ?? new List<Guid>();
        }
    }
}
