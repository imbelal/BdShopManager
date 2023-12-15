using AutoMapper;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Queries
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetAllUsersQueryHandler(IReadOnlyApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IResponse<List<UserDto>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var userList = await _context.Users.Include(u => u.UserRole).ToListAsync();
            var results = _mapper.Map<List<Domain.Entities.User>, List<UserDto>>(userList);
            if (results.Count == 0)
                return Response.Fail<List<UserDto>>("No user found!!");

            return Response.Success(results);
        }
    }
}
