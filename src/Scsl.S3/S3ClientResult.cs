using System.Globalization;

using Scsl.S3.Models;

namespace Scsl.S3;

public class S3ClientResult
{
    private readonly List<S3ClientError> _errors = [];

    /// <summary>
    /// Flag indicating whether if the operation succeeded or not.
    /// </summary>
    /// <value>True if the operation succeeded, otherwise false.</value>
    public bool Succeeded { get; protected set; }

    /// <summary>
    /// An <see cref="IEnumerable{T}"/> of <see cref="S3ClientError"/> instances containing errors
    /// that occurred during the database operation.
    /// </summary>
    /// <value>An <see cref="IEnumerable{T}"/> of <see cref="S3ClientError"/> instances.</value>
    public IEnumerable<S3ClientError> Errors => _errors;

    /// <summary>
    /// Returns an <see cref="S3ClientResult"/> indicating a successful database operation.
    /// </summary>
    /// <returns>An <see cref="S3ClientResult"/> indicating a successful operation.</returns>
    public static S3ClientResult Success { get; } = new() { Succeeded = true };

    /// <summary>
    /// Creates an <see cref="S3ClientResult"/> indicating a failed database operation, with a list of <paramref name="errors"/> if applicable.
    /// </summary>
    /// <param name="errors">An optional array of <see cref="S3ClientError"/>s which caused the operation to fail.</param>
    /// <returns>An <see cref="S3ClientResult"/> indicating a failed database operation, with a list of <paramref name="errors"/> if applicable.</returns>
    public static S3ClientResult Failed(List<S3ClientError> errors1, params S3ClientError[] errors)
    {
        var result = new S3ClientResult { Succeeded = false };
        if (errors != null)
        {
            result._errors.AddRange(errors);
        }
        return result;
    }

    internal static S3ClientResult Failed(List<S3ClientError>? errors)
    {
        var result = new S3ClientResult { Succeeded = false };
        if (errors != null)
        {
            result._errors.AddRange(errors);
        }
        return result;
    }

    /// <summary>
    /// Converts the value of the current <see cref="S3ClientResult"/> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the current <see cref="S3ClientResult"/> object.</returns>
    /// <remarks>
    /// If the operation was successful the ToString() will return "Succeeded" otherwise it returned
    /// "Failed : " followed by a comma delimited list of error codes from its <see cref="Errors"/> collection, if any.
    /// </remarks>
    public override string ToString()
    {
        return Succeeded ?
               "Succeeded" :
               string.Format(CultureInfo.InvariantCulture, "{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
    }
}