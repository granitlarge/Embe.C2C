using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Candidates;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record CandidateDto
(
    Guid Id,
    Guid UserId,
    Guid CandidateUserId,
    Guid UserSearchProfileId,
    Guid CandidateSearchProfileId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    bool? Judgement,
    ReadDto<UserDto, UserPermission>? User,
    ReadDto<UserDto, UserPermission>? Candidate,
    ReadDto<SearchProfileDto, SearchProfilePermission>? UserSearchProfile,
    ReadDto<SearchProfileDto, SearchProfilePermission>? CandidateSearchProfile
);

public class CandidateDtoMapper
{
    public CandidateDtoMapper()
    {

    }

    public CandidateDto? ToDto
    (
        Candidate candidate,
        CandidateVariant variant,
        ReadDto<UserDto, UserPermission>? userDto,
        ReadDto<UserDto, UserPermission>? candidateDto,
        ReadDto<SearchProfileDto, SearchProfilePermission>? userSearchProfileDto,
        ReadDto<SearchProfileDto, SearchProfilePermission>? candidateSearchProfileDto
    )
    {

        if (variant == CandidateVariant.Empty)
            return null;

        return new CandidateDto
        (
            candidate.Id,
            candidate.UserId,
            candidate.CandidateUserId,
            candidate.UserSearchProfileId,
            candidate.CandidateSearchProfileId,
            variant.IncludeCreatedAt ? candidate.CreatedAt : null,
            variant.IncludeUpdatedAt ? candidate.UpdatedAt : null,
            variant.IncludeJudgement ? candidate.Judgement : null,
            userDto,
            candidateDto,
            userSearchProfileDto,
            candidateSearchProfileDto
        );

    }
}