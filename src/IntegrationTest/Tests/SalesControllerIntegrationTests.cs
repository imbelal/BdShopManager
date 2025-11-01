using Application.Features.Sales.Commands;
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
    public class SalesControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Faker<Customer> _customerFaker;
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Category> _categoryFaker;

        public SalesControllerIntegrationTests()
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
        public async Task Create_Sale_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var customer = _customerFaker.Generate();

            var quantity = 5;
            var unitPrice = 50.00m;
            var expectedTotal = quantity * unitPrice; // 5 * 50 = 250

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = expectedTotal,
                DiscountPercentage = 0,
                TotalPaid = expectedTotal,
                Remark = "Test sale",
                SalesItems = salesItems
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createDto),
                preparedEntities: [category, product, customer]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify sale was saved to database
            var savedSale = await Context.Sales.FirstOrDefaultAsync(s => s.Id == response.Data);
            Assert.NotNull(savedSale);
            Assert.Equal(customer.Id, savedSale.CustomerId);
            Assert.Equal(expectedTotal, savedSale.TotalPrice);

            // Verify stock was reduced
            var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(95, updatedProduct.StockQuantity); // 100 - 5 = 95

            // Verify stock transaction was created
            var stockTransaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.Sale);
            Assert.NotNull(stockTransaction);
            Assert.Equal(StockTransactionType.OUT, stockTransaction.Type);
            Assert.Equal(quantity, stockTransaction.Quantity);
        }

        [Fact]
        public async Task Create_Sale_With_Insufficient_Stock_Should_Fail()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 2).Generate(); // Only 2 in stock
            var customer = _customerFaker.Generate();

            var quantity = 5; // Trying to sell 5 when only 2 available
            var unitPrice = 50.00m;
            var expectedTotal = quantity * unitPrice;

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = expectedTotal,
                DiscountPercentage = 0,
                TotalPaid = expectedTotal,
                Remark = "Test sale with insufficient stock",
                SalesItems = salesItems
            };

            // Act & Assert
            await Assert.ThrowsAsync<Common.Exceptions.BusinessLogicException>(async () =>
            {
                await ExecuteControllerMethodAsync(
                    () => ResolveController<SalesController>().Create(createDto),
                    preparedEntities: [category, product, customer]
                );
            });
        }

        [Fact]
        public async Task Cancel_Sale_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 50).Generate();
            var customer = _customerFaker.Generate();

            // First create a sale
            var quantity = 10;
            var unitPrice = 50.00m;
            var expectedTotal = quantity * unitPrice; // 10 * 50 = 500

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = expectedTotal,
                DiscountPercentage = 0,
                TotalPaid = expectedTotal / 2, // Only pay half to allow cancellation
                Remark = "Test sale for cancellation",
                SalesItems = salesItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createDto),
                preparedEntities: [category, product, customer]);

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<Guid>>(createOkResult.Value);
            var saleId = createResponse.Data;

            // Verify stock was reduced from the sale
            var productAfterSale = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(40, productAfterSale.StockQuantity); // 50 - 10 = 40

            // Act - Cancel the sale
            var cancelResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Cancel(saleId));

            // Assert
            Assert.NotNull(cancelResult);
            var cancelOkResult = Assert.IsType<OkObjectResult>(cancelResult);
            var cancelResponse = Assert.IsType<Response<bool>>(cancelOkResult.Value);
            Assert.True(cancelResponse.Succeeded);

            // Verify stock was restored (should get 10 units back from cancellation)
            var productAfterCancel = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(50, productAfterCancel.StockQuantity); // 40 + 10 = 50 (restored)

            // Verify cancellation stock transaction was created
            var cancelTransaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.SalesCancellation);
            Assert.NotNull(cancelTransaction);
            Assert.Equal(StockTransactionType.IN, cancelTransaction.Type); // Should be IN to restore stock
            Assert.Equal(quantity, cancelTransaction.Quantity);
        }

        [Fact]
        public async Task Get_Sale_By_Id_Should_Return_Sale()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Generate();
            var customer = _customerFaker.Generate();

            var quantity = 2;
            var unitPrice = 50.00m;
            var expectedTotal = quantity * unitPrice; // 2 * 50 = 100

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = expectedTotal,
                DiscountPercentage = 0,
                TotalPaid = expectedTotal,
                Remark = "Test sale for get by id",
                SalesItems = salesItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createDto),
                preparedEntities: [category, product, customer]);

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<Guid>>(createOkResult.Value);
            var saleId = createResponse.Data;

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().GetById(saleId));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<SalesDto>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(saleId, response.Data.Id);
            Assert.Equal(customer.Id, response.Data.CustomerId);
        }

        [Fact]
        public async Task Get_All_Sales_Should_Return_Sales()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Delete_Sale_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 30).Generate();
            var customer = _customerFaker.Generate();

            var quantity = 5;
            var unitPrice = 50.00m;
            var expectedTotal = quantity * unitPrice; // 5 * 50 = 250

            var salesItems = new List<SalesItemDetailsDto>
            {
                new SalesItemDetailsDto
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Unit = product.Unit,
                    UnitPrice = unitPrice
                }
            };

            var createDto = new CreateSalesDto
            {
                CustomerId = customer.Id,
                TotalPrice = expectedTotal,
                DiscountPercentage = 0,
                TotalPaid = expectedTotal,
                Remark = "Test sale for deletion",
                SalesItems = salesItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Create(createDto),
                preparedEntities: [category, product, customer]);

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<Guid>>(createOkResult.Value);
            var saleId = createResponse.Data;

            // Verify stock was reduced from the sale
            var productAfterSale = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(25, productAfterSale.StockQuantity); // 30 - 5 = 25

            // Act - Delete the sale
            var deleteResult = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Delete(saleId));

            // Assert
            Assert.NotNull(deleteResult);
            var deleteOkResult = Assert.IsType<OkObjectResult>(deleteResult);
            var deleteResponse = Assert.IsType<Response<Guid>>(deleteOkResult.Value);
            Assert.True(deleteResponse.Succeeded);

            // Verify stock was restored (should get 5 units back from deletion)
            var productAfterDelete = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(30, productAfterDelete.StockQuantity); // 25 + 5 = 30 (restored)
        }

        [Fact]
        public async Task Update_Sale_With_Id_Mismatch_Should_Return_BadRequest()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var updateCommand = new UpdateSalesCommand
            {
                Id = differentId,
                DiscountPercentage = 10,
                Remark = "Updated sale"
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<SalesController>().Update(saleId, updateCommand));

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }
    }
}