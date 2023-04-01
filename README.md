# Data transfer API lab

Sending and fetching JSON data (base64 encoded) with transfer audit logging.

ASP.NET minimal API, with the following initial goal:
 - [x] Database as data store
 - [x] Sending/fetching JSON data
 - [x] Base64 encoded data
 - [ ] Transfer audit logging
 - [ ] API versioning -> Asp.Versioning.Http

Keeping it intentionally as a single file program due to its small size.

## Usage

1. dotnet restore
2. dotnet ef database update
3. dotnet run
4. send json data
    ~~~shell
    curl -d "@data.json" -X POST http://localhost:5063/v1/transfer
    ~~~

    data.json
    ~~~json
    {
        "transferData": "<base64 encoded string>"
    }
    ~~~

## Entity Framework

Create the migration plan and execute it in order to have the SQLite database created with a table corresponding the model.

Migration:
~~~console
> dotnet ef migrations add InitialCreate
> dotnet ef database update
~~~

## Resources

### C# and ASP.NET

* https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-7.0&tabs=visual-studio-code
* https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string

Entity Framework:
* https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli

OpenAPI/Swagger:
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0

API versioning:
* https://www.nuget.org/packages/Asp.Versioning.Http
* https://github.com/dotnet/aspnet-api-versioning/tree/3857a332057d970ad11bac0edfdbff8a559a215d/examples/AspNetCore/WebApi/MinimalApiExample

### SQLite

* https://sqlite.org/cli.html
* https://sqlitebrowser.org/
