using AspNetCore.ApiGateway.AzureFunctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Sample.ApiGateway.AzureFunctions;

//var builder = FunctionsApplication.CreateBuilder(args);

//builder.ConfigureFunctionsWebApplication();

//// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
//// builder.Services
////     .AddApplicationInsightsTelemetryWorkerService()
////     .ConfigureFunctionsApplicationInsights();

//builder.Build().Run();

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register the GatewayMiddleware to handle incoming requests to the gateway
// This should be registered before any other middleware to ensure it can process the requests correctly
// The GatewayMiddleware will block the request if it doesn't match the gateway route pattern
builder.UseMiddleware<GatewayMiddleware>();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);

app.Run();
