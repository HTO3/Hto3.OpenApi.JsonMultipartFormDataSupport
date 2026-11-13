using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace Hto3.OpenApi.JsonMultipartFormDataSupport
{
    public class JsonFormPartBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.HasFormContentType)
            {
                context.ModelState.TryAddModelError(context.ModelName, "Expected multipart form data.");
                return;
            }

            var form = await request.ReadFormAsync(context.HttpContext.RequestAborted);
            var filePart = form.Files.GetFile(context.ModelName);
            var field = form[context.ModelName];

            if (filePart is null && field.Count == 0)
            {
                BindMissingPart(context);
                return;
            }

            string? json;

            if (filePart is not null)
            {
                await using var stream = filePart.OpenReadStream();
                using var reader = new StreamReader(stream);

                json = await reader.ReadToEndAsync(context.HttpContext.RequestAborted);
            }
            else
            {
                json = field.Count == 1 ? field[0] : null;
            }

            if (String.IsNullOrWhiteSpace(json))
            {
                BindMissingPart(context);
                return;
            }

            try
            {
                var model = JsonSerializer.Deserialize(json, context.ModelType, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (model is null)
                {
                    BindMissingPart(context);
                    return;
                }

                context.Result = ModelBindingResult.Success(model);
            }
            catch (JsonException)
            {
                context.ModelState.TryAddModelError(context.ModelName, "Invalid JSON.");
            }
        }

        private static void BindMissingPart(ModelBindingContext context)
        {
            if (context.ModelMetadata.IsRequired)
                context.ModelState.TryAddModelError(context.ModelName, "A JSON object is required.");
            else
                context.Result = ModelBindingResult.Success(null);
        }
    }
}
