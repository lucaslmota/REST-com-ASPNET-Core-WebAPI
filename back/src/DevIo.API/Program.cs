using DevIo.API.Configuration;
using DevIo.API.Extensions;
using DevIo.Data.Context;
using HealthChecks.UI.Client;
using IdentityModel.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddCheck("Produtos", new SqlServerHealthCheck(builder.Configuration.GetConnectionString("ConnStr")))
    .AddSqlServer(builder.Configuration.GetConnectionString("ConnStr"), name:"sqlserver", tags: new string[] { "db", "data", "sql" });

builder.Services.AddHealthChecksUI()
    .AddSqlServerStorage(builder.Configuration.GetConnectionString("ConnStr"));

builder.Services.AddControllers().AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<MeuDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr"));
});

#region
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified  = true;
    options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ResolveDependencies();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

builder.Services.AddSwaggerConfig();

builder.Services.AddLoggingConfigurationn();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfig(app.Services.GetService<IApiVersionDescriptionProvider>());
}

app.UseLoggingConfiguration();

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/api/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options => 
{
    options.UIPath = "/monitor";
});

app.Run();
