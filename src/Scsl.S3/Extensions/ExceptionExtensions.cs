using System.Net;

using Amazon.S3;

using Scsl.S3.Models;

namespace Scsl.S3.Extensions;

internal static class ExceptionExtensions
{
    /// <summary>
    /// Converts an <see cref="AmazonS3Exception"/> into a list of <see cref="S3ClientError"/> objects,
    /// capturing the error code and message details.
    /// </summary>
    /// <param name="ex">The <see cref="AmazonS3Exception"/> to process.</param>
    /// <returns>A list of <see cref="S3ClientError"/> objects with error details.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the exception parameter is null.</exception>
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
    
    /// <summary>
    /// Converts an exception to a list of <see cref="S3ClientError"/> objects for error reporting.
    /// </summary>
    /// <param name="ex">The exception to convert.</param>
    /// <returns>A list of <see cref="S3ClientError"/> objects containing error details.</returns>
    /// <exception cref="NullReferenceException">Thrown if the inner exception is null when accessing its message.</exception>
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
    
    /// <summary>
    /// Generates a list containing a single S3 client error indicating an internal server error.
    /// </summary>
    /// <returns>A list of <see cref="S3ClientError"/> objects with a predefined error.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the list initialization fails.</exception>
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
    
    /// <summary>
    /// Creates a list containing a single S3 client error indicating a file not found.
    /// </summary>
    /// <returns>A list of <see cref="S3ClientError"/> with an error code and description for "File Not Found".</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="S3ClientError"/> object initialization fails.
    /// </exception>
    public static List<S3ClientError> FileNotFoundError()
    {
        List<S3ClientError> errors = [];
        var error = new S3ClientError() { Code = HttpStatusCode.NotFound.ToString(), Description = "File not found." };

        errors.Add(error);
        return errors;
    }
}