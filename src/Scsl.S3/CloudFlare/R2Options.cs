namespace Scsl.S3.CloudFlare;

#nullable disable
public class R2Options
{
    public const string Name = "R2Options";
    
    public string PublicEndpoint { get; set; }
    public string BucketName { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string Endpoint { get; set; }
}