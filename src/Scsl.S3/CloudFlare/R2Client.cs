using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using Microsoft.Extensions.Options;

using Scsl.S3.Contacts;
using Scsl.S3.Extensions;

namespace Scsl.S3.CloudFlare;

public class R2Client : IR2Client, IDisposable
{
    private readonly AmazonS3Client _s3Client;

    /// <summary>
    /// Initializes a new instance of the <see cref="R2Client"/> class with the specified S3 configuration options.
    /// </summary>
    /// <param name="options">An <see cref="IOptions{TOptions}"/> containing the S3 configuration options.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when any required option (PublicEndpoint, AccessKeyId, SecretAccessKey, or Endpoint) is null or empty.
    /// </exception>
    /// <remarks>
    /// This constructor validates the provided options and sets up the Cloudflare R2 client with the specified credentials and endpoint configuration.
    /// </remarks>
    public R2Client(IOptions<R2Options> options)
    {
        ArgumentException.ThrowIfNullOrEmpty(options.Value.PublicEndpoint, nameof(options));
        ArgumentException.ThrowIfNullOrEmpty(options.Value.AccessKeyId, nameof(options));
        ArgumentException.ThrowIfNullOrEmpty(options.Value.SecretAccessKey, nameof(options));
        ArgumentException.ThrowIfNullOrEmpty(options.Value.Endpoint, nameof(options));

        var credentials = new BasicAWSCredentials(options.Value.AccessKeyId, options.Value.SecretAccessKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config { ServiceURL = options.Value.Endpoint });
    }
    
    /// <summary>
    /// Uploads a file to Cloudflare R2 bucket asynchronously.
    /// </summary>
    /// <param name="bucketName">The name of the S3 bucket.</param>
    /// <param name="key">The object key (path/file in the bucket).</param>
    /// <param name="localFilePath">The local file path of the object to upload.</param>
    /// <returns>A <see cref="S3ClientResult"/> indicating the success or failure of the operation.</returns>
    /// <exception cref="AmazonS3Exception">Thrown if an Amazon S3 error occurs during the upload. </exception>
    /// <exception cref="Exception">Thrown if an unknown error occurs during the upload.</exception>
    public async Task<S3ClientResult> PutObjectAsync(string localFilePath, string bucketName, string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(localFilePath, nameof(localFilePath));
        ArgumentException.ThrowIfNullOrEmpty(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName, Key = key, FilePath = localFilePath, DisablePayloadSigning = true
            };
            await _s3Client.PutObjectAsync(request);
            return S3ClientResult.Success;
        }
        catch (AmazonS3Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
        catch (Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
    }

    /// <summary>
    /// Uploads a file to Cloudflare R2 bucket synchronously.
    /// </summary>
    /// <param name="bucketName">The name of the S3 bucket.</param>
    /// <param name="key">The object key (path/file in the bucket).</param>
    /// <param name="localFilePath">The local file path of the object to upload.</param>
    /// <returns>An <see cref="S3ClientResult"/> indicating the success or failure of the operation.</returns>
    /// <exception cref="AmazonS3Exception">Thrown if an Amazon S3 error occurs during the upload.</exception>
    /// <exception cref="Exception">Thrown if an unknown error occurs during the upload.</exception>
    public S3ClientResult PutObject(string localFilePath, string bucketName, string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(localFilePath, nameof(localFilePath));
        ArgumentException.ThrowIfNullOrEmpty(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        
        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                FilePath = localFilePath, BucketName = bucketName, Key = key, DisablePayloadSigning = true
            };

            using var fileTransferUtility = new TransferUtility(_s3Client);
            fileTransferUtility.Upload(uploadRequest);

            return S3ClientResult.Success;
        }
        catch (AmazonS3Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
        catch (Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
    }

    /// <summary>
    /// Uploads a file to Cloudflare R2 bucket asynchronously.
    /// </summary>
    /// <param name="fileStream">The stream containing the file to upload.</param>
    /// <param name="bucketName">The name of the S3 bucket.</param>
    /// <param name="key">The object key (path/file in the bucket)</param>
    /// <returns>An <see cref="S3ClientResult"/> indicating the success or failure of the operation.</returns>
    /// <exception cref="AmazonS3Exception">Thrown if an Amazon S3 error occurs during the upload.</exception>
    /// <exception cref="Exception">Thrown if an unknown error occurs during the upload.</exception>
    public async Task<S3ClientResult> PutObjectAsync(Stream fileStream, string bucketName, string key)
    {
        ArgumentNullException.ThrowIfNull(fileStream, nameof(fileStream));
        ArgumentException.ThrowIfNullOrEmpty(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = fileStream, Key = key, BucketName = bucketName, DisablePayloadSigning = true
            };

            using var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            return S3ClientResult.Success;
        }
        catch (AmazonS3Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
        catch (Exception e)
        {
            S3ClientResult.Failed(e.ToS3ClientErrors());
        }

        return S3ClientResult.Failed(ExceptionExtensions.FailErrors());
    }

    /// <summary>
    /// Uploads a file to Cloudflare R2 bucket synchronously.
    /// </summary>
    /// <param name="fileStream">The stream containing the file to upload.</param>
    /// <param name="bucketName">The name of the S3 bucket.</param>
    /// <param name="key">The key under which the object will be stored.</param>
    /// <returns>An <see cref="S3ClientResult"/> indicating the success or failure of the operation.</returns>
    /// <exception cref="AmazonS3Exception">Thrown if an Amazon S3 error occurs during the upload.</exception>
    /// <exception cref="Exception">Thrown if an unknown error occurs during the upload.</exception>
    public S3ClientResult PutObject(Stream fileStream, string bucketName, string key)
    {
        ArgumentNullException.ThrowIfNull(fileStream, nameof(fileStream));
        ArgumentException.ThrowIfNullOrEmpty(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = fileStream, Key = key, BucketName = bucketName, DisablePayloadSigning = true
            };

            using var fileTransferUtility = new TransferUtility(_s3Client);
            fileTransferUtility.Upload(uploadRequest);

            return S3ClientResult.Success;
        }
        catch (AmazonS3Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
        catch (Exception e)
        {
            S3ClientResult.Failed(e.ToS3ClientErrors());
        }

        return S3ClientResult.Failed(ExceptionExtensions.FailErrors());
    }

    /// <summary>
    /// Asynchronously deletes an object from a Cloudflare R2 bucket.
    /// </summary>
    /// <param name="bucketName">The name of the S3 bucket containing the object to be deleted.</param>
    /// <param name="key">The key (or unique identifier) of the object to be deleted.</param>
    /// <returns>
    /// A <see cref="DeleteObjectResponse"/> containing the result of the delete operation, 
    /// including the HTTP status code.
    /// </returns>
    /// <exception cref="AmazonS3Exception">
    /// Thrown when an error is encountered on the server during the delete operation.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown for any unknown errors encountered during the delete operation.
    /// </exception>
    /// <remarks>
    /// Logs debug information when initiating the delete operation and logs errors
    /// for both known and unknown exceptions. In case of an error, a response with
    /// <see cref="System.Net.HttpStatusCode.BadRequest"/> is returned.
    /// </remarks>
    public async Task<S3ClientResult> DeleteObjectAsync(string bucketName, string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        
        try
        {
            var request = new DeleteObjectRequest { BucketName = bucketName, Key = key};
            if (!await _s3Client.IsFileExists(bucketName, key))
            {
                return S3ClientResult.Failed(ExceptionExtensions.FileNotFoundError());
            }
            await _s3Client.DeleteObjectAsync(request);
            return S3ClientResult.Success;
        }
        catch (AmazonS3Exception e)
        {
            return S3ClientResult.Failed(e.ToS3ClientErrors());
        }
        catch (Exception e)
        {
            S3ClientResult.Failed(e.ToS3ClientErrors());
        }

        return S3ClientResult.Failed(ExceptionExtensions.FailErrors());
    }

    /// <summary>
    /// Releases resources used by the instance, including the associated AWS S3 client.
    /// Suppresses finalization to prevent redundant garbage collection operations.
    /// </summary>
    public void Dispose()
    {
        _s3Client.Dispose();
        GC.SuppressFinalize(this);
    }
}