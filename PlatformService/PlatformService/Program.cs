using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// add the db context 
if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Use SqlServer Db");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Use In memory Db");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
// register automapper 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// register the Http client factory 
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
// add RabbitMQ message bus 
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
// add grpc for dependency injection 
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// add the grpc endpoint 
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<GrpcPlatformService>();
    endpoints.MapGet("/protos/platforms.protos", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/plaforms.proto"));
    });
});
// call the static class - to apply migrations 
PrepDb.PrePopulation(app, builder.Environment.IsProduction());

app.Run();
