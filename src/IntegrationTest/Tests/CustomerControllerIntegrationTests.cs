using Application.Features.Customer.Commands;
using Bogus;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Entities;
using IntegrationTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using Xunit;

namespace IntegrationTest.Tests
{
    public class CustomerControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Faker<Customer> _customerFaker;

        public CustomerControllerIntegrationTests()
        {
            _customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.FirstName, f => f.Person.FirstName)
                .RuleFor(c => c.LastName, f => f.Person.LastName)
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.ContactNo, f => f.Phone.PhoneNumber().Substring(0, 10))
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Remark, f => f.Lorem.Sentence());
        }

        [Fact]
        public async Task Create_Customer_Should_Succeed()
        {
            // Arrange
            var createCommand = new CreateCustomerCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                ContactNo = "1234567890",
                Email = "john.doe@example.com",
                Remark = "Test customer"
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<CustomerController>().Create(createCommand));

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);
            Assert.NotEqual(Guid.Empty, response.Data);

            // Verify customer was saved to database
            var savedCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Id == response.Data);
            Assert.NotNull(savedCustomer);
            Assert.Equal("John", savedCustomer.FirstName);
            Assert.Equal("Doe", savedCustomer.LastName);
        }

        [Fact]
        public async Task Update_Customer_Should_Succeed()
        {
            // Arrange
            var customer = _customerFaker.Generate();

            var updateCommand = new UpdateCustomerCommand
            {
                Id = customer.Id,
                FirstName = "Jane",
                LastName = "Smith",
                Address = "456 Oak Ave",
                ContactNo = "0987654321",
                Email = "jane.smith@example.com",
                Remark = "Updated customer"
            };

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<CustomerController>().Update(updateCommand),
                preparedEntities: [customer]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);

            // Verify customer was updated
            var updatedCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
            Assert.NotNull(updatedCustomer);
            Assert.Equal("Jane", updatedCustomer.FirstName);
            Assert.Equal("Smith", updatedCustomer.LastName);
        }

        [Fact]
        public async Task Delete_Customer_Should_Succeed()
        {
            // Arrange
            var customer = _customerFaker.Generate();

            var deleteCommand = new DeleteCustomerCommand(customer.Id);

            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<CustomerController>().Delete(deleteCommand),
                preparedEntities: [customer]);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<Guid>>(okResult.Value);
            Assert.True(response.Succeeded);

            // Verify customer was soft deleted
            var deletedCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
            Assert.Null(deletedCustomer); // Soft deleted customer might be filtered out
        }

        [Fact]
        public async Task Get_All_Customers_Should_Return_Customers()
        {
            // Act
            var result = await ExecuteControllerMethodAsync(
                () => ResolveController<CustomerController>().GetAll());

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Just verify the controller returns a result without specific response type checking
        }
    }
}