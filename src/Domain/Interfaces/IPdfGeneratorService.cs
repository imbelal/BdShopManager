using Domain.Dtos;

namespace Domain.Interfaces
{
    public interface IPdfGeneratorService
    {
        byte[] GenerateOrderPdf(OrderDto order, string tenantName, string tenantAddress, string tenantPhone, string customerAddress, string customerPhone);
    }
}
