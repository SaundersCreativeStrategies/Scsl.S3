using Amazon.S3;
using Amazon.S3.Model;

namespace Scsl.S3.Extensions;

internal static class IoExtensions
{
    public static Task<bool> IsFileExists(this AmazonS3Client s3Client, string bucketName, string key)
    {
        ArgumentNullException.ThrowIfNull(s3Client);
		ArgumentException.ThrowIfNullOrEmpty(bucketName);
        ArgumentException.ThrowIfNullOrEmpty(key);

        try
        {
            GetObjectMetadataRequest request = new() { BucketName = bucketName, Key = key };
            Task.FromResult(s3Client.GetObjectMetadataAsync(request).Result);
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            if (e.InnerException is not AmazonS3Exception awsEx) throw;

            if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                return Task.FromResult(false);
            
            else if (string.Equals(awsEx.ErrorCode, "NotFound"))
                return Task.FromResult(false);

            throw;
        }
    }
}