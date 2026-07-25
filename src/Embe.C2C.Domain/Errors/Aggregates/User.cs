namespace Embe.C2C.Domain.Errors.Aggregates;

public static class UserErrors
{
    public static readonly DomainError AgeOutOfRange = new("user.age_out_of_range", "The specified user age must be between 18 and 120.");
    public static readonly DomainError InvalidFileCount = new("user.invalid_file_count", "The specified user has an invalid number of files.");
}