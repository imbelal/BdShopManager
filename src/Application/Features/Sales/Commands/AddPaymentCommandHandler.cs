using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Sales.Commands
{
    public class AddPaymentCommandHandler : ICommandHandler<AddPaymentCommand, bool>
    {
        private readonly ISalesRepository _salesRepository;

        public AddPaymentCommandHandler(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IResponse<bool>> Handle(AddPaymentCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Sales? sales = await _salesRepository.GetByIdAsync(command.SalesId, cancellationToken);
            if (sales == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales not found!");
            }

            // AddPayment will validate and update status automatically
            sales.AddPayment(command.Amount, command.PaymentMethod, command.Remark);

            _salesRepository.Update(sales);
            await _salesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
