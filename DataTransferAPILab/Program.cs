using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;


namespace DataTransferAPILab;


// Models
public class Transfer
{
    public int TransferId { get; set; }   
    public string TransferData { get; set; } // base64 encoded
}


// DB Context
public class DataTransferAPILabContext : DbContext
{
    public DataTransferAPILabContext (DbContextOptions<DataTransferAPILabContext> options)
        : base(options) { }

    public DbSet<Transfer> Transfers { get; set; }
}


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

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Data Transfer API Lab", Description = "Integrate your data", Version = "v1" });
        });
        
        // DB Context
        builder.Services.AddDbContext<DataTransferAPILabContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DataTransferAPILabContext") ?? throw new InvalidOperationException("Connection string 'DataTransferAPILabContext' not found.")));

        var app = builder.Build();
        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Data Transfer API Lab V1");
        });

        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

        app.MapPost("/v1/transfer", async (string data, DataTransferAPILabContext db) => 
        {
            var transfer = new Transfer();
            transfer.TransferData = Base64Decode(data);
            
            db.Transfers.Add(transfer);
            await db.SaveChangesAsync();

            return Results.Created($"/v1/transfer/{transfer.TransferId}", transfer);
        });

        app.MapGet("/v1/transfer/{id}", async (int id, DataTransferAPILabContext db) => 
        {
            Transfer transfer = await db.Transfers.FindAsync(id);
            transfer.TransferData = Base64Encode(transfer.TransferData);
            return transfer;
        });

        // Debug - Get all transfers as text
        app.MapGet("/v1/transfer", async (DataTransferAPILabContext db) =>
        {
            List<Transfer> transfers = await db.Transfers.ToListAsync();
            return transfers;
        });

        app.Run();
    }
}





