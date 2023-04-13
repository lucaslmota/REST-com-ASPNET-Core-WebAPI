using DevIo.API.Configuration;
using DevIo.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnStr");

builder.Services.AddDbContext<MeuDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddControllers().AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ResolveDependencies();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
