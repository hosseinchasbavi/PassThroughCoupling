using Microsoft.EntityFrameworkCore;
using ProviderProject;
using ProviderProject.Db;
using ProviderProject.RabbitMq;

var builder = WebApplication.CreateBuilder(args);


var builderConfiguration = builder.Configuration.AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", reloadOnChange: true, optional: true)
    .AddEnvironmentVariables();

var configuration = builderConfiguration.Build();

// Add services to the container.
builder.Services.AddDbContext<MyContext>(options => { options.UseSqlServer(configuration.GetConnectionString("MyConnection")); });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IPushHandler, PushHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();