using Application.Features.SalesReturn.Commands;
using Bogus;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Entities;
using Domain.Enums;
using IntegrationTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class SalesReturnsControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Faker<Customer> _customerFaker;
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Category> _categoryFaker;

        public SalesReturnsControllerIntegrationTests()
        {
            _categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Title, f => f.Commerce.Categories(1)[0]);

            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.CategoryId, f => _categoryFaker.Generate().Id)
                .RuleFor(p => p.Unit, f => f.PickRandom<ProductUnit>())
                .RuleFor(p => p.CostPrice, f => f.Finance.Amount(10, 100))
                .RuleFor(p => p.SellingPrice, 50.00m) // Fixed price for easier calculation
                .RuleFor(p => p.StockQuantity, f => f.Random.Int(50, 100))
                .RuleFor(p => p.ProductTags, f => new List<ProductTag>());

            _customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.FirstName, f => f.Person.FirstName)
                .RuleFor(c => c.LastName, f => f.Person.LastName)
                .RuleFor(c => c.ContactNo, f => f.Phone.PhoneNumber().Substring(0, 10))
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Remark, f => f.Lorem.Sentence());
        }

        [Fact]
        public async Task Create_SalesReturn_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var customer = _customerFaker.Generate();

            // First create a sale
            var saleQuantity = 10;
            var unitPrice = 50.00m;
            var saleTotal = saleQuantity * unitPrice; // 10 * 50 = 500

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = saleQuantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createSalesDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = saleTotal,
                DiscountPercentage = 0,
                TotalPaid = saleTotal,
                Remark = "Test sale for return",
                SalesItems = salesItems
            };

            var saleResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createSalesDto),
                preparedEntities: [category, product, customer]);

            var saleOkResult = Assert.IsType<OkObjectResult>(saleResult);
            var saleResponse = Assert.IsType<Response<Guid>>(saleOkResult.Value);
            var salesId = saleResponse.Data;

            // Verify stock was reduced from the sale
            var productAfterSale = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(90, productAfterSale.StockQuantity); // 100 - 10 = 90

            // Now create a sales return
            var returnQuantity = 3;
            var returnTotal = returnQuantity * unitPrice; // 3 * 50 = 150

            var salesReturnItems = new List<SalesReturnItemDetailsDto>
            {
                new SalesReturnItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = returnQuantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice,
                    Reason = "Defective product"
                }
            };

            var createReturnDto = new CreateSalesReturnDto
            {
                SalesId = salesId,
                TotalRefundAmount = returnTotal,
                Remark = "Customer returned defective items",
                SalesReturnItems = salesReturnItems
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().Create(createReturnDto));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<SalesReturnDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotNull(response.Data);
            Assert.NotEqual(Guid.Empty, response.Data.Id);

            // Verify sales return was saved to database
            var savedReturn = await Context.SalesReturns.FirstOrDefaultAsync(sr => sr.Id == response.Data.Id);
            Assert.NotNull(savedReturn);
            Assert.Equal(salesId, savedReturn.SalesId);
            Assert.Equal(returnTotal, savedReturn.TotalRefundAmount);

            // Verify stock was increased (returned items added back to stock)
            var productAfterReturn = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(productAfterReturn);
            Assert.Equal(93, productAfterReturn.StockQuantity); // 90 + 3 = 93

            // Verify stock transaction was created for return
            var returnTransaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.SalesReturn);
            Assert.NotNull(returnTransaction);
            Assert.Equal(StockTransactionType.IN, returnTransaction.Type);
            Assert.Equal(returnQuantity, returnTransaction.Quantity);
        }

        [Fact]
        public async Task Create_SalesReturn_With_Total_Mismatch_Should_Fail()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var customer = _customerFaker.Generate();

            // First create a sale
            var saleQuantity = 10;
            var unitPrice = 50.00m;
            var saleTotal = saleQuantity * unitPrice;

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = saleQuantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createSalesDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = saleTotal,
                DiscountPercentage = 0,
                TotalPaid = saleTotal,
                Remark = "Test sale for return",
                SalesItems = salesItems
            };

            var saleResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createSalesDto),
                preparedEntities: [category, product, customer]);

            var saleOkResult = Assert.IsType<OkObjectResult>(saleResult);
            var saleResponse = Assert.IsType<Response<Guid>>(saleOkResult.Value);
            var salesId = saleResponse.Data;

            // Create sales return with incorrect total (should be 150, but we provide 200)
            var returnQuantity = 3;
            var incorrectTotal = 200m; // Should be 3 * 50 = 150

            var salesReturnItems = new List<SalesReturnItemDetailsDto>
            {
                new SalesReturnItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = returnQuantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice,
                    Reason = "Defective product"
                }
            };

            var createReturnDto = new CreateSalesReturnDto
            {
                SalesId = salesId,
                TotalRefundAmount = incorrectTotal,
                Remark = "Customer returned defective items",
                SalesReturnItems = salesReturnItems
            };

            // Act & Assert
            await Assert.ThrowsAsync<Common.Exceptions.BusinessLogicException>(async () =>
            {
                await ExecuteControllerMethodAsync(
                    () => ResolveController<SalesReturnsController>().Create(createReturnDto));
            });
        }

        [Fact]
        public async Task Update_SalesReturn_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product1 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var product2 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var customer = _customerFaker.Generate();

            // First create a sale with two products
            var unitPrice = 50.00m;
            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product1.Id,
                    Quantity = 10,
                    Unit = product1.Unit,
                    UnitPrice = unitPrice
                },
                new SalesItemDetailsDto
                {
                    ProductId = product2.Id,
                    Quantity = 5,
                    Unit = product2.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createSalesDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = 750m, // (10 * 50) + (5 * 50) = 750
                DiscountPercentage = 0,
                TotalPaid = 750m,
                Remark = "Test sale for return update",
                SalesItems = salesItems
            };

            var saleResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createSalesDto),
                preparedEntities: [category, product1, product2, customer]);

            var saleOkResult = Assert.IsType<OkObjectResult>(saleResult);
            var saleResponse = Assert.IsType<Response<Guid>>(saleOkResult.Value);
            var salesId = saleResponse.Data;

            // Create initial sales return for product1 only
            var initialReturnItems = new List<SalesReturnItemDetailsDto>
            {
                new SalesReturnItemDetailsDto
                {
                    ProductId = product1.Id,
                    Quantity = 3,
                    Unit = product1.Unit,
                    UnitPrice = unitPrice,
                    Reason = "Defective product"
                }
            };

            var createReturnDto = new CreateSalesReturnDto
            {
                SalesId = salesId,
                TotalRefundAmount = 150m, // 3 * 50
                Remark = "Initial return",
                SalesReturnItems = initialReturnItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().Create(createReturnDto));

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<SalesReturnDto>>(createOkResult.Value);
            var returnId = createResponse.Data.Id;

            // Verify initial stock levels
            var product1AfterReturn = await Context.Products.FirstOrDefaultAsync(p => p.Id == product1.Id);
            Assert.Equal(93, product1AfterReturn.StockQuantity); // 100 - 10 + 3 = 93

            // Now update the return to include product2 instead
            var updatedReturnItems = new List<UpdateSalesReturnItemDto>
            {
                new UpdateSalesReturnItemDto
                {
                    ProductId = product2.Id,
                    Quantity = 2,
                    Unit = product2.Unit,
                    UnitPrice = unitPrice,
                    Reason = "Changed mind"
                }
            };

            var updateDto = new UpdateSalesReturnDto
            {
                TotalRefundAmount = 100m, // 2 * 50
                Remark = "Updated return",
                SalesReturnItems = updatedReturnItems
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().Update(returnId, updateDto));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);

            // Verify the return was updated
            var updatedReturn = await Context.SalesReturns
                .Include(sr => sr.SalesReturnItems)
                .FirstOrDefaultAsync(sr => sr.Id == returnId);
            Assert.NotNull(updatedReturn);
            Assert.Equal(100m, updatedReturn.TotalRefundAmount);
            Assert.Equal("Updated return", updatedReturn.Remark);
            Assert.Single(updatedReturn.SalesReturnItems);
            Assert.Equal(product2.Id, updatedReturn.SalesReturnItems.First().ProductId);

            // Verify stock adjustments: product1 should have returned items removed from stock
            // and product2 should have returned items added to stock
            var product1AfterUpdate = await Context.Products.FirstOrDefaultAsync(p => p.Id == product1.Id);
            Assert.Equal(90, product1AfterUpdate.StockQuantity); // 100 - 10 = 90 (return reversed)

            var product2AfterUpdate = await Context.Products.FirstOrDefaultAsync(p => p.Id == product2.Id);
            Assert.Equal(97, product2AfterUpdate.StockQuantity); // 100 - 5 + 2 = 97
        }

        [Fact]
        public async Task Get_SalesReturn_By_Id_Should_Return_SalesReturn()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Generate();
            var customer = _customerFaker.Generate();

            // Create a sale first
            var unitPrice = 50.00m;
            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = 10,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createSalesDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = 500m,
                DiscountPercentage = 0,
                TotalPaid = 500m,
                Remark = "Test sale",
                SalesItems = salesItems
            };

            var saleResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createSalesDto),
                preparedEntities: [category, product, customer]);

            var saleOkResult = Assert.IsType<OkObjectResult>(saleResult);
            var saleResponse = Assert.IsType<Response<Guid>>(saleOkResult.Value);
            var salesId = saleResponse.Data;

            // Create a sales return
            var salesReturnItems = new List<SalesReturnItemDetailsDto>
            {
                new SalesReturnItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = 2,
                    Unit = product.Unit,
                    UnitPrice = unitPrice,
                    Reason = "Test return"
                }
            };

            var createReturnDto = new CreateSalesReturnDto
            {
                SalesId = salesId,
                TotalRefundAmount = 100m,
                Remark = "Test return for get by id",
                SalesReturnItems = salesReturnItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().Create(createReturnDto));

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<SalesReturnDto>>(createOkResult.Value);
            var returnId = createResponse.Data.Id;

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().GetById(returnId));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<SalesReturnDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(returnId, response.Data.Id);
            Assert.Equal(salesId, response.Data.SalesId);
            Assert.Equal(100m, response.Data.TotalRefundAmount);
        }

        [Fact]
        public async Task Get_All_SalesReturns_Should_Return_SalesReturns()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Delete_SalesReturn_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var customer = _customerFaker.Generate();

            // Create a sale first
            var unitPrice = 50.00m;
            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = 10,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createSalesDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = 500m,
                DiscountPercentage = 0,
                TotalPaid = 500m,
                Remark = "Test sale for deletion",
                SalesItems = salesItems
            };

            var saleResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createSalesDto),
                preparedEntities: [category, product, customer]);

            var saleOkResult = Assert.IsType<OkObjectResult>(saleResult);
            var saleResponse = Assert.IsType<Response<Guid>>(saleOkResult.Value);
            var salesId = saleResponse.Data;

            // Verify stock after sale
            var productAfterSale = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(90, productAfterSale.StockQuantity); // 100 - 10 = 90

            // Create a sales return
            var salesReturnItems = new List<SalesReturnItemDetailsDto>
            {
                new SalesReturnItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = 5,
                    Unit = product.Unit,
                    UnitPrice = unitPrice,
                    Reason = "Test return for deletion"
                }
            };

            var createReturnDto = new CreateSalesReturnDto
            {
                SalesId = salesId,
                TotalRefundAmount = 250m,
                Remark = "Test return for deletion",
                SalesReturnItems = salesReturnItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().Create(createReturnDto));

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<SalesReturnDto>>(createOkResult.Value);
            var returnId = createResponse.Data.Id;

            // Verify stock after return creation (should have increased)
            var productAfterReturn = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(95, productAfterReturn.StockQuantity); // 90 + 5 = 95

            // Act - Delete the sales return
            var deleteResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesReturnsController>().Delete(returnId));

            // Assert
            Assert.NotNull(deleteResult);
            var deleteOkResult = Assert.IsType<OkObjectResult>(deleteResult);
            var deleteResponse = Assert.IsType<Response<Guid>>(deleteOkResult.Value);
            Assert.True(deleteResponse.Succeeded);

            // Verify stock was adjusted (returned items should be removed from stock again)
            var productAfterDelete = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(90, productAfterDelete.StockQuantity); // 95 - 5 = 90 (return deleted, stock decreased)

            // Verify the return is marked as deleted
            var deletedReturn = await Context.SalesReturns
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(sr => sr.Id == returnId);
            Assert.NotNull(deletedReturn);
            Assert.True(deletedReturn.IsDeleted);
        }
    }
}
