using Common.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expense.Commands
{
    public class ApproveExpenseCommandHandler : ICommandHandler<ApproveExpenseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ApproveExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IResponse<Guid>> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (expense == null)
            {
                throw new BusinessLogicException("Expense not found.");
            }

            if (!expense.CanBeApproved())
            {
                throw new BusinessLogicException($"Expense cannot be approved in {expense.Status} status.");
            }

            string user = _currentUserService?.GetUser()?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value;

            if (!string.IsNullOrEmpty(user))
            {
                expense.Approve(user);
            }
            else
            {
                throw new BusinessLogicException("User not found.");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Response.Success<Guid>(expense.Id);
        }
    }
}