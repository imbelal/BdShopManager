using Common.Exceptions;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Common.Services.Interfaces;
using Domain.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expense.Commands
{
    public class MarkExpenseAsPaidCommandHandler : ICommandHandler<MarkExpenseAsPaidCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MarkExpenseAsPaidCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IResponse<Guid>> Handle(MarkExpenseAsPaidCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (expense == null)
            {
                throw new BusinessLogicException("Expense not found.");
            }

            if (!expense.CanBePaid())
            {
                throw new BusinessLogicException($"Expense cannot be marked as paid in {expense.Status} status.");
            }

            expense.MarkAsPaid();
            // UpdatedBy and UpdatedDate will be set automatically by AuditContextBase

            await _context.SaveChangesAsync(cancellationToken);

            return Response.Success<Guid>(expense.Id);
        }
    }
}