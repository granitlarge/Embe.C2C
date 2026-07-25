namespace Embe.C2C.Domain.Errors.Aggregates;

public static class SearchProfileErrors
{
    private static readonly string CodePrefix = "search_profile";
    public static readonly DomainError GendersEmpty = new($"{CodePrefix}.genders_empty", "At least 1 gender must be specified in a search profile");
    public static readonly DomainError AgeRangeInvalid = new($"{CodePrefix}.age_range_invalid", "The specified age range is invalid.");
    public static readonly DomainError GendersInvalid = new($"{CodePrefix}.genders_invalid", "The specified genders are invalid.");
    public static readonly DomainError OwnerDistanceFilterButLocationNotSet = new($"{CodePrefix}.owner_distance_filter_but_location_not_set", "The search profile owner has specified a distance filter, but their location is not set. The owner must set their location before specifying a distance filter.");
    public static readonly DomainError AddGenderAlreadyExists = new($"{CodePrefix}.add_gender_already_exists", "Attempted to add a gender to a search profile that already exists.");
    public static readonly DomainError RemoveGenderDoesNotExist = new($"{CodePrefix}.remove_gender_does_not_exist", "Attempted to remove a gender from a search profile that doesn't exist on the search profile.");
    public static readonly DomainError RemoveGenderExceedsMinimumCountOfOne = new($"{CodePrefix}.remove_gender_exceeds_minimum_count_of_one", "Attempted to remove a gender from a search profile with only one gender, which would make the total number of genders 0, which isn't allowed.");
}