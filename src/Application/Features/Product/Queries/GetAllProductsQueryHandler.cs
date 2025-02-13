﻿using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Queries
{
    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, Pagination<ProductDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetAllProductsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.Products.Include(p => p.ProductTags).Select(p => new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Unit = p.Unit,
                CategoryId = p.CategoryId,
                CategoryName = _context.Categories.FirstOrDefault(c => c.Id.Equals(p.CategoryId)).Title,
                CreatedBy = p.CreatedBy,
                CreatedDate = p.CreatedUtcDate.ToString("F"),
                ProductTags = _context.Tags.Where(t => p.ProductTags.Select(x => x.TagId).Contains(t.Id)).Select(x => x.Title).ToList()
            });

            var pagination = await new Pagination<ProductDto>().CreateAsync(queryable, request.PageNumber, request.PageSize);
            return Response.Success(pagination);
        }
    }
}
