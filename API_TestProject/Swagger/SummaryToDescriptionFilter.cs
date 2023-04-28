using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace API_TestProject.Swagger
{
    public class SummaryToDescriptionFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || string.IsNullOrEmpty(operation.Summary))
            { return; }

            operation.Description = operation.Summary;
            operation.Summary = null;
        }
    }
}
