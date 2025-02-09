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
        public List<Guid> TagIds { get; set; } = new List<Guid>();

        public CreateProductCommand(string title, string description, Guid categoryId, ProductUnit unit, List<Guid> tagIds)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Unit = unit;
            TagIds = tagIds ?? new List<Guid>();
        }
    }
}
