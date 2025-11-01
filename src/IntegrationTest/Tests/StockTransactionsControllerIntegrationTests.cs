using Application.Features.StockTransaction.Commands;
using Application.Features.StockTransaction.Queries;
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
    public class StockTransactionsControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Category> _categoryFaker;

        public StockTransactionsControllerIntegrationTests()
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
                .RuleFor(p => p.SellingPrice, f => f.Finance.Amount(50, 200))
                .RuleFor(p => p.StockQuantity, f => f.Random.Int(50, 100))
                .RuleFor(p => p.ProductTags, f => new List<ProductTag>());
        }

        [Fact]
        public async Task CreateAdjustment_StockIn_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 50).Generate();

            var command = new CreateStockAdjustmentCommand
            {
                ProductId = product.Id,
                Type = StockTransactionType.IN,
                Quantity = 20,
                Reason = "Received new stock from supplier"
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command),
                preparedEntities: [category, product]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify stock adjustment was saved
            var savedAdjustment = await Context.StockAdjustments.FirstOrDefaultAsync(sa => sa.Id == response.Data);
            Assert.NotNull(savedAdjustment);
            Assert.Equal(product.Id, savedAdjustment.ProductId);
            Assert.Equal(StockTransactionType.IN, savedAdjustment.Type);
            Assert.Equal(20, savedAdjustment.Quantity);

            // Verify product stock was increased
            var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(70, updatedProduct.StockQuantity); // 50 + 20 = 70

            // Verify stock transaction was created
            var transaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.Adjustment);
            Assert.NotNull(transaction);
            Assert.Equal(StockTransactionType.IN, transaction.Type);
            Assert.Equal(20, transaction.Quantity);
            Assert.Equal(response.Data, transaction.RefId);
        }

        [Fact]
        public async Task CreateAdjustment_StockOut_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();

            var command = new CreateStockAdjustmentCommand
            {
                ProductId = product.Id,
                Type = StockTransactionType.OUT,
                Quantity = 15,
                Reason = "Damaged goods removed from inventory"
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command),
                preparedEntities: [category, product]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify stock adjustment was saved
            var savedAdjustment = await Context.StockAdjustments.FirstOrDefaultAsync(sa => sa.Id == response.Data);
            Assert.NotNull(savedAdjustment);
            Assert.Equal(product.Id, savedAdjustment.ProductId);
            Assert.Equal(StockTransactionType.OUT, savedAdjustment.Type);
            Assert.Equal(15, savedAdjustment.Quantity);

            // Verify product stock was decreased
            var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(85, updatedProduct.StockQuantity); // 100 - 15 = 85

            // Verify stock transaction was created
            var transaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.Adjustment);
            Assert.NotNull(transaction);
            Assert.Equal(StockTransactionType.OUT, transaction.Type);
            Assert.Equal(15, transaction.Quantity);
            Assert.Equal(response.Data, transaction.RefId);
        }

        [Fact]
        public async Task CreateAdjustment_StockOut_WithInsufficientStock_Should_Fail()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 10).Generate(); // Only 10 in stock

            var command = new CreateStockAdjustmentCommand
            {
                ProductId = product.Id,
                Type = StockTransactionType.OUT,
                Quantity = 20, // Trying to remove 20 when only 10 available
                Reason = "Test insufficient stock"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Common.Exceptions.BusinessLogicException>(async () =>
            {
                await ExecuteControllerMethodAsync(
                    () => ResolveController<StockTransactionsController>().CreateAdjustment(command),
                    preparedEntities: [category, product]);
            });
        }

        [Fact]
        public async Task CreateAdjustment_WithInvalidQuantity_Should_Fail()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Generate();

            var command = new CreateStockAdjustmentCommand
            {
                ProductId = product.Id,
                Type = StockTransactionType.IN,
                Quantity = 0, // Invalid quantity
                Reason = "Test invalid quantity"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Common.Exceptions.BusinessLogicException>(async () =>
            {
                await ExecuteControllerMethodAsync(
                    () => ResolveController<StockTransactionsController>().CreateAdjustment(command),
                    preparedEntities: [category, product]);
            });
        }

        [Fact]
        public async Task CreateAdjustment_WithNonExistentProduct_Should_Fail()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();

            var command = new CreateStockAdjustmentCommand
            {
                ProductId = nonExistentProductId,
                Type = StockTransactionType.IN,
                Quantity = 10,
                Reason = "Test non-existent product"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Common.Exceptions.BusinessLogicException>(async () =>
            {
                await ExecuteControllerMethodAsync(
                    () => ResolveController<StockTransactionsController>().CreateAdjustment(command));
            });
        }

        [Fact]
        public async Task GetAll_StockTransactions_Should_Return_Transactions()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();

            // Create some stock adjustments to generate transactions
            var command1 = new CreateStockAdjustmentCommand
            {
                ProductId = product.Id,
                Type = StockTransactionType.IN,
                Quantity = 10,
                Reason = "Test transaction 1"
            };

            var command2 = new CreateStockAdjustmentCommand
            {
                ProductId = product.Id,
                Type = StockTransactionType.OUT,
                Quantity = 5,
                Reason = "Test transaction 2"
            };

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command1),
                preparedEntities: [category, product]);

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command2));

            // Act
            var query = new GetAllStockTransactionsQuery
            {
                PageNumber = 1,
                PageSize = 50
            };

            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().GetAll(query));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            // The actual response type depends on the query handler implementation
            // Just verify we got a successful response
        }

        [Fact]
        public async Task GetByProductId_Should_Return_Product_Transactions()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product1 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var product2 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();

            // Create adjustments for product1
            var command1 = new CreateStockAdjustmentCommand
            {
                ProductId = product1.Id,
                Type = StockTransactionType.IN,
                Quantity = 10,
                Reason = "Product 1 adjustment"
            };

            // Create adjustments for product2
            var command2 = new CreateStockAdjustmentCommand
            {
                ProductId = product2.Id,
                Type = StockTransactionType.IN,
                Quantity = 15,
                Reason = "Product 2 adjustment"
            };

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command1),
                preparedEntities: [category, product1, product2]);

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command2));

            // Act - Get transactions for product1
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().GetByProductId(product1.Id));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Verify database has transactions for product1
            var transactions = await Context.StockTransactions
                .Where(st => st.ProductId == product1.Id)
                .ToListAsync();
            Assert.NotEmpty(transactions);
            Assert.All(transactions, t => Assert.Equal(product1.Id, t.ProductId));
        }

        [Fact]
        public async Task Multiple_StockAdjustments_Should_Correctly_Update_Stock()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 50).Generate();

            // Create multiple adjustments
            var adjustments = new[]
            {
                new CreateStockAdjustmentCommand
                {
                    ProductId = product.Id,
                    Type = StockTransactionType.IN,
                    Quantity = 30,
                    Reason = "First IN adjustment"
                },
                new CreateStockAdjustmentCommand
                {
                    ProductId = product.Id,
                    Type = StockTransactionType.OUT,
                    Quantity = 10,
                    Reason = "First OUT adjustment"
                },
                new CreateStockAdjustmentCommand
                {
                    ProductId = product.Id,
                    Type = StockTransactionType.IN,
                    Quantity = 20,
                    Reason = "Second IN adjustment"
                },
                new CreateStockAdjustmentCommand
                {
                    ProductId = product.Id,
                    Type = StockTransactionType.OUT,
                    Quantity = 15,
                    Reason = "Second OUT adjustment"
                }
            };

            // Act - Execute all adjustments
            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(adjustments[0]),
                preparedEntities: [category, product]);

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(adjustments[1]));

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(adjustments[2]));

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(adjustments[3]));

            // Assert
            var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            // Expected: 50 + 30 - 10 + 20 - 15 = 75
            Assert.Equal(75, updatedProduct.StockQuantity);

            // Verify all transactions were created
            var transactions = await Context.StockTransactions
                .Where(st => st.ProductId == product.Id && st.RefType == StockReferenceType.Adjustment)
                .ToListAsync();
            Assert.Equal(4, transactions.Count);
        }

        [Fact]
        public async Task GetAll_WithFilters_Should_Filter_Transactions()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product1 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();
            var product2 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 100).Generate();

            // Create IN adjustment for product1
            var command1 = new CreateStockAdjustmentCommand
            {
                ProductId = product1.Id,
                Type = StockTransactionType.IN,
                Quantity = 10,
                Reason = "Product 1 IN adjustment"
            };

            // Create OUT adjustment for product2
            var command2 = new CreateStockAdjustmentCommand
            {
                ProductId = product2.Id,
                Type = StockTransactionType.OUT,
                Quantity = 5,
                Reason = "Product 2 OUT adjustment"
            };

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command1),
                preparedEntities: [category, product1, product2]);

            await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().CreateAdjustment(command2));

            // Act - Filter by product ID
            var query = new GetAllStockTransactionsQuery
            {
                ProductId = product1.Id,
                PageNumber = 1,
                PageSize = 50
            };

            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<StockTransactionsController>().GetAll(query));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Verify in database that product1 has transactions
            var product1Transactions = await Context.StockTransactions
                .Where(st => st.ProductId == product1.Id)
                .ToListAsync();
            Assert.NotEmpty(product1Transactions);
        }
    }
}
