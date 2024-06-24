using AutoMapper;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Queries
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetUserByIdQueryHandler(IReadOnlyApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Where(u => u.Id == request.Id).FirstOrDefaultAsync();
            var result = _mapper.Map<Domain.Entities.User, UserDto>(user);
            if (result == null)
            {
                return Response.Fail<UserDto>("No user found!!");
            }

            return Response.Success(result);
        }
    }
}
