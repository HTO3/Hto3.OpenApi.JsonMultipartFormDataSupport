using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hto3.OpenApi.JsonMultipartFormDataSupport
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class JsonFormPartAttribute : ModelBinderAttribute
    {
        public JsonFormPartAttribute() : base(typeof(JsonFormPartBinder))
        {
        }
    }
}
