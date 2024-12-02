using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Scsl.S3.CloudFlare;
using Scsl.S3.Contacts;

namespace Scsl.S3.UnitTest;

[TestClass]
public class DeleteObjectFromBucketTests
{
    private ServiceProvider _serviceProvider  = null!;

    [TestInitialize]
    public void Setup()
    {
        //Loads configuration file
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // set up DI container
        var services = new ServiceCollection();

        // Register configuration options
        services.Configure<R2Options>(config.GetSection(R2Options.Name));

        //Register other dependencies (e.g., R2Client)
        services.AddScoped<IR2Client, R2Client>();
    
        //Build ServiceProvider
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public void DeleteObjectFromBucket_SuccessIsOk()
    {
        // Get R2Client Options via IOptions
        var options = _serviceProvider.GetRequiredService<IOptions<R2Options>>().Value;
        
        // Verifies that options are successfully loaded from the configuration
        Assert.IsNotNull(options.PublicEndpoint);
        Assert.IsNotNull(options.Endpoint);
        Assert.IsNotNull(options.BucketName);
        Assert.IsNotNull(options.AccessKeyId);
        Assert.IsNotNull(options.SecretAccessKey);
        
        // Test IR2Client Functionality
        const string key = "profile/8C9E189D-9F08-415E-9788-6D4E4B87BDE7/D6BEFA01-650D-49F9-8EE3-39FCDDAF74CB.jpg";
        var s3Client = _serviceProvider.GetRequiredService<IR2Client>();
        var results = s3Client.DeleteObjectAsync(options.BucketName, key).Result;
        
        Assert.IsTrue(results.Succeeded);
    }
    
    [TestMethod]
    public void DeleteObjectFromBucket_SuccessFailed()
    {
        // Get R2Client Options via IOptions
        var options = _serviceProvider.GetRequiredService<IOptions<R2Options>>().Value;
        
        // Verifies that options are successfully loaded from the configuration
        Assert.IsNotNull(options.PublicEndpoint);
        Assert.IsNotNull(options.Endpoint);
        Assert.IsNotNull(options.BucketName);
        Assert.IsNotNull(options.AccessKeyId);
        Assert.IsNotNull(options.SecretAccessKey);
        
        // Test IR2Client Functionality
        const string key = "profile/8C9E189D-9F08-415E-9788-6D4E4B87BDE7/D6BEFA01-650D-49F9-8EE3-39FCDDAF74CB.jpg";
        var s3Client = _serviceProvider.GetRequiredService<IR2Client>();
        var results = s3Client.DeleteObjectAsync(options.BucketName, key).Result;
        
        Assert.IsTrue(!results.Succeeded);
    }

    [TestMethod]
    public void Cleanup()
    {
        if (_serviceProvider is IDisposable disposable)
            disposable.Dispose();
    }
}