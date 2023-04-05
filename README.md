# Data transfer API lab

Sending and fetching JSON data (base64 encoded) with transfer audit logging.

It expects to receive base64 encoded data in a JSON payload. \
It will decode any base64 data received and store it in a database. \
On request it will return base64 encoded data in a JSON payload.

ASP.NET minimal API, with the following initial goal:
 - [x] Database as data store
 - [x] Send/fetch JSON data
 - [x] Base64 data encoding/decoding
 - [ ] API versioning -> Asp.Versioning.Http
 - [ ] Transfer audit logging

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
    some data in the air -> base64 -> c29tZSBkYXRhIGluIHRoZSBhaXIK
    ~~~json
    {
        "transferData": "c29tZSBkYXRhIGluIHRoZSBhaXIK"
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

**Entity Framework**
* https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli
* https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations
* https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations
Concern regarding protecting Id (primary key) not to be overridden:
* https://stackoverflow.com/questions/26768695/can-private-setters-be-used-in-an-entity-model

OpenAPI/Swagger:
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0

API versioning:
* https://www.nuget.org/packages/Asp.Versioning.Http
* https://github.com/dotnet/aspnet-api-versioning/tree/3857a332057d970ad11bac0edfdbff8a559a215d/examples/AspNetCore/WebApi/MinimalApiExample

### SQLite

* https://sqlite.org/cli.html
* https://sqlitebrowser.org/
