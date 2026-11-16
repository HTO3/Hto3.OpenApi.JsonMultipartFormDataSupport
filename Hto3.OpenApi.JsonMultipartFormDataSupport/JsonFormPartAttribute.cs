using Microsoft.AspNetCore.Mvc;

namespace Hto3.OpenApi.JsonMultipartFormDataSupport
{
    /// <summary>
    /// Marks a controller action parameter to be bound from a JSON object within a multipart form data request.
    /// </summary>
    /// <remarks>
    /// The JSON value should be sent as a form field with the same name as the parameter.
    /// It can be provided either as a file upload or as a form field value.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class JsonFormPartAttribute : ModelBinderAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormPartAttribute"/> class.
        /// </summary>
        public JsonFormPartAttribute() : base(typeof(JsonFormPartBinder))
        {
        }
    }
}
