using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Embe.C2C.Api.EndPoints;
using Embe.C2C.Api.Extensions;
using Embe.C2C.Infrastructure;
using Embe.C2C.Infrastructure.Ef;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var settings = new Settings(builder.Configuration);

builder.Services.AddServices(settings, builder.Environment);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndPoints();
app.MapUserEndPoints();
app.MapMatchingEndPoints();
app.MapNotificationEndPoints();
app.MapMessageEndPoints();
app.MapGeographyEndpoints();
app.MapSearchProfileEndPoints();
app.MapOpenApiEndpoints();
app.MapSignalREndPoints();
app.MapCandidateEndPoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<C2CContext>();

    await context.Database.MigrateAsync();

    #region setup cors on azurite
    var blobServiceClient = new BlobServiceClient(app.Configuration.GetConnectionString("AzureStorageBlobs"));
    var containerClient = blobServiceClient.GetBlobContainerClient("images");
    await containerClient.CreateIfNotExistsAsync();
    var props = await blobServiceClient.GetPropertiesAsync();

    props.Value.Cors.Clear();
    props.Value.Cors.Add(new BlobCorsRule
    {
        AllowedOrigins = "http://frontend-embe.c2c.aspire.dev.localhost:51649",
        AllowedMethods = "GET,PUT,HEAD,OPTIONS,DELETE",
        AllowedHeaders = "*",
        ExposedHeaders = "*",
        MaxAgeInSeconds = 3600
    });

    await blobServiceClient.SetPropertiesAsync(props.Value);
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    #endregion
}

app.Run();