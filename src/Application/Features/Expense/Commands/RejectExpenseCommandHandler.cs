using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expense.Commands
{
    public class RejectExpenseCommandHandler : ICommandHandler<RejectExpenseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RejectExpenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IResponse<Guid>> Handle(RejectExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (expense == null)
            {
                return Response.Fail<Guid>("Expense not found.");
            }

            if (!expense.CanBeRejected())
            {
                return Response.Fail<Guid>($"Expense cannot be rejected in {expense.Status} status.");
            }

            string user = _currentUserService?.GetUser()?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value;

            if (!string.IsNullOrEmpty(user))
            {
                expense.Reject(user, request.RejectionReason);
            }
            else
            {
                return Response.Fail<Guid>("User not found.");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Response.Success<Guid>(expense.Id);
        }
    }
}