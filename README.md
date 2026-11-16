![logo](https://raw.githubusercontent.com/HTO3/Hto3.OpenApi.JsonMultipartFormDataSupport/main/nuget-logo-small.png)

# Hto3.OpenApi.JsonMultipartFormDataSupport

Support JSON Multipart data in OpenAPI with ASP.NET Core.

## Overview

This library provides seamless integration for handling JSON data within multipart form requests in ASP.NET Core applications. It enables you to send JSON objects as form parts alongside file uploads and other multipart data, with automatic OpenAPI specification support.

## Installation

### NuGet Package

```bash
dotnet add package Hto3.OpenApi.JsonMultipartFormDataSupport
```

Or install via NuGet Package Manager:
```
Install-Package Hto3.OpenApi.JsonMultipartFormDataSupport
```

## Quick Start

### 1. Register in Startup Configuration

Add the `JsonMultipartOperationTransformer` to your OpenAPI configuration in `Program.cs`:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer<JsonMultipartOperationTransformer>();
});
```

### 2. Use the Attribute in Your Endpoints

```csharp
[HttpPost]
public async Task<IActionResult> UploadData([JsonFormPart] MyDataModel data, [FromForm] FormFile? file)
{
    // data is automatically deserialized from the JSON form part
    // file contains the uploaded file if provided

    return Ok(new { message = "Upload successful", data });
}

public class MyDataModel
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Description { get; set; }
}
```
## Requirements

- **.NET 10.0** or later
- **ASP.NET Core** with OpenAPI support
- **Microsoft.AspNetCore.OpenApi** v10.0.11 or later

## License

MIT License - See [LICENSE](LICENSE) file for details.