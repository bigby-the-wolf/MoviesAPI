# MoviesAPI

MoviesAPI is an example of writing a Web API that is sustainable.

## Background

This solution comes as a cumulative effort to implement the great ideas presented by [Mark Seemann](https://blog.ploeh.dk/about/) in his books: [Dependency Injection Principles, Practices, and Patterns](https://www.amazon.com/gp/product/B09783WN5C) and [Code That Fits in Your Head](https://www.amazon.com/gp/product/B09D2X43VX).

## Goal

The focus is creating software that is sustainable. This is, in my opinion, the most important trait a code base can boast.

The chosen domain is trivial, it focuses on a single entity, Movie. This is deliberate because the focus is on the "infrastructure" that supports developing readable, testable, maintainable and extensible code.

## Implementation

This is made possible by using Aspect Oriented Programming driven by the SOLID principles via Command-Query Separation.

All actions are modeled as either commands or queries and everything is loosely coupled via Dependency Injection. Every Cross-Cutting Concern is added as a Decorator to the appropriate command/query.

Cross-Cutting Concerns implemented: logging, exception handling, resiliency.

## Technologies and Tools

The solution is written using .NET 6 in VS2022 and all the latest features and package versions. For DB access I chose Entity Framework as it's the popular choice.

Important packages include: [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) for modeling null cases with Maybe and [Polly](https://github.com/App-vNext/Polly) for defining the resilience policy.

## Build and Run

The solution uses Docker. 

First, two DBs must be available. The second one is for testing purposes. There is a docker compose file that can be used to create the required containers. I suggest running:
```csharp
docker compose up --no-start
```
and manually start whichever container you want. You could also comment out unneeded containers.

The containers are:
- web: hosts the application;
- web_development: hosts the application but watches for changes in the codebase and triggers automatic rebuilds;
- mssql: hosts the DB;
- mssql_tests: hosts a DB for testing.

Before using the application a manual database update must be done to add the EF migration. Also the visual studio debug profile must be instructed to use the new docker network used by the other containers.

## Testing

The solution has two tests projects. One for the WebApi and one for the EF code.

The Web API tests are composed of Unit Tests and Boundary Tests.

The EF tests are dependent on a DB being available for testing. This comes directly from [MS](https://docs.microsoft.com/en-us/ef/core/testing/). 
