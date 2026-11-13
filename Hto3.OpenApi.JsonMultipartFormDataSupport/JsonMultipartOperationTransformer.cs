using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hto3.OpenApi.JsonMultipartFormDataSupport
{
    public sealed class JsonMultipartOperationTransformer : IOpenApiOperationTransformer
    {
        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (context.Description.ActionDescriptor is not ControllerActionDescriptor action)
                return;

            var parameters = action.MethodInfo.GetParameters();

            if (!parameters.Any(p => p.IsDefined(typeof(JsonFormPartAttribute), false)))
                return;

            var jsonPartNames = parameters
                .Where(p => p.IsDefined(typeof(JsonFormPartAttribute), false))
                .Select(p => p.Name!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            if (operation.Parameters is not null)
            {
                var queryParameters = operation.Parameters
                    .Where(p => p.In == ParameterLocation.Query && jsonPartNames.Contains(p.Name!))
                    .ToArray();

                foreach (var queryParameter in operation.Parameters)
                    operation.Parameters.Remove(queryParameter);
            }

            var properties = new Dictionary<String, IOpenApiSchema>();
            var encoding = new Dictionary<String, OpenApiEncoding>();
            var required = new HashSet<String>();
            var nullability = new NullabilityInfoContext();

            foreach (var parameter in parameters)
            {
                var name = parameter.Name ?? throw new InvalidOperationException("A form parameter has no name.");

                if (parameter.IsDefined(typeof(JsonFormPartAttribute), false))
                {
                    properties[name] = await context.GetOrCreateSchemaAsync(parameter.ParameterType, cancellationToken: cancellationToken);
                    encoding[name] = new OpenApiEncoding()
                    {
                        ContentType = "application/json"
                    };                
                }
                else if (parameter.ParameterType == typeof(IFormFile))
                {
                    properties[name] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String,
                        Format = "binary"
                    };
                }

                var isNullable = nullability.Create(parameter).ReadState == NullabilityState.Nullable || Nullable.GetUnderlyingType(parameter.ParameterType) is not null;

                if (!isNullable && !parameter.HasDefaultValue)
                    required.Add(name);
            }

            operation.RequestBody = new OpenApiRequestBody()
            {
                Required = required.Count > 0,
                Content = new Dictionary<String, OpenApiMediaType>()
                {
                    ["multipart/form-data"] = new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Object,
                            Properties = properties,
                            Required = required
                        },
                        Encoding = encoding
                    }
                }
            };
        }
    }
}
