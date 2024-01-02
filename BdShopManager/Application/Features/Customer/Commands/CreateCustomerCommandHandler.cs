using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Customer.Commands
{
    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer customer = new(command.FirstName, command.LastName, command.Address, command.ContactNo, command.Email, command.Remark);
            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(customer.Id);
        }
    }
}
