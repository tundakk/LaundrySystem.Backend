<general_rules>
- When creating new controllers, always first check the Controllers/Implementations directory to see if one exists. If not, use the Python script `python generate_controllers.py` in the LaundrySystem.Api directory to generate standardized controllers that inherit from BaseController.
- When creating new services, always first check the Services/Interfaces and Services/Implementations directories. If not found, use the Python script `python generate_bll_files.py` in the LaundrySystem.BLL directory to generate both interface and implementation files that follow the IBaseService pattern.
- When creating new repositories, always first check the Repos/Interfaces and Repos/Implementations directories. If not found, use the Python script `python generate_repos.py` in the LaundrySystem.DAL directory to generate both interface and implementation files that inherit from BaseRepo.
- All entities must use Guid as the primary key type and follow the naming convention: Entity -> EntityModel -> IEntityService -> EntityService -> IEntityRepo -> EntityRepo.
- Always wrap service responses in ServiceResponse<T> objects to maintain consistent API responses.
- When adding new entities, ensure you create corresponding Model classes in the Domain.Model project and configure Mapster mappings in MappingConfig.cs.
- Register all new services and repositories in ServiceCollectionExtensions.cs following the existing scoped lifetime pattern.
- Use async/await patterns consistently across all layers (repositories, services, and controllers).
- Enable nullable reference types and handle null values appropriately throughout the codebase.
- Generate XML documentation for all public APIs by setting GenerateDocumentationFile to true in project files.
</general_rules>

<repository_structure>
- **LaundrySystem.Api**: ASP.NET Core Web API project containing controllers, mappings, and application startup configuration. Controllers inherit from BaseController and are organized in Base/ and Implementations/ directories.
- **LaundrySystem.BLL**: Business Logic Layer containing services, email/SMS integrations, and dependency injection configuration. Services follow the IBaseService pattern and are organized in Base/, Interfaces/, and Implementations/ directories.
- **LaundrySystem.DAL**: Data Access Layer containing Entity Framework DbContext, repositories, migrations, and entity configurations. Repositories inherit from BaseRepo and are organized in Base/, Interfaces/, and Implementations/ directories.
- **LaundrySystem.Domain.Model**: Contains domain entities, models (DTOs), enums, responses, and value objects. Entities represent database tables while Models represent API contracts.
- **Test Projects**: LaundrySystem.BLL.Tests, LaundrySystem.DAL.Tests, and LaundrySystem.Base.Tests contain unit tests using NUnit framework.
- **Python Scripts**: Code generation scripts located in each project directory (generate_controllers.py, generate_bll_files.py, generate_repos.py) for creating standardized boilerplate code.
- **Media Directory**: Contains static files and documentation assets.
</repository_structure>

<dependencies_and_installation>
- Requires .NET 8.0 SDK for development and runtime.
- Uses NuGet Package Manager for dependency management - run `dotnet restore` at solution level to install all packages.
- Database: SQL Server with Entity Framework Core - connection string configured in appsettings.json.
- External Services: Twilio for SMS (configure in appsettings.json Twilio section) and Brevo/SendGrid for email (configure API keys).
- Key packages: Entity Framework Core, ASP.NET Core Identity, Mapster for object mapping, JWT Bearer authentication, Swagger/OpenAPI.
- Run `dotnet build` to build the entire solution or `dotnet run --project LaundrySystem.Api` to start the API.
</dependencies_and_installation>

<testing_instructions>
- Uses NUnit testing framework with NUnit3TestAdapter for test execution.
- Test projects follow naming convention: [ProjectName].Tests (e.g., LaundrySystem.BLL.Tests).
- Run tests using `dotnet test` command at solution level or target specific test projects.
- Tests should cover business logic in BLL layer and data access logic in DAL layer.
- Use coverlet.collector for code coverage analysis.
- Test classes should use [SetUp] and [Test] attributes following NUnit conventions.
- Mock external dependencies and database contexts for unit testing.
- Integration tests should use separate test database configurations.
</testing_instructions>

<pull_request_formatting>
</pull_request_formatting>
