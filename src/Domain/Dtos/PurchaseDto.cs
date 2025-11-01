using Domain.Enums;

namespace Domain.Dtos
{
    public class PurchaseDto
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Remark { get; set; }
        public PurchaseStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public List<PurchaseItemDto> PurchaseItems { get; set; } = new();
    }
}
