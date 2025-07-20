using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Customer.Commands
{
    public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IResponse<Guid>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer customer = await _customerRepository.GetByIdAsync(command.Id, cancellationToken);
            if (customer == null) throw new Exception("Customer not found!!");

            customer.Update(command.FirstName, command.LastName, command.Address, command.ContactNo, command.Email, command.Remark);
            _customerRepository.Update(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(customer.Id);
        }
    }
}
