## AddS3Configuration Method Documentation

How to use the `AddS3Configuration` method to load configuration files into your application using the `IConfigurationBuilder` in .NET.

#### Method Signature

```csharp  
public static IConfigurationBuilder AddS3Configuration(  
    this IConfigurationBuilder builder,   
    string env,  
    string filePrefixName,   
    string subFolder = ""  
)  
```  
#### Parameters
|  Parameter | Type | Description
|---|---|---|
| `builder` | `IConfigurationBuilder` |The `IConfigurationBuilder` instance to which the configuration sources will be added. |
|`env`|`string`|The current environment (e.g., `Development`, `Staging`, `Production`) used to load environment-specific configuration files.|
|`filePrefixName`|`string`|The prefix name of the JSON configuration files.|
|`subFolder`|`string`|An optional subfolder where the configuration files are located. Defaults to an empty string.|

#### Behavior
This method performs the following actions:
1. Constructs the path to the configuration files using the `BasePath` and the `subFolder` parameter.
2. Adds a base configuration file with the name `<filePreFixName>.json`.
3. Adds an environment-specific configuration file with the name `<filePreFixName>.<env>.json`.
4. Both configuration files are optional but will reloaded on change if present.

### Example Usage
Here is an example of how to use the `AddS3Configuration`method in your .NET application:

**1.  Setting Up `BasePath`**
The `BasePath` constant is defined and points to the `AppDomain.CurrentDomain.BaseDirectory` which is the root folder of your application.

**2.  Adding to the Configuration Builder**
In your application start code (e.g. `Program.cs`), use the `AddS3Configuration` method:
```csharp
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
var configuration = builder
    .AddS3Configuration(env: "Development", filePrefixName: "appsettings", subFolder: "config")
    .Build();
```

***3. Accessing Configuration Values***
Once the configuration is built, you can access the values like so:
```csharp
string myvlaue = configuration["MyKey"];
Console.WriteLine($"Value: {myValue}");
```
### Notes
- Ensure that the configuration file exist at the specified path.
- The `subFolder` should be a set to a valid directory in your project or deployment environment.

### Troubleshooting
If configuration values are not loaded as expected:

- Verify that the `BasePath` and `subFolder`are correct.
- Ensure the files `<filePrefixName>.json` and `<filePrefixName>.<env>.json`exist in the specified directory.
- Check for typos in the keys or file names.