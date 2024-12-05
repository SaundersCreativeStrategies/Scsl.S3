Using the R2Client Class for Cloudflare's S3-Compatible Storage
=======
![NuGet Downloads](https://img.shields.io/nuget/dt/Scsl.S3)  ![NuGet Version](https://img.shields.io/nuget/v/Scsl.S3)

This guide explains how to use the `R2Client` class to interact with an S3-compatible storage system. It includes instructions for configuring your application with dependency injection (DI), adding dynamic S3-based configuration, and using methods for uploading and deleting objects.

### Perquisites
1. **Install Scsl.S3 SDK for .NET**
   You should install [Scsl.S3 with NuGet](https://www.nuget.org/packages/Scsl.S3/):
   ```  
   Install-Package Scsl.S#
   ```  
   Or via .NET Core command line interface:
     ```
     dotnet add package Scsl.S3 
     ```
   Either commands, from Package Manager Console or .NET Core CLI, will download and install Scsl.S3 and all required dependencies.


2. **Configure `R2Options` Options in appsettings.json**
   Ensure your application includes an `R2Options` section. Example:
    ```json
	  {  
	      "R2Options":  
	      {  
	        "PublicEndpoint": "https://example-public-endpoint",  
	        "BucketName": "string",  
	        "AccessKeyId": "string",  
	        "SecretAccessKey": "string",  
	        "Endpoint": "https://<ACCOUNT_ID>.r2.cloudflarestorage.com/"  
	      }  
	  }  
	```
---
### Register Options in Dependency Injection (DI) Container:
Configure `R2Options` in the DI container so it can be injected in the the `R2Client` constructor.

**Register the configuration in your `Startup.cs` or `Program.cs`**:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<R2Options>(builder.Configuration.GetSection(R2Options.Name));
builder.Services.AddScoped<IR2Client, R2Client>();  
```
### S3-Based Configuration
Use the `AddS3Configuration` extension method to dynamically load configuration files based on the environment and file structure. See [here](https://github.com/SaundersCreativeStrategies/Scsl.S3/blob/master/docs/s3configuration.md) for usage.

### Extension Method: `AddS3Configuration`
**Method Signature**
```csharp
public static IConfigurationBuilder AddS3Configuration(
    this IConfigurationBuilder builder, 
    string env, 
    string filePrefixName, 
    string subFolder = ""
)
```
**Parameters**
- `env`: The current environment (e.g. `Development`, `Production`).
- `filePrefixName`: The prefix of the configuration file (e.g. `appsettings`).
- `subFolder`: (Optional) Subfolder where configuration files are stored.

**Behavior**
- Loads `appsettings.json` and `appsettings.{env}.json` from the specified `subFolder`.
- Defaults to the base path if no `subFolder is provided.`
---
### Example Setup in `Programs.cs`
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddS3Configuration( 
	env: builder.Environment.EnvironmentName, 
	filePrefixName: "appsettings", 
	subFolder: "config" 
);

builder.Services.Configure<R2Options>(builder.Configuration.GetSection(R2Options.Name));
```
This configuration approach dynamically adapts to different environments.

### Initialing the `R2Client`
The `R2Client` constructor resolves `IOptions<R2Options>` automatically from DI. Once configured, it’s ready for use in your application.

**Example Initialization in a Controller**
```csharp
public class S3Controller 
{ 
	private readonly R2Client _r2Client;
	
	public S3Controller(R2Client r2Client) 
	{ 
	_r2Client = r2Client; 
	}
}
```
---
### Uploading Files to S3-Compatible Storage
The `R2Client` provides two methods for uploading files:

**Method 1**: `PutObjectAsync` (**Local File Path**)
Uploads a file to the specified bucket using its local file path.

**Method Signature**
```csharp
public async Task<S3ClientResult> PutObjectAsync(
	string localFilePath, 
	string bucketName, 
	string key
)
```
**Parameters**
- `localFilePath`: Fill path to the local file.
- `bucketName`: Name of the target bucket.
- `key`: Object key (filename or folder/filename) in the bucket.

**Example Usage**
```csharp
var localFilePath = @"C:\path\to\file.txt";
var bucketName = "example-bucket";
var key = "folder/file.txt";

var result = await _r2Client.PutObjectAsync(localFilePath, bucketName, key);

if (result == S3ClientResult.Success)
{
    Console.WriteLine("File uploaded successfully.");
}
else
{
    Console.WriteLine("File upload failed.");
}
```
---
**Method 2**: `PutObjectAsync` (**Stream**)
Upload a file to the specified bucket using a `Stream`.

**Method Signature**
 ```csharp
 public async Task<S3ClientResult> PutObjectAsync(Stream fileStream, string bucketName, string key)
 ```
**Parameters**
- `fileStream`: A `Stream` containing the file data.
- `bucketName`: Name of the target bucket.
- `key`: Object key (filename or folder/filename) in the bucket.

**Example Usage**
```csharp
using var fileStream = File.OpenRead(@"C:\path\to\file.txt");
var bucketName = "example-bucket";
var key = "folder/file.txt";

var result = await _r2Client.PutObjectAsync(fileStream, bucketName, key);

if (result == S3ClientResult.Success)
{
    Console.WriteLine("Stream uploaded successfully.");
}
else
{
    Console.WriteLine("Stream upload failed.");
}
```
---
### Deleting Files from S3-Compatible Storage
**Method**: `DeleteObjectAsync`
Removes an object from the specified bucket.

**Method Signature**
```csharp
public async Task<S3ClientResult> DeleteObjectAsync(string bucketName, string key)
```
**Example Usage**
```csharp
var bucketName = "example-bucket"; 
var key = "folder/file.txt";

var result = await _r2Client.DeleteObjectAsync(bucketName, key);
if (result == S3ClientResult.Success) 
{ 
	Console.WriteLine("File deleted successfully."); 
} 
else 
{ 
	Console.WriteLine("File deletion failed."); 
}
```
---
### Error Handling
Both `PutObjectAsync` methods handle exceptions from:
1. **AmzonS3Exception**: Errors specific to the S3 service, such as invalid bucket names or key.
2. **General Exceptions**: Catches other runtime errors.
### Logging Errors
Inspect the returned `S3ClientResult.Failed` to log or handle errors:
 ```csharp
 if (result == S3ClientResult.Failed)
{
    Console.WriteLine($"Error: {result.Errors}");
}
 ```
### Notes
-  **Dependencies**: Ensure Cloudflare R2 credentials and endpoint are valid.
-  **Bucket Policy**: Verify that the target bucket has appropriate permissions.
- **Existence Check**: The `DeleteObjectAsync` method use `S3Client.IsFileExists` to confirm file existence before deletion.
 
