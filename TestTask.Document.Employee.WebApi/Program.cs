using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.Text.Json;
using TestTask.Document.Employee.Endpoints;
using TestTask.Document.Employee.WebApi.Builders;
using TestTask.Document.Employee.WebApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEmployeeDocumentServices(builder.Configuration);

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddExceptionHandler<ExceptionHandler>();
}

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddControllers();

var propertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;

builder.Services.ConfigureHttpJsonOptions(opt =>
{
    opt.SerializerOptions.PropertyNamingPolicy = propertyNamingPolicy;
});

if (builder.Configuration.GetValue<bool>("UseSwagger"))
{
    builder.Services.AddTransient<ISerializerDataContractResolver>(_ =>
        new JsonSerializerDataContractResolver(
           new JsonSerializerOptions { PropertyNamingPolicy = propertyNamingPolicy, }
    ));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        var securitySchema = new OpenApiSecurityScheme
        {
            Description = "Стандартный заголовок авторизации с использованием схемы Bearer. Пример: \"Authorization: Bearer {jwt}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            //Reference = new OpenApiReference
            //{
            //    Type = ReferenceType.SecurityScheme,
            //    Id = JwtBearerDefaults.AuthenticationScheme
            //}
        };

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);

        options.CustomSchemaIds(x => x.FullName);

        options.MapType<TimeOnly>(() => new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Default = new TimeOnly(DateTime.UtcNow.TimeOfDay.Ticks).ToString("hh:mm:ss"),
        });
    });
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseStatusCodePages();
}


if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseEmployeeDocumentEndpoints();

app.Run();
