using ErrorOr;

namespace Embe.C2C.Application.Extensions;

public static class ErrorExtensions
{
    public static Error WithPropertyName(this Error error, string propertyName)
    {
        var metadata = error.Metadata;
        if (metadata != null)
        {
            metadata["PropertyName"] = propertyName;
            return error;
        }

        metadata = new Dictionary<string, object> { { "PropertyName", propertyName } };
        return error.Type switch
        {
            ErrorType.Validation => Error.Validation(error.Code, error.Description, metadata),
            ErrorType.NotFound => Error.NotFound(error.Code, error.Description, metadata),
            ErrorType.Conflict => Error.Conflict(error.Code, error.Description, metadata),
            ErrorType.Failure => Error.Failure(error.Code, error.Description, metadata),
            ErrorType.Unauthorized => Error.Unauthorized(error.Code, error.Description, metadata),
            ErrorType.Forbidden => Error.Forbidden(error.Code, error.Description, metadata),
            ErrorType.Unexpected => Error.Unexpected(error.Code, error.Description, metadata),
            _ => error
        };
    }

    public static IEnumerable<Error> WithPropertyName(this IEnumerable<Error> errors, string propertyName)
    {
        return errors.Select(error => error.WithPropertyName(propertyName));
    }
}