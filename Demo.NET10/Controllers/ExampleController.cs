using Microsoft.AspNetCore.Mvc;
using Hto3.OpenApi.JsonMultipartFormDataSupport;
using System.ComponentModel.DataAnnotations;

namespace Demo.NET10.Controllers
{
    [ApiController]
    [Route("example")]
    public class ExampleController : ControllerBase
    {
        /// <summary>
        /// Demonstrates basic JSON multipart form data with a single JSON object.
        /// </summary>
        /// <remarks>
        /// Send a JSON object as a form part named 'data'.
        /// Example request body:
        /// - Field name: data
        /// - Content-Type: application/json
        /// - Value: {"name": "John", "email": "john@example.com"}
        /// </remarks>
        [HttpPost("basic-json")]
        public async Task<IActionResult> UploadBasicJson([JsonFormPart] UserData data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new 
            { 
                message = "Basic JSON upload successful", 
                data 
            });
        }

        /// <summary>
        /// Demonstrates JSON multipart form data combined with an optional file upload.
        /// </summary>
        /// <remarks>
        /// Send both a JSON object and a file in the same multipart request.
        /// - Field 'metadata': JSON object (application/json)
        /// - Field 'file': Binary file data
        /// </remarks>
        [HttpPost("json-with-optional-file")]
        public async Task<IActionResult> UploadJsonWithOptionalFile([JsonFormPart] UploadMetadata metadata, [FromForm] IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fileInfo = new
            {
                fileName = file?.FileName,
                fileSize = file?.Length ?? -1,
                contentType = file?.ContentType
            };

            return Ok(new
            {
                message = "JSON with optional file upload successful",
                metadata,
                fileInfo
            });
        }

        /// <summary>
        /// Demonstrates JSON multipart form data combined with a file upload.
        /// </summary>
        /// <remarks>
        /// Send both a JSON object and a file in the same multipart request.
        /// - Field 'metadata': JSON object (application/json)
        /// - Field 'file': Binary file data
        /// </remarks>
        [HttpPost("json-with-required-file")]
        public async Task<IActionResult> UploadJsonWithRequiredFile([JsonFormPart] UploadMetadata metadata, [FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (file is null)
                return BadRequest("File is required.");

            var fileInfo = new
            {
                fileName = file.FileName,
                fileSize = file.Length,
                contentType = file.ContentType
            };

            return Ok(new
            {
                message = "JSON with required file upload successful",
                metadata,
                fileInfo
            });
        }

        /// <summary>
        /// Demonstrates multiple JSON objects in a single multipart request.
        /// </summary>
        /// <remarks>
        /// Send multiple JSON form parts with different field names.
        /// - Field 'user': User JSON object
        /// - Field 'settings': Settings JSON object
        /// </remarks>
        [HttpPost("multiple-json")]
        public async Task<IActionResult> UploadMultipleJson([JsonFormPart] UserData user, [JsonFormPart] UserSettings settings)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new
            {
                message = "Multiple JSON objects upload successful",
                user,
                settings
            });
        }

        /// <summary>
        /// Demonstrates optional JSON form parts.
        /// </summary>
        /// <remarks>
        /// One or both JSON form parts can be omitted.
        /// </remarks>
        [HttpPost("optional-json")]
        public async Task<IActionResult> UploadOptionalJson([JsonFormPart] UserData? user, [JsonFormPart] UserSettings? settings)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new
            {
                message = "Optional JSON upload processed",
                user = user,
                settings = settings
            });
        }

        /// <summary>
        /// Demonstrates JSON with arrays/collections in multipart form.
        /// </summary>
        /// <remarks>
        /// Send JSON containing collections like arrays or lists.
        /// </remarks>
        [HttpPost("json-with-arrays")]
        public async Task<IActionResult> UploadJsonWithArrays([JsonFormPart] UserData[]? batchData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new
            {
                message = "JSON with arrays upload successful",
                itemCount = batchData?.Length ?? 0,
                data = batchData
            });
        }

        /// <summary>
        /// Demonstrates JSON multipart with multiple files.
        /// </summary>
        /// <remarks>
        /// Send metadata JSON along with multiple file uploads.
        /// </remarks>
        [HttpPost("json-with-multiple-files")]
        public async Task<IActionResult> UploadJsonWithMultipleFiles([JsonFormPart] DocumentMetadata metadata, [FromForm] IFormFileCollection files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!files.Any())
                return BadRequest("At least one file is required.");

            var filesInfo = files.Select(f => new
            {
                fileName = f.FileName,
                fileSize = f.Length,
                contentType = f.ContentType
            }).ToList();

            return Ok(new
            {
                message = "JSON with multiple files upload successful",
                metadata,
                filesInfo,
                totalFiles = files.Count
            });
        }

        /// <summary>
        /// Demonstrates validation and error handling.
        /// </summary>
        /// <remarks>
        /// Test validation rules on JSON objects.
        /// </remarks>
        [HttpPost("with-validation")]
        public async Task<IActionResult> WithValidation([JsonFormPart] ValidatedData validatedData)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });

            return Ok(new
            {
                message = "Validated JSON upload successful",
                data = validatedData
            });
        }
    }

    // Data Models

    /// <summary>Basic user data model.</summary>
    public class UserData
    {
        public String? Name { get; set; }
        public String? Email { get; set; }
        public Int32 Age { get; set; }
    }

    /// <summary>User settings/preferences model.</summary>
    public class UserSettings
    {
        public Boolean NotificationsEnabled { get; set; }
        public String? Theme { get; set; }
        public String? Language { get; set; }
        public Dictionary<String, String>? CustomProperties { get; set; }
    }

    /// <summary>Upload metadata for file operations.</summary>
    public class UploadMetadata
    {
        public String? Title { get; set; }
        public String? Description { get; set; }
        public String? Category { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Document metadata.</summary>
    public class DocumentMetadata
    {
        public String? DocumentTitle { get; set; }
        public String? DocumentType { get; set; }
        public String? Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<String>? Tags { get; set; }
    }

    /// <summary>Individual data record.</summary>
    public class DataRecord
    {
        public String? Id { get; set; }
        public DateTime Timestamp { get; set; }
        public String? Content { get; set; }
        public Dictionary<String, object>? Metadata { get; set; }
    }

    /// <summary>Data model with validation attributes.</summary>
    public class ValidatedData
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public String? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public String? Email { get; set; }

        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
        public Int32 Age { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        public String? Website { get; set; }
    }
}
