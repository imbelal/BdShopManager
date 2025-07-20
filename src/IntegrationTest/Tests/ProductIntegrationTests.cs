using Application.Features.Product.Commands;
using Bogus;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Entities;
using Domain.Enums;
using IntegrationTest.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class ProductIntegrationTests : IntegrationTestBase
    {
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Category> _categoryFaker;
        private readonly Faker<Supplier> _supplierFaker;

        public ProductIntegrationTests()
        {
            // Create fakers for test data
            _categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Title, f => f.Commerce.Categories(1)[0]);

            _supplierFaker = new Faker<Supplier>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.Name, f => f.Company.CompanyName())
                .RuleFor(s => s.ContactNo, f => f.Phone.PhoneNumber().Substring(0, 10))
                .RuleFor(s => s.Details, f => f.Address.FullAddress());

            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.CategoryId, f => _categoryFaker.Generate().Id)
                .RuleFor(p => p.Unit, f => f.PickRandom<ProductUnit>())
                .RuleFor(p => p.ProductTags, f => new List<ProductTag>());
        }

        [Fact]
        public async Task Create_Product_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var supplier = _supplierFaker.Generate();

            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                category.Id,
                ProductUnit.Piece,
                new List<Guid>());

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Create(command),
                preparedEntities: [category, supplier]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify product was saved to database
            var savedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == response.Data);
            Assert.NotNull(savedProduct);
            Assert.Equal("Test Product", savedProduct.Title);
            Assert.Equal("Test Description", savedProduct.Description);
        }

        [Fact]
        public async Task Create_Product_With_Invalid_Category_Should_Fail()
        {
            // Arrange
            var command = new CreateProductCommand(
                "Test Product",
                "Test Description",
                Guid.NewGuid(), // Non-existent category
                ProductUnit.Piece,
                new List<Guid>());

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Create(command)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Category not found", response.Errors);
        }

        [Fact]
        public async Task Update_Product_Should_Succeed()
        {
            // Arrange
            var category = _categoryFaker.Generate();
            var supplier = _supplierFaker.Generate();
            var product = _productFaker.Generate();

            var command = new UpdateProductCommand(
                product.Id,
                "Updated Product Name",
                "Updated Description",
                category.Id);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Update(command),
                preparedEntities: [category, supplier, product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(product.Id, response.Data);

            // Verify product was updated in database
            var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product Name", updatedProduct.Title);
            Assert.Equal("Updated Description", updatedProduct.Description);
        }

        [Fact]
        public async Task Update_NonExistent_Product_Should_Fail()
        {
            // Arrange
            var category = _categoryFaker.Generate();

            var command = new UpdateProductCommand(
                Guid.NewGuid(), // Non-existent product
                "Updated Product Name",
                "Updated Description",
                category.Id);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Update(command),
                preparedEntities: [category]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Product not found", response.Errors);
        }

        [Fact]
        public async Task Delete_Product_Should_Succeed()
        {
            // Arrange
            var product = _productFaker.Generate();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Delete(product.Id),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(product.Id, response.Data);

            // Verify product was deleted from database
            var deletedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task Delete_NonExistent_Product_Should_Fail()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().Delete(nonExistentProductId)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Product not found", response.Errors);
        }

        [Fact]
        public async Task GetProducts_With_Paging_Should_Succeed()
        {
            // Arrange
            var preparedEntities = new List<object>();

            var category = _categoryFaker.Generate();
            preparedEntities.Add(category);

            var products = _productFaker.Generate(5);
            foreach (var product in products)
            {
                product.CategoryId = category.Id;
                preparedEntities.Add(product);
            }

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().GetProducts(3, 1),
                preparedEntities: preparedEntities
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async Task GetProduct_ById_Should_Succeed()
        {
            // Arrange
            var product = _productFaker.Generate();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().GetById(product.Id),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Product>>(okResult.Value);
            Assert.True(response.Succeeded);
        }

        [Fact]
        public async Task GetProduct_By_NonExistent_Id_Should_Fail()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().GetById(nonExistentProductId)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Product>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("No Product found!!", response.Errors);
        }

        [Fact]
        public async Task Upload_Product_Photo_Should_Succeed()
        {
            // Arrange
            var product = _productFaker.Generate();

            // Create a mock file
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test-image.jpg");
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().UploadPhoto(product.Id, mockFile.Object, true, 1),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify file storage service was called
            Factory.MockFileStorageService.Verify(
                x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify photo was saved to database
            var savedPhoto = await Context.ProductPhotos.FirstOrDefaultAsync(p => p.Id == response.Data);
            Assert.NotNull(savedPhoto);
            Assert.Equal(product.Id, savedPhoto.ProductId);
            Assert.True(savedPhoto.IsPrimary);
            Assert.Equal(1, savedPhoto.DisplayOrder);
        }

        [Fact]
        public async Task Upload_Product_Photo_With_Invalid_File_Type_Should_Fail()
        {
            // Arrange
            var product = _productFaker.Generate();

            // Create a mock file with invalid type
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test-file.txt");
            mockFile.Setup(f => f.ContentType).Returns("text/plain");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().UploadPhoto(product.Id, mockFile.Object, false, 1),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Invalid file type. Only image files are allowed.", response.Errors);
        }

        [Fact]
        public async Task Upload_Product_Photo_To_NonExistent_Product_Should_Fail()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();

            // Create a mock file
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test-image.jpg");
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().UploadPhoto(nonExistentProductId, mockFile.Object, false, 1)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Product not found", response.Errors);
        }

        [Fact]
        public async Task Get_Product_Photos_Should_Succeed()
        {
            // Arrange
            var product = _productFaker.Generate();
            var photo1 = new ProductPhoto(product.Id, "photo1.jpg", "Photo 1", "image/jpeg", 1024, "https://test-url1.com", true, 1);
            var photo2 = new ProductPhoto(product.Id, "photo2.jpg", "Photo 2", "image/jpeg", 2048, "https://test-url2.com", false, 2);

            product.AddPhoto(photo1);
            product.AddPhoto(photo2);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().GetProductPhotos(product.Id),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<ProductPhotoDto>>>(okResult.Value);
            Assert.True(response.Succeeded);
        }

        [Fact]
        public async Task Get_Product_Photos_For_NonExistent_Product_Should_Fail()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().GetProductPhotos(nonExistentProductId)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<List<ProductPhotoDto>>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Product not found!!", response.Errors);
        }

        [Fact]
        public async Task Delete_Product_Photo_Should_Succeed()
        {
            // Arrange
            var product = _productFaker.Generate();
            var photo = new ProductPhoto(product.Id, "photo-to-delete.jpg", "Photo to Delete", "image/jpeg", 1024, "https://test-url.com", false, 1);
            product.AddPhoto(photo);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().DeletePhoto(photo.Id),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(photo.Id, response.Data);

            // Verify file storage service was called to delete the file
            Factory.MockFileStorageService.Verify(
                x => x.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify photo was deleted from database
            var deletedPhoto = await Context.ProductPhotos.FirstOrDefaultAsync(p => p.Id == photo.Id);
            Assert.Null(deletedPhoto);
        }

        [Fact]
        public async Task Delete_NonExistent_Product_Photo_Should_Fail()
        {
            // Arrange
            var nonExistentPhotoId = Guid.NewGuid();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().DeletePhoto(nonExistentPhotoId)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Product photo not found", response.Errors);
        }

        [Fact]
        public async Task Set_Primary_Product_Photo_Should_Succeed()
        {
            // Arrange
            var product = _productFaker.Generate();
            var photo1 = new ProductPhoto(product.Id, "photo1.jpg", "Photo 1", "image/jpeg", 1024, "https://test-url1.com", true, 1);
            var photo2 = new ProductPhoto(product.Id, "photo2.jpg", "Photo 2", "image/jpeg", 2048, "https://test-url2.com", false, 2);

            product.AddPhoto(photo1);
            product.AddPhoto(photo2);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().SetPrimaryPhoto(photo2.Id),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.Equal(photo2.Id, response.Data);

            // Verify photo2 is now primary and photo1 is not
            var updatedPhoto1 = await Context.ProductPhotos.FirstOrDefaultAsync(p => p.Id == photo1.Id);
            var updatedPhoto2 = await Context.ProductPhotos.FirstOrDefaultAsync(p => p.Id == photo2.Id);

            Assert.NotNull(updatedPhoto1);
            Assert.NotNull(updatedPhoto2);
            Assert.False(updatedPhoto1.IsPrimary);
            Assert.True(updatedPhoto2.IsPrimary);
        }

        [Fact]
        public async Task Set_Primary_NonExistent_Product_Photo_Should_Fail()
        {
            // Arrange
            var nonExistentPhotoId = Guid.NewGuid();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().SetPrimaryPhoto(nonExistentPhotoId)
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("Product photo not found", response.Errors);
        }

        [Fact]
        public async Task Upload_Product_Photo_With_Null_File_Should_Fail()
        {
            // Arrange
            var product = _productFaker.Generate();

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().UploadPhoto(product.Id, null, false, 1),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("No file provided", response.Errors);
        }

        [Fact]
        public async Task Upload_Product_Photo_With_Empty_File_Should_Fail()
        {
            // Arrange
            var product = _productFaker.Generate();

            // Create a mock file with zero length
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test-image.jpg");
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.Length).Returns(0);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<ProductsController>().UploadPhoto(product.Id, mockFile.Object, false, 1),
                preparedEntities: [product]
            );

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.False(response.Succeeded);
            Assert.Contains("No file provided", response.Errors);
        }
    }
}
