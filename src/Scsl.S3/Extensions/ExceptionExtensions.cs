using System.Net;

using Amazon.S3;

using Scsl.S3.Models;

namespace Scsl.S3.Extensions;

internal static class ExceptionExtensions
{
    public static List<S3ClientError> ToS3ClientErrors(this AmazonS3Exception ex)
    {
        List<S3ClientError> errors = [];
        if (!ex.Message.Equals(
                "An error occurred while saving the entity changes. See the inner exception for details."))
        {
            errors.Add(new S3ClientError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
        }
        else
        {
            errors.Add(new S3ClientError()
            {
                Code = ex.StatusCode.ToString(), Description = ex.InnerException!.Message
            });
        }

        return errors;
    }
    
    public static List<S3ClientError> ToS3ClientErrors(this Exception ex)
    {
        List<S3ClientError> errors = [];
        if (!ex.Message.Equals(
                "An error occurred while saving the entity changes. See the inner exception for details."))
        {
            errors.Add(new S3ClientError()
            {
                Code = HttpStatusCode.InternalServerError.ToString(), Description = ex.Message
            });
        }
        else
        {
            errors.Add(new S3ClientError()
            {
                Code = HttpStatusCode.InternalServerError.ToString(), Description = ex.InnerException!.Message
            });
        }

        return errors;
    }
    
    public static List<S3ClientError> FailErrors()
    {
        List<S3ClientError> errors = [];
        var error = new S3ClientError()
        {
            Code = HttpStatusCode.InternalServerError.ToString(), Description = "Internal Server Error"
        };

        errors.Add(error);
        return errors;
    }
    
    public static List<S3ClientError> FileNotFoundError()
    {
        List<S3ClientError> errors = [];
        var error = new S3ClientError() { Code = HttpStatusCode.NotFound.ToString(), Description = "File not found." };

        errors.Add(error);
        return errors;
    }
}