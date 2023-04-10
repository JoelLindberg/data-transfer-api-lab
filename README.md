# Data transfer API lab

Sending and fetching JSON data (base64 encoded) with transfer audit logging.

It expects to receive base64 encoded data in a JSON payload. \
It will decode any base64 data received and store it in a database. \
On request it will return base64 encoded data in a JSON payload.

ASP.NET minimal API, with the following initial goal:
 - [x] Database as data store
 - [x] Send/fetch JSON data
 - [x] Base64 data encoding/decoding
 - [x] Transfer audit logging
 - [x] API versioning -> Asp.Versioning.Http
 - [x] Delete data request added in v2

<br />

## Usage

1. `dotnet restore`
2. `dotnet ef database update`
3. `dotnet run`
4. Try uploading some json data (base64 encoded):
    
    Windows (curl.exe can also be used):
    ~~~powershell
    $headers=@{}
    $headers.Add("Content-Type", "application/json")
    $response = Invoke-RestMethod -Uri 'http://localhost:5264/api/v2/transfer' -Method POST -Headers $headers -ContentType 'application/json' -Body '{
    "transferName": "job01-data01",
    "transferData": "c29tZSBkYXRhIGluIHRoZSBhaXIK"
    }'
    ~~~

    Linux:
    ~~~shell
    curl --request POST \
    --url http://localhost:5264/api/v2/transfer \
    --header 'Content-Type: application/json' \
    --data '{
    "transferName": "job01-data01",
    "transferData": "c29tZSBkYXRhIGluIHRoZSBhaXIK"
    }'
    ~~~

    *transferData: "some data in the air" -> base64 -> c29tZSBkYXRhIGluIHRoZSBhaXIK*

5. Explore the other API resources via Swagger UI: `http://localhost:5264`

    ![Flow](https://github.com/joellindberg/data-transfer-api-lab/raw/main/images/api-swagger-ui.png)

<br />
<br />

## Entity Framework

Create the migration plan and execute it in order to have the SQLite database created with a table corresponding the model.

Migration:
~~~console
> dotnet ef migrations add InitialCreate
> dotnet ef database update
~~~

<br />
<br />

## Resources

Self notes. Not sure these links can be of help to someone else. They are not specified in any specific order. They are resources I resorted to frequently or for specific one-time problems when getting stuck.

### C# and ASP.NET

#### Minimal API & Base64

* https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-7.0&tabs=visual-studio-code
* https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string

#### Entity Framework

* https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli
* https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations
* https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations

Concern regarding protecting Id (primary key) not to be overridden:
* https://stackoverflow.com/questions/26768695/can-private-setters-be-used-in-an-entity-model

Select specific columns using entity framework:
* https://www.brentozar.com/archive/2016/09/select-specific-columns-entity-framework-query/

#### Swagger/OpenAPI (Swashbuckle implementation)
* https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-7.0
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0
* https://stackoverflow.com/questions/60186760/how-can-i-specify-the-default-opening-version-of-swagger

#### API versioning
* https://www.nuget.org/packages/Asp.Versioning.Http
* https://github.com/dotnet/aspnet-api-versioning/tree/3857a332057d970ad11bac0edfdbff8a559a215d/examples/AspNetCore/WebApi/MinimalApiExample
* https://stackoverflow.com/questions/58834430/c-sharp-net-core-swagger-trying-to-use-multiple-api-versions-but-all-end-point

#### Logging (middleware)
* https://learn.microsoft.com/en-us/answers/questions/1109851/asp-net-core-web-api-how-to-log-requests-and-respo
* https://www.devtrends.co.uk/blog/conditional-middleware-based-on-request-in-asp.net-core
* https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/middleware?view=aspnetcore-7.0

### SQLite

* https://sqlite.org/cli.html
* https://sqlitebrowser.org/
