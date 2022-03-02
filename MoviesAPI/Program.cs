using Microsoft.EntityFrameworkCore;
using MoviesApi.EntityFramework;
using MoviesAPI.ExceptionFilters;
using MoviesAPI.Utilities.Extensions;

#pragma warning disable CA1812
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<NotImplementedExceptionFilter>();
    options.Filters.Add<BrokenCircuitExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbConnectionString = builder.Configuration.GetConnectionString("MoviesDb");
builder.Services.AddDbContext<MoviesContext>(opt => opt.UseSqlServer(dbConnectionString));

builder.Services.ConfigurePollyPolicies();
builder.Services.ConfigureCQS();

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