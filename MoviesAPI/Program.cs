using Microsoft.EntityFrameworkCore;
using MoviesApi.EntityFramework;
using MoviesAPI.CommandProcessors;
using MoviesAPI.CQS;
using MoviesAPI.QueryProcessors;

#pragma warning disable CA1812
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbConnectionString = builder.Configuration.GetConnectionString("MoviesDb");
builder.Services.AddDbContext<MoviesContext>(opt => opt.UseSqlServer(dbConnectionString));

builder.Services.AddScoped<ICommandProcessor, MSDICommandProcessor>();
builder.Services.AddScoped<IQueryProcessor, MSDIQueryProcessor>();

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
#pragma warning restore CA1812