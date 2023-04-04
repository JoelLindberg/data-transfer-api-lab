using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning.Conventions;
using Asp.Versioning;

using DataTransferApiLab.Models;
using DataTransferApiLab.Data;


namespace DataTransferApiLab;


public class Program
{
    public static string Base64Encode(string plainText) 
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }


    public static string Base64Decode(string base64EncodedData) 
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }


    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options => {});        
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddApiVersioning(
        options =>
        {
            // reporting api versions will return the headers
            // "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;

            options.Policies.Sunset( 0.9 )
                            .Effective( DateTimeOffset.Now.AddDays( 60 ) )
                            .Link( "policy.html" )
                                .Title( "Versioning Policy" )
                                .Type( "text/html" );
        } )
        .AddApiExplorer(
        options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        } );

        // Swagger
        //builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        //builder.Services.AddSwaggerGen( options => options.OperationFilter<SwaggerDefaultValues>() );
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Data Transfer API Lab", Description = "Integrate your data", Version = "v1" });
        });
        
        // DB Context
        builder.Services.AddDbContext<DataTransferApiLabContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DataTransferApiLabContext") ?? throw new InvalidOperationException("Connection string 'DataTransferApiLabContext' not found.")));


        var app = builder.Build();

        app.UseCors();

        var versionSet = app.NewApiVersionSet("Transfers")
                            .HasApiVersion( 1.0 )
                            .HasApiVersion( 2.0 )
                            .ReportApiVersions()
                            .Build();

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Data Transfer API Lab v1");
        });


        app.MapGet("/", () => Results.Redirect("/swagger"))
                                     .WithApiVersionSet( versionSet )
                                     .IsApiVersionNeutral()
                                     .ExcludeFromDescription();

        // Version 1.0
        app.MapPost("/api/v{version:apiVersion}/transfer", async (HttpRequest request, DataTransferApiLabContext db) => 
        {
            var transfer = await request.ReadFromJsonAsync<Transfer>();
            //var transfer = new Transfer();
            transfer.TransferData = Base64Decode(transfer.TransferData);

            db.Transfers.Add(transfer);
            await db.SaveChangesAsync();

            //return Results.Created($"/api/v1/transfer/{transfer.TransferId}", transfer);
            var scheme = request.Scheme;
            var host = request.Host;
            var version = request.HttpContext.GetRequestedApiVersion();
            var location = new Uri( $"{scheme}{Uri.SchemeDelimiter}{host}/v{version}/api/people/{transfer.TransferId}" );
            return Results.Created(location, transfer);
            //return Results.Json(data: transfer, statusCode: 201);
        })
        .Accepts<Transfer>("application/json")
        .Produces<Transfer>(201)
        .Produces(400)
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0);

        app.MapGet("/api/v{version:apiVersion}/transfer/{id}", async (int id, DataTransferApiLabContext db) => 
        {
            Transfer transfer = await db.Transfers.FindAsync(id);
            transfer.TransferData = Base64Encode(transfer.TransferData);
            return transfer;
        })
        .Produces<Transfer>(200)
        .Produces(400)
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0);


        // Version 2.0
        
        // Add version 2 here ...



        // Debug - Get all transfers as text
        app.MapGet("/api/transfer", async (DataTransferApiLabContext db) =>
        {
            List<Transfer> transfers = await db.Transfers.ToListAsync();
            return transfers;
        })
        .WithApiVersionSet( versionSet )
        .IsApiVersionNeutral();

        app.Run();
    }
}





