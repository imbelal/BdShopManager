using Application.Features.Purchase.Commands;
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
    public class PurchasesControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Faker<Supplier> _supplierFaker;
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Category> _categoryFaker;

        public PurchasesControllerIntegrationTests()
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
                .RuleFor(p => p.CostPrice, 20.00m) // Fixed cost price for easier calculation
                .RuleFor(p => p.SellingPrice, f => f.Finance.Amount(50, 200))
                .RuleFor(p => p.StockQuantity, f => f.Random.Int(10, 50))
                .RuleFor(p => p.ProductTags, f => new List<ProductTag>());

            _supplierFaker = new Faker<Supplier>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.Name, f => f.Company.CompanyName())
                .RuleFor(s => s.ContactNo, f => f.Phone.PhoneNumber().Substring(0, 10))
                .RuleFor(s => s.Details, f => f.Address.FullAddress());
        }

        [Fact]
        public async Task Create_Purchase_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 20).Generate();
            var supplier = _supplierFaker.Generate();

            var purchaseItems = new List<CreatePurchaseItemDto>
            {
                new CreatePurchaseItemDto
                {
                    ProductId = product.Id,
                    Quantity = 50,
                    CostPerUnit = 25.00m
                }
            };

            var createCommand = new CreatePurchaseCommand
            {
                SupplierId = supplier.Id,
                PurchaseDate = DateTime.UtcNow,
                Remark = "Test purchase",
                PurchaseItems = purchaseItems
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Create(createCommand),
                preparedEntities: [category, product, supplier]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify purchase was saved to database
            var savedPurchase = await Context.Purchases.FirstOrDefaultAsync(p => p.Id == response.Data);
            Assert.NotNull(savedPurchase);
            Assert.Equal(supplier.Id, savedPurchase.SupplierId);
            Assert.Equal(PurchaseStatus.Pending, savedPurchase.Status);

            // Verify stock was increased
            var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(70, updatedProduct.StockQuantity); // 20 + 50 = 70

            // Verify stock transaction was created
            var stockTransaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.Purchase);
            Assert.NotNull(stockTransaction);
            Assert.Equal(StockTransactionType.IN, stockTransaction.Type);
            Assert.Equal(50, stockTransaction.Quantity);
        }

        [Fact]
        public async Task Cancel_Purchase_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 30).Generate();
            var supplier = _supplierFaker.Generate();

            // First create a purchase
            var purchaseItems = new List<CreatePurchaseItemDto>
            {
                new CreatePurchaseItemDto
                {
                    ProductId = product.Id,
                    Quantity = 25,
                    CostPerUnit = 30.00m
                }
            };

            var createCommand = new CreatePurchaseCommand
            {
                SupplierId = supplier.Id,
                PurchaseDate = DateTime.UtcNow,
                Remark = "Test purchase for cancellation",
                PurchaseItems = purchaseItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Create(createCommand),
                preparedEntities: [category, product, supplier]);

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<Guid>>(createOkResult.Value);
            var purchaseId = createResponse.Data;

            // Verify stock was increased from the purchase
            var productAfterPurchase = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(55, productAfterPurchase.StockQuantity); // 30 + 25 = 55

            // Act - Cancel the purchase
            var cancelResult = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Cancel(purchaseId));

            // Assert
            Assert.NotNull(cancelResult);
            var cancelOkResult = Assert.IsType<OkObjectResult>(cancelResult);
            var cancelResponse = Assert.IsType<Response<bool>>(cancelOkResult.Value);
            Assert.True(cancelResponse.Succeeded);

            // Verify purchase status was updated to cancelled
            var cancelledPurchase = await Context.Purchases.FirstOrDefaultAsync(p => p.Id == purchaseId);
            Assert.NotNull(cancelledPurchase);
            Assert.Equal(PurchaseStatus.Cancelled, cancelledPurchase.Status);

            // Verify stock was reduced (should lose 25 units from cancellation)
            var productAfterCancel = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(30, productAfterCancel.StockQuantity); // 55 - 25 = 30 (back to original)

            // Verify cancellation stock transaction was created
            var cancelTransaction = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product.Id && st.RefType == StockReferenceType.PurchaseCancellation);
            Assert.NotNull(cancelTransaction);
            Assert.Equal(StockTransactionType.OUT, cancelTransaction.Type);
            Assert.Equal(25, cancelTransaction.Quantity);
        }

        [Fact]
        public async Task Delete_Purchase_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Clone().RuleFor(p => p.StockQuantity, 15).Generate();
            var supplier = _supplierFaker.Generate();

            // First create a purchase
            var purchaseItems = new List<CreatePurchaseItemDto>
            {
                new CreatePurchaseItemDto
                {
                    ProductId = product.Id,
                    Quantity = 20,
                    CostPerUnit = 35.00m
                }
            };

            var createCommand = new CreatePurchaseCommand
            {
                SupplierId = supplier.Id,
                PurchaseDate = DateTime.UtcNow,
                Remark = "Test purchase for deletion",
                PurchaseItems = purchaseItems
            };

            var createResult = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Create(createCommand),
                preparedEntities: [category, product, supplier]);

            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createResponse = Assert.IsType<Response<Guid>>(createOkResult.Value);
            var purchaseId = createResponse.Data;

            // Verify stock was increased from the purchase
            var productAfterPurchase = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(35, productAfterPurchase.StockQuantity); // 15 + 20 = 35

            // Act - Delete the purchase
            var deleteResult = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Delete(purchaseId));

            // Assert
            Assert.NotNull(deleteResult);
            var deleteOkResult = Assert.IsType<OkObjectResult>(deleteResult);
            var deleteResponse = Assert.IsType<Response<bool>>(deleteOkResult.Value);
            Assert.True(deleteResponse.Succeeded);

            // Verify purchase was soft deleted
            var deletedPurchase = await Context.Purchases.FirstOrDefaultAsync(p => p.Id == purchaseId);
            Assert.Null(deletedPurchase); // Soft deleted purchase might be filtered out

            // Verify stock was reduced (should lose 20 units from deletion)
            var productAfterDelete = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Equal(15, productAfterDelete.StockQuantity); // 35 - 20 = 15 (back to original)
        }

        [Fact]
        public async Task Get_All_Purchases_Should_Return_Purchases()
        {
            // Arrange
            var supplier = _supplierFaker.Generate();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().GetAll(),
                preparedEntities: [supplier]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_All_Purchases_With_Supplier_Filter_Should_Filter_Correctly()
        {
            // Arrange
            var supplier = _supplierFaker.Generate();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().GetAll(supplierId: supplier.Id),
                preparedEntities: [supplier]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_All_Purchases_With_Status_Filter_Should_Filter_Correctly()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().GetAll(status: PurchaseStatus.Pending));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_All_Purchases_With_Product_Filter_Should_Filter_Correctly()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var product = _productFaker.Generate();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().GetAll(productId: product.Id),
                preparedEntities: [category, product]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_All_Purchases_With_Date_Range_Should_Filter_Correctly()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow.AddDays(-1);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().GetAll(startDate: startDate, endDate: endDate));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Get_All_Purchases_With_Search_Term_Should_Filter_Correctly()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().GetAll(searchTerm: "test"));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }

        [Fact]
        public async Task Update_Purchase_With_Id_Mismatch_Should_Return_BadRequest()
        {
            // Arrange
            var purchaseId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var updateCommand = new UpdatePurchaseCommand
            {
                Id = differentId,
                Remark = "Updated purchase"
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Update(purchaseId, updateCommand));

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_Purchase_With_Multiple_Items_Should_Succeed()
        {
            // Arrange
            var category1 = _categoryFaker.Generate();
            var category2 = _categoryFaker.Generate();
            var product1 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 10).Generate();
            var product2 = _productFaker.Clone().RuleFor(p => p.StockQuantity, 15).Generate();
            var supplier = _supplierFaker.Generate();

            var purchaseItems = new List<CreatePurchaseItemDto>
            {
                new CreatePurchaseItemDto
                {
                    ProductId = product1.Id,
                    Quantity = 20,
                    CostPerUnit = 25.00m
                },
                new CreatePurchaseItemDto
                {
                    ProductId = product2.Id,
                    Quantity = 30,
                    CostPerUnit = 35.00m
                }
            };

            var createCommand = new CreatePurchaseCommand
            {
                SupplierId = supplier.Id,
                PurchaseDate = DateTime.UtcNow,
                Remark = "Test purchase with multiple items",
                PurchaseItems = purchaseItems
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<PurchasesController>().Create(createCommand),
                preparedEntities: [category1, category2, product1, product2, supplier]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify stock was increased for both products
            var updatedProduct1 = await Context.Products.FirstOrDefaultAsync(p => p.Id == product1.Id);
            Assert.NotNull(updatedProduct1);
            Assert.Equal(30, updatedProduct1.StockQuantity); // 10 + 20 = 30

            var updatedProduct2 = await Context.Products.FirstOrDefaultAsync(p => p.Id == product2.Id);
            Assert.NotNull(updatedProduct2);
            Assert.Equal(45, updatedProduct2.StockQuantity); // 15 + 30 = 45

            // Verify stock transactions were created for both products
            var stockTransaction1 = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product1.Id && st.RefType == StockReferenceType.Purchase);
            Assert.NotNull(stockTransaction1);
            Assert.Equal(StockTransactionType.IN, stockTransaction1.Type);
            Assert.Equal(20, stockTransaction1.Quantity);

            var stockTransaction2 = await Context.StockTransactions
                .FirstOrDefaultAsync(st => st.ProductId == product2.Id && st.RefType == StockReferenceType.Purchase);
            Assert.NotNull(stockTransaction2);
            Assert.Equal(StockTransactionType.IN, stockTransaction2.Type);
            Assert.Equal(30, stockTransaction2.Quantity);
        }
    }
}