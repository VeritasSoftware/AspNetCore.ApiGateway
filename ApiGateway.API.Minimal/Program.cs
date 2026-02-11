using ApiGateway.API.Minimal;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureAppConfiguration(configBuilder =>
//{
//    configBuilder.SetBasePath(Environment.CurrentDirectory)
//                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)//To specify environment
//                 .AddEnvironmentVariables();//You can add if you need to read environment variables.
//});

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);

app.Run();

//builder.Host.ConfigureHostConfiguration(configBuilder =>
//{
//    configBuilder.SetBasePath(Environment.CurrentDirectory)
//                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)//To specify environment
//                 .AddEnvironmentVariables();//You can add if you need to read environment variables.
//})
//.ConfigureWebHostDefaults(webBuilder =>
//{
//    webBuilder.UseStartup<Startup>();
//});

//builder.Build().Run();

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseApiGateway(orchestrator => ApiGateway.API.Minimal.ApiOrchestration.Create(orchestrator, app));

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
