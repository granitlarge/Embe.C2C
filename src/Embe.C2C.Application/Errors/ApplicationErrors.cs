namespace Embe.C2C.Application.Errors;

public static class ApplicationErrors
{
    public static readonly ApplicationError NotFound = new("not_found", "The requested resource was not found.");
    public static readonly ApplicationError Forbidden = new("forbidden", "The authenticated user does not have access to the requested resource.");
}