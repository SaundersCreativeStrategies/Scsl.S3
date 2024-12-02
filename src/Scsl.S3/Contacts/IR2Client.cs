namespace Scsl.S3.Contacts;

public interface IR2Client
{
    Task<S3ClientResult> DeleteObjectAsync(string bucketName, string key);
    S3ClientResult PutObject(Stream fileStream, string bucketName, string key);
    Task<S3ClientResult> PutObjectAsync(Stream fileStream, string bucketName, string key);
    S3ClientResult PutObject(string localFilePath, string bucketName, string key);
    Task<S3ClientResult> PutObjectAsync(string localFilePath, string bucketName, string key );
}