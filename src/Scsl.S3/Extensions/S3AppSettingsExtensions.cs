// ref: https://jd-bots.com/2022/01/30/fixed-configurationbuilder-does-not-contain-a-definition-for-setbasepath/
using Microsoft.Extensions.Configuration;

namespace Scsl.S3.Extensions;

public static class S3AppSettingsExtensions
{
    private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Extends <see cref="IConfigurationBuilder"/> to add JSON configuration files from a specified S3 path.
    /// Combines the base path, optional sub-folder, and file prefix to determine file locations.
    /// </summary>
    /// <param name="builder">The configuration builder to extend.</param>
    /// <param name="env">The environment name to load environment-specific configuration.</param>
    /// <param name="subFolder">The optional sub-folder within the base path.</param>
    /// <param name="filePrefixName">The prefix for the configuration file names.</param>
    /// <returns>The updated <see cref="IConfigurationBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="builder"/> is null. </exception>
    /// <exception cref="DirectoryNotFoundException"> Thrown if the specified path or sub-folder does not exist.</exception>
    /// <exception cref="FileNotFoundException"> Thrown if any required configuration file is missing.</exception>
    public static IConfigurationBuilder AddS3Configuration(this IConfigurationBuilder builder, string env,
        string filePrefixName, string subFolder = "") 
    {
        string path = string.IsNullOrEmpty(subFolder) ? BasePath : Path.Combine(BasePath, subFolder);

        return builder
            .SetBasePath(path)
            .AddJsonFile($"{filePrefixName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{filePrefixName}.{env}.json", optional: true, reloadOnChange: true);
    }
}