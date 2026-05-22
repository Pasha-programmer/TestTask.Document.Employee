using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TestTask.Document.Employee.WebApi.Swagger.OperationFilters.Auth
{
    internal class ApplyAuthJwtBearerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
            {
                operation.Security = [
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                            }, [ JwtBearerDefaults.AuthenticationScheme ]
                        }
                    }
                ];
            }
        }
    }
}
