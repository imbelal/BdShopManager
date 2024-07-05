# .NET Monolith Boilerplate with Clean Architecture, DDD And CQRS

This project is a .NET-based monolith application that demonstrates a clean architecture design pattern. It leverages Domain-Driven Design (DDD), Command Query Responsibility Segregation (CQRS), and MediatR for decoupling and modularity. The application is built with various modern software engineering principles and practices to ensure scalability, maintainability, and testability.

## Features

- **Architecture**: Event driven clean architecture following DDD and CQRS principles.
- **Database**: Separate databases for read and write operations.
- **Caching**: In-memory caching for improved performance.
- **Soft Delete**: Soft delete implementation for entities.
- **Auto Auditing**: Automatic auditing for tracking changes.
- **Pagination**: Support for paginated queries.
- **MediatR**: Used for handling commands and queries.
- **Repository Pattern**: Abstract data access.
- **Unit of Work**: Manage transactions.
- **Logging**: Implemented using NLog.
- **Cron Jobs**: Scheduled tasks using cron jobs.
- **Exception Handling**: Global exception handler.
- **Custom user authentication and authorization**: Custom user authentication using JWT token (Access and Refresh token) with role based authorization.
- **Testing**: 
  - Unit tests using xUnit
  - Integration tests using in-memory SQLite database.
- **Build Pipeline**: Azure build pipeline scripts provided.
- **Docker Support**: Containerized using Docker.

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Docker (optional, for containerization)

### Installation

1. **Clone the repository:**

    ```bash
    git clone https://github.com/imbelal/BdShopManager.git
    ```

2. **Restore dependencies:**

    ```bash
    dotnet restore
    ```

3. **Build the project:**

    ```bash
    dotnet build
    ```

4. **Run the application:**

   ```bash
    dotnet run --project webapi
   ```

### Run with docker-compose
  ```bash
    docker-compose up
  ```

   ```bash
    docker-compose down
   ```

## Project Structure

- **Domain**: Contains domain entities, value objects, aggregates, and interfaces.
- **Application**: Application logic including commands, queries, handlers, validators, and DTOs.
- **Infrastructure**: Implementation of repository patterns, database context, logging, and other services.
- **WebAPI**: Entry point of the application with controllers, middleware, and configuration.
- **UnitTest**: Contains all unit tests.
- **IntegrationTest**: Contains all integration tests.

## Key Components

### Logging

NLog is configured for logging. Configuration can be found in `nlog.config`.

### Domain-Driven Design (DDD)

The core business logic and domain models are encapsulated within the `Core` project.

### Command Query Responsibility Segregation (CQRS)

Commands and queries are separated to ensure clean separation of concerns. MediatR is used to handle commands and queries.

### Database

Separate databases are used for read and write operations. Entity Framework Core is used for data access.

### Caching

In-memory caching is implemented for improving performance on read-heavy operations.

### Soft Delete and Auto Auditing

Entities are configured to support soft delete and automatic auditing of create, update, and delete operations.

### Pagination

Support for paginated queries is implemented to handle large datasets efficiently.

### Cron Jobs

Scheduled tasks are implemented using cron jobs.

### Repository and Unit of Work

The repository pattern abstracts data access, while the unit of work pattern manages transactions.

### Testing

- **Unit Tests**: Implemented using xUnit.
- **Integration Tests**: Using in-memory SQLite database for testing database interactions.

### Global Exception Handling

Global exception handling middleware is configured to provide consistent error responses.

### Docker Support

Dockerfile is provided to build and run the application in a containerized environment.

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License.

## Acknowledgements

- [NLog](https://nlog-project.org/)
- [MediatR](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [xUnit](https://xunit.net/)
- [Docker](https://www.docker.com/)

---

This README provides a high-level overview of the application's features and structure. For more detailed documentation, please refer to the specific documentation files or the source code comments.
