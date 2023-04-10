using System.Text.Json;

using DataTransferApiLab.Data;
using DataTransferApiLab.Models;

namespace DataTransferApiLab.Middleware;


public class RequestResponseLogger
{
    private readonly RequestDelegate _next;

    public RequestResponseLogger(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, DataTransferApiLabContext db)
    {
        var apiVersion = httpContext.GetRequestedApiVersion();

        if (httpContext.Request.Path == $"/api/v{apiVersion}/transfer" && httpContext.Request.Method == "POST") {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.PropertyNameCaseInsensitive = true;

            var requestBody = await ReadBodyFromRequest(httpContext.Request);
            var receivedPayload = JsonSerializer.Deserialize<Transfer>(requestBody, jsonOptions);

            // Temporarily replace the HttpResponseStream, which is a write-only stream, with a MemoryStream to capture it's value in-flight.  
            var originalResponseBody = httpContext.Response.Body;
            using var newResponseBody = new MemoryStream();
            httpContext.Response.Body = newResponseBody;

            // Call the next middleware in the pipeline  
            await _next(httpContext);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            var sentResponse = JsonSerializer.Deserialize<TransferUploadResponse>(responseBody, jsonOptions);

            var audit = new Audit();
            audit.TransferName = sentResponse.TransferName;
            audit.TransferDataId = sentResponse.TransferDataId;
            audit.Timestamp = DateTime.Now.ToString();
            audit.Action = "Upload";
            audit.Bytes = sentResponse.Bytes;
            db.Audits.Add(audit);
            await db.SaveChangesAsync();

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
        }
        else if (httpContext.Request.Path.StartsWithSegments($"/api/v{apiVersion}/transfer") && httpContext.Request.Method == "GET")
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.PropertyNameCaseInsensitive = true;

            // Temporarily replace the HttpResponseStream, which is a write-only stream, with a MemoryStream to capture it's value in-flight.  
            var originalResponseBody = httpContext.Response.Body;
            using var newResponseBody = new MemoryStream();
            httpContext.Response.Body = newResponseBody;

            // Call the next middleware in the pipeline  
            await _next(httpContext);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            var sentResponse = JsonSerializer.Deserialize<TransferDownloadResponse>(responseBody, jsonOptions);

            var audit = new Audit();
            audit.TransferName = sentResponse.TransferName;
            audit.TransferDataId = sentResponse.TransferDataId;
            audit.Timestamp = DateTime.Now.ToString();
            audit.Action = "Download";
            audit.Bytes = sentResponse.Bytes;
            db.Audits.Add(audit);
            await db.SaveChangesAsync();

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
        }
        else 
        {
            await _next(httpContext);
        }
    }

    private static async Task<string> ReadBodyFromRequest(HttpRequest request)
    {
        // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).  
        request.EnableBuffering();

        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync();

        // Reset the request's body stream position for next middleware in the pipeline.  
        request.Body.Position = 0;
        return requestBody;
    }
}