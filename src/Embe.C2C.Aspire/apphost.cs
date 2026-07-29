#:package Aspire.Hosting.Azure.Functions@13.4.6
#:package Aspire.Hosting.Azure.ServiceBus@13.4.6
#:package Aspire.Hosting.Azure.Storage@13.4.6
#:package Aspire.Hosting.JavaScript@13.4.6
#:package Aspire.Hosting.PostgreSQL@13.4.6
#:sdk Aspire.AppHost.Sdk@13.4.6

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithImage("postgis/postgis");

var defaultConnection = postgres.AddDatabase("DefaultConnection", "c2c");

var azureSignalR = builder.AddExecutable("AzureSignalR", "asrs-emulator", ".", "start");
const string azureSignalRConnectionString = "Endpoint=http://localhost;Port=8888;AccessKey=ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGH;Version=1.0;";

var azureStorage = builder
    .AddAzureStorage("AzureStorage")
    .RunAsEmulator();

var azureStorageBlobs = azureStorage.AddBlobs("AzureStorageBlobs");
var azureStorageQueues = azureStorage.AddQueues("AzureStorageQueues");

var azureServiceBus = builder.AddAzureServiceBus("AzureServiceBus2").RunAsEmulator().AddServiceBusQueue("AzureServiceBus", "work-items");

var api = builder
    .AddProject("Api", "../Embe.C2C.Api/Embe.C2C.Api.csproj")
    .WithEnvironment("ConnectionStrings__AzureSignalR", azureSignalRConnectionString)
    .WithEnvironment("Jwt:Issuer", "Embe.C2C.Api")
    .WithEnvironment("Jwt:Audience", "Embe.C2C.Api")
    .WithEnvironment("Jwt__Secrets__AccessToken", "AccessTokenYourSuperSecretKeyForJwtTokenGeneration")
    .WithEnvironment("Jwt__Secrets__RefreshToken", "AccessTokenYourSuperSecretKeyForJwtTokenGeneration")
    .WithEnvironment("Jwt__Secrets__ResetPasswordToken", "AccessTokenYourSuperSecretKeyForJwtTokenGeneration")
    .WithEnvironment("Cors__AllowedOrigins", "http://frontend-embe.c2c.aspire.dev.localhost:51649")
    .WithEnvironment("Application__Name", "Embe.C2C")
    .WithEnvironment("Site__Url", "http://frontend-embe.c2c.aspire.dev.localhost:51649")
    .WithReference(defaultConnection)
    .WithReference(azureServiceBus)
    .WithReference(azureStorageBlobs)
    .WaitFor(azureStorage)
    .WaitFor(azureSignalR)
    .WaitFor(azureServiceBus);

var functions = builder
    .AddAzureFunctionsProject("functions", "../Embe.C2C.Functions/Embe.C2C.Functions.csproj")
    .WithEnvironment("ConnectionStrings__AzureSignalR", azureSignalRConnectionString)
    .WithHostStorage(azureStorage)
    .WithReference(azureStorageBlobs)
    .WithReference(defaultConnection)
    .WithReference(azureServiceBus)
    .WaitFor(azureStorageBlobs)
    .WaitFor(azureSignalR)
    .WaitFor(defaultConnection)
    .WaitFor(azureServiceBus);

#pragma warning disable ASPIREJAVASCRIPT001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var frontend = builder.AddNextJsApp("frontend", "../embe.c2c.frontend/")
    .WaitFor(api)
    .WithReference(api)
    .WithEnvironment("API_URL", api.GetEndpoint("https"))
    .WithEnvironment("NEXT_PUBLIC_API_URL", api.GetEndpoint("https"))
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(port: 51649);
#pragma warning restore ASPIREJAVASCRIPT001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Build().Run();