# Scsl.S3
This lightweight library provides seamless access to Cloudflare R2 Object Storage, enabling you to efficiently upload (Put) and delete objects.

### Installation
```
dotnet add package 
```

### Using the Cloudflare R2 Library: A Quick Guide
Program.cs
```csharp
using Scsl.S3;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.Configure<R2Options>(builder.Configuration.GetSection(R2Options.Name));
builder.Services.AddScoped<IR2Client, R2Client>();
```
appsettings.json
```json
{
    "R2Options":
    {
      "PublicEndpoint": "https://sub.domain.tld",
      "BucketName": "string",
      "AccessKeyId": "string",
      "SecretAccessKey": "string",
      "Endpoint": "https://<ACCOUNT_ID>.r2.cloudflarestorage.com/"
    }
}
```
FileUploadFromModel.cs
```csharp
public class FileuploadFromModel
{
    [Required]
    [Display(Name="File"])
    public IFormFile FormFile {get; set;}
}
```
Constructor 
```csharp
private readonly IR2Cleint _cleint;
private readonly IOptions<R2Options> _options
    
public Constructor(IR2Client client, IOptions<R2Options> options)
{
   _client = client;
   _options = options;
}
```
PutObject using MemoryStream
```csharp
public async Task<IActionResult> PutObject(FileUploadFromModel file)
{
    using var stream = new MemoryStream();
    await file.FormFile.CopyToAsync(stream);
    var results = await _client.PutObject(stream, _options.Value.BucketName, "Upload/file.ext");
    if(!results.Succeeded) return BadRequest(new Json(results.Errors.ToList());
    
    return HttpStatusCode.Created;
}
```
DeleteObject
```csharp
public async Task<IActionResult> DeleteObject(string file)
{
    var results = await _client.DeleteObject(_options.BucketName, file);
    if(!results.Succeeded) return BadRequest(new Json(results.Errors.ToList());
    
    return HttpStatusCode.NoContent;
}
```
R3ClientResult response
```json
{
  "Success": bool,
  "Errors": [
    { "Code": "string", "Description":  "string" }
  ]
}
```


