using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning.Conventions;
using Asp.Versioning;

using DataTransferApiLab.Models;
using DataTransferApiLab.Data;
using DataTransferApiLab.Middleware;

namespace DataTransferApiLab;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options => {});
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddApiVersioning(
        options =>
        {
            options.DefaultApiVersion = new ApiVersion(2, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            // reporting api versions will return the headers
            options.ReportApiVersions = true;
            options.Policies.Sunset(0.9)
                            .Effective(DateTimeOffset.Now.AddDays(60))
                            .Link("policy.html")
                                .Title("Versioning Policy")
                                .Type("text/html");
        })
        .AddApiExplorer(
        options =>
        {
            options.DefaultApiVersion = new ApiVersion(2, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";
            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo {Title = "Data Transfer API Lab", Description = "Integrate your data", Version = "v1"});
            options.SwaggerDoc("v2", new OpenApiInfo { Title = "Data Transfer API Lab", Description = "Integrate your data", Version = "v2" });
        });
        
        // DB Context
        builder.Services.AddDbContext<DataTransferApiLabContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DataTransferApiLabContext") ?? throw new InvalidOperationException("Connection string 'DataTransferApiLabContext' not found.")));


        var app = builder.Build();

        app.UseCors();
        
        var versionSet = app.NewApiVersionSet("Transfers")
                            .HasApiVersion(1.0)
                            .HasApiVersion(2.0)
                            .ReportApiVersions()
                            .Build();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "Data Transfer API Lab v2");
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Data Transfer API Lab v1");
            options.DefaultModelsExpandDepth(-1); // hide schemas from the main swagger ui page
        });

        app.UseMiddleware<RequestResponseLogger>();

        app.MapGet("/", () => Results.Redirect("/swagger"))
                                     .WithApiVersionSet(versionSet)
                                     .IsApiVersionNeutral()
                                     .ExcludeFromDescription();

        // Version 1.0

        // Upload
        app.MapPost("/api/v{version:apiVersion}/transfer", async (HttpRequest request, DataTransferApiLabContext db) => {
            var receivedPayload = await request.ReadFromJsonAsync<Transfer>();
            var transfer = new Transfer();
            transfer.TransferName = receivedPayload.TransferName;
            transfer.TransferData = Utils.Base64.Base64Decode(receivedPayload.TransferData);
            transfer.SetBytes(System.Text.ASCIIEncoding.UTF8.GetByteCount(transfer.TransferData));
            db.Transfers.Add(transfer);
            await db.SaveChangesAsync();

            var scheme = request.Scheme;
            var host = request.Host;
            var version = request.HttpContext.GetRequestedApiVersion();
            var location = new Uri($"{scheme}{Uri.SchemeDelimiter}{host}/api/v{version}/transfer/{transfer.TransferDataId}");

            var response = new TransferResponse();
            response.TransferDataId = transfer.TransferDataId;
            response.TransferName = transfer.TransferName;
            response.Bytes = transfer.Bytes;

            return Results.Json(response);
        })
        .Accepts<Transfer>("application/json")
        .Produces<TransferResponse>(201)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Send base64 encoded data",
            Description = "Accepts base64 encoded data in a JSON payload."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .WithGroupName("v1");


        // Download
        app.MapGet("/api/v{version:apiVersion}/transfer/{id}", async (int id, DataTransferApiLabContext db) => {
            Transfer transfer = await db.Transfers.FindAsync(id);
            var transferDownloadResponse = new TransferDownloadResponse();
            transferDownloadResponse.TransferDataId = transfer.TransferDataId;
            transferDownloadResponse.TransferName = transfer.TransferName;
            transferDownloadResponse.TransferData = Utils.Base64.Base64Encode(transfer.TransferData);
            transferDownloadResponse.Bytes = transfer.Bytes;

            return Results.Json(transferDownloadResponse);
        })
        .Produces<TransferDownloadResponse>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Fetch base64 encoded data",
            Description = "Sends base64 encoded data in a JSON payload."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .WithGroupName("v1");


        // Fetch transfer audit logs
        app.MapGet("/api/v{version:apiVersion}/audits", async (DataTransferApiLabContext db) => {
            var audits = await db.Audits.ToListAsync();

            return Results.Json(audits);
        })
        .Produces<Audit>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Get all transfer logs",
            Description = "Fetches all transfer events from the transfer audit logs."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .WithGroupName("v1");


        // View currently stored transfer data
        app.MapGet("/api/v{version:apiVersion}/transfers", async (DataTransferApiLabContext db) => {
            var transfers = await db.Transfers.Select(i => new { i.TransferDataId, i.TransferName, i.Bytes }).ToListAsync();
            return Results.Json(transfers);
        })
        .Produces<TransferResponse>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "View all stored data",
            Description = "Returns an overview of all stored transfer data."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .WithGroupName("v1");




        // Version 2.0

        // Upload
        app.MapPost("/api/v{version:apiVersion}/transfer", async (HttpRequest request, DataTransferApiLabContext db) => {
            var receivedPayload = await request.ReadFromJsonAsync<Transfer>();
            var transfer = new Transfer();
            transfer.TransferName = receivedPayload.TransferName;
            transfer.TransferData = Utils.Base64.Base64Decode(receivedPayload.TransferData);
            transfer.SetBytes(System.Text.ASCIIEncoding.UTF8.GetByteCount(transfer.TransferData));
            db.Transfers.Add(transfer);
            await db.SaveChangesAsync();

            var scheme = request.Scheme;
            var host = request.Host;
            var version = request.HttpContext.GetRequestedApiVersion();
            var location = new Uri($"{scheme}{Uri.SchemeDelimiter}{host}/api/v{version}/transfer/{transfer.TransferDataId}");

            var response = new TransferResponse();
            response.TransferDataId = transfer.TransferDataId;
            response.TransferName = transfer.TransferName;
            response.Bytes = transfer.Bytes;

            return Results.Json(response);
        })
        .Accepts<Transfer>("application/json")
        .Produces<TransferResponse>(201)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Send base64 encoded data",
            Description = "Accepts base64 encoded data in a JSON payload."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(2.0)
        .WithGroupName("v2");


        // Download
        app.MapGet("/api/v{version:apiVersion}/transfer/{id}", async (int id, DataTransferApiLabContext db) => {
            Transfer transfer = await db.Transfers.FindAsync(id);
            var transferDownloadResponse = new TransferDownloadResponse();
            transferDownloadResponse.TransferDataId = transfer.TransferDataId;
            transferDownloadResponse.TransferName = transfer.TransferName;
            transferDownloadResponse.TransferData = Utils.Base64.Base64Encode(transfer.TransferData);
            transferDownloadResponse.Bytes = transfer.Bytes;

            return Results.Json(transferDownloadResponse);
        })
        .Produces<TransferDownloadResponse>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Fetch base64 encoded data",
            Description = "Sends base64 encoded data in a JSON payload."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(2.0)
        .WithGroupName("v2");


        // Delete stored transfer data
        app.MapDelete("/api/v{version:apiVersion}/transfer/{id}", async (int id, DataTransferApiLabContext db) => {
            var transfer = await db.Transfers
                                   .Where(p => p.TransferDataId == id)
                                   .SingleAsync();
            db.Remove(transfer);
            await db.SaveChangesAsync();

            var transferResponse = new TransferResponse();
            transferResponse.TransferDataId = transfer.TransferDataId;
            transferResponse.TransferName = transfer.TransferName;
            transferResponse.Bytes = transfer.Bytes;

            return Results.Json(transferResponse);
        })
        .Produces<TransferResponse>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Remove stored transfer data",
            Description = "Removes the requested stored data."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(2.0)
        .WithGroupName("v2");


        // Fetch transfer audit logs
        app.MapGet("/api/v{version:apiVersion}/audits", async (DataTransferApiLabContext db) => {
            var audits = await db.Audits.ToListAsync();

            return Results.Json(audits);
        })
        .Produces<Audit>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "Get all transfer logs",
            Description = "Fetches all transfer events from the transfer audit logs."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(2.0)
        .WithGroupName("v2");


        // View currently stored transfer data
        app.MapGet("/api/v{version:apiVersion}/transfers", async (DataTransferApiLabContext db) => {
            var transfers = await db.Transfers.Select(i => new { i.TransferDataId, i.TransferName, i.Bytes }).ToListAsync();
            return Results.Json(transfers);
        })
        .Produces<TransferResponse>(200)
        .Produces(400)
        .WithOpenApi(operation => new(operation) {
            Summary = "View all stored data",
            Description = "Returns an overview of all stored transfer data."
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(2.0)
        .WithGroupName("v2");




        app.Run();
    }
}





