using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Customer.Commands
{
    public class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer customer = await _customerRepository.GetByIdAsync(command.Id, cancellationToken);
            if (customer == null) throw new Exception("Customer not found!!");

            _customerRepository.Remove(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(customer.Id);
        }
    }
}
