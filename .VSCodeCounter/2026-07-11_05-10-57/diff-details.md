# Diff Details

Date : 2026-07-11 05:10:57

Directory c:\\Users\\Miro\\source\\repos\\Embe.C2C

Total : 245 files,  992 codes, 108 comments, -822 blanks, all 278 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [data/scripts/LoadADMFileToDb.sql](/data/scripts/LoadADMFileToDb.sql) | MS SQL | 26 | 0 | 0 | 26 |
| [data/scripts/gadm\_add\_centroid.py](/data/scripts/gadm_add_centroid.py) | Python | 27 | 53 | 11 | 91 |
| [package-lock.json](/package-lock.json) | JSON | 756 | 0 | 1 | 757 |
| [package.json](/package.json) | JSON | 5 | 0 | 1 | 6 |
| [src/Embe.C2C.Api/EndPoints/GeographyEndPoints.cs](/src/Embe.C2C.Api/EndPoints/GeographyEndPoints.cs) | C# | 49 | 0 | 7 | 56 |
| [src/Embe.C2C.Api/EndPoints/SearchProfileEndPoints.cs](/src/Embe.C2C.Api/EndPoints/SearchProfileEndPoints.cs) | C# | 62 | 0 | 7 | 69 |
| [src/Embe.C2C.Api/EndPoints/UserEndPoints.cs](/src/Embe.C2C.Api/EndPoints/UserEndPoints.cs) | C# | 20 | 0 | 3 | 23 |
| [src/Embe.C2C.Api/Program.cs](/src/Embe.C2C.Api/Program.cs) | C# | 3 | 0 | 0 | 3 |
| [src/Embe.C2C.Application/Abstractions/Entities/IAdminArea.cs](/src/Embe.C2C.Application/Abstractions/Entities/IAdminArea.cs) | C# | 12 | 0 | 1 | 13 |
| [src/Embe.C2C.Application/Abstractions/Repos/Repository.cs](/src/Embe.C2C.Application/Abstractions/Repos/Repository.cs) | C# | 19 | 0 | 0 | 19 |
| [src/Embe.C2C.Application/Authorizations/FactGenerators/JudgementAuthorizationFactGenerator.cs](/src/Embe.C2C.Application/Authorizations/FactGenerators/JudgementAuthorizationFactGenerator.cs) | C# | 35 | 0 | 4 | 39 |
| [src/Embe.C2C.Application/Authorizations/FactGenerators/SearchProfileFactGenerator.cs](/src/Embe.C2C.Application/Authorizations/FactGenerators/SearchProfileFactGenerator.cs) | C# | 36 | 0 | 6 | 42 |
| [src/Embe.C2C.Application/Authorizations/FactStores/Judgements/JudgementAuthorizationFactStore.cs](/src/Embe.C2C.Application/Authorizations/FactStores/Judgements/JudgementAuthorizationFactStore.cs) | C# | -1 | 0 | -3 | -4 |
| [src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/Facts/IsCandidateForUserFact.cs](/src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/Facts/IsCandidateForUserFact.cs) | C# | 2 | 1 | 2 | 5 |
| [src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/Facts/IsMatchedFact.cs](/src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/Facts/IsMatchedFact.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/Facts/IsOwnerFact.cs](/src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/Facts/IsOwnerFact.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/SearchProfileAuthorizationFactStore.cs](/src/Embe.C2C.Application/Authorizations/FactStores/SearchProfiles/SearchProfileAuthorizationFactStore.cs) | C# | 59 | 0 | 12 | 71 |
| [src/Embe.C2C.Application/Authorizations/JudgementAuthorizationPolicy.cs](/src/Embe.C2C.Application/Authorizations/JudgementAuthorizationPolicy.cs) | C# | -70 | 0 | -13 | -83 |
| [src/Embe.C2C.Application/Authorizations/JudgementAuthorizationService.cs](/src/Embe.C2C.Application/Authorizations/JudgementAuthorizationService.cs) | C# | 65 | 21 | 11 | 97 |
| [src/Embe.C2C.Application/Authorizations/MatchingAuthorizationPolicy.cs](/src/Embe.C2C.Application/Authorizations/MatchingAuthorizationPolicy.cs) | C# | -108 | 0 | -16 | -124 |
| [src/Embe.C2C.Application/Authorizations/MatchingAuthorizationService.cs](/src/Embe.C2C.Application/Authorizations/MatchingAuthorizationService.cs) | C# | 73 | 41 | 10 | 124 |
| [src/Embe.C2C.Application/Authorizations/MessageAuthorizationPolicy.cs](/src/Embe.C2C.Application/Authorizations/MessageAuthorizationPolicy.cs) | C# | -96 | 0 | -12 | -108 |
| [src/Embe.C2C.Application/Authorizations/MessageAuthorizationService.cs](/src/Embe.C2C.Application/Authorizations/MessageAuthorizationService.cs) | C# | 81 | 18 | 9 | 108 |
| [src/Embe.C2C.Application/Authorizations/SearchProfileAuthorizationService.cs](/src/Embe.C2C.Application/Authorizations/SearchProfileAuthorizationService.cs) | C# | 79 | 0 | 15 | 94 |
| [src/Embe.C2C.Application/Authorizations/UserAuthorizationPolicy.cs](/src/Embe.C2C.Application/Authorizations/UserAuthorizationPolicy.cs) | C# | -132 | 0 | -26 | -158 |
| [src/Embe.C2C.Application/Authorizations/UserAuthorizationService.cs](/src/Embe.C2C.Application/Authorizations/UserAuthorizationService.cs) | C# | 108 | 21 | 19 | 148 |
| [src/Embe.C2C.Application/Commands/Judgements/Handlers/JudgeHandler.cs](/src/Embe.C2C.Application/Commands/Judgements/Handlers/JudgeHandler.cs) | C# | 51 | 0 | 1 | 52 |
| [src/Embe.C2C.Application/Commands/Matching/Handlers/UnmatchHandler.cs](/src/Embe.C2C.Application/Commands/Matching/Handlers/UnmatchHandler.cs) | C# | 1 | 0 | 1 | 2 |
| [src/Embe.C2C.Application/Commands/Messages/Handlers/CreateMessageHandler.cs](/src/Embe.C2C.Application/Commands/Messages/Handlers/CreateMessageHandler.cs) | C# | 3 | 0 | 2 | 5 |
| [src/Embe.C2C.Application/Commands/Messages/Handlers/EditMessageHandler.cs](/src/Embe.C2C.Application/Commands/Messages/Handlers/EditMessageHandler.cs) | C# | 9 | 0 | 2 | 11 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/CreateSearchProfileCommand.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/CreateSearchProfileCommand.cs) | C# | 23 | 0 | 3 | 26 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/CreateSearchProfileHandler.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/CreateSearchProfileHandler.cs) | C# | 105 | 0 | 8 | 113 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/UpdateSearchProfileHandler.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/UpdateSearchProfileHandler.cs) | C# | 125 | 0 | 11 | 136 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/UpdateSearchProfileCommand.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/UpdateSearchProfileCommand.cs) | C# | 16 | 0 | 2 | 18 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/GenerateCandidatesHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/GenerateCandidatesHandler.cs) | C# | 33 | 0 | 7 | 40 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/RegisterHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/RegisterHandler.cs) | C# | -47 | 0 | -5 | -52 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/UpdateHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/UpdateHandler.cs) | C# | 6 | 0 | 2 | 8 |
| [src/Embe.C2C.Application/Commands/Users/RegisterCommand.cs](/src/Embe.C2C.Application/Commands/Users/RegisterCommand.cs) | C# | 8 | 0 | 1 | 9 |
| [src/Embe.C2C.Application/Commands/Users/UpdateCommand.cs](/src/Embe.C2C.Application/Commands/Users/UpdateCommand.cs) | C# | 4 | 0 | 1 | 5 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/Judgement.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/Judgement.cs) | C# | 1 | 0 | -1 | 0 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/Matching.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/Matching.cs) | C# | 1 | 0 | 3 | 4 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/Message.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/Message.cs) | C# | 10 | 0 | 3 | 13 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/SearchProfile.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/SearchProfile.cs) | C# | 48 | 0 | 6 | 54 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/User.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/User.cs) | C# | 7 | 0 | 2 | 9 |
| [src/Embe.C2C.Application/Dtos/Read/Entities/ConversationDto.cs](/src/Embe.C2C.Application/Dtos/Read/Entities/ConversationDto.cs) | C# | 1 | 0 | 3 | 4 |
| [src/Embe.C2C.Application/Dtos/Read/Entities/FileDto.cs](/src/Embe.C2C.Application/Dtos/Read/Entities/FileDto.cs) | C# | -27 | 0 | -3 | -30 |
| [src/Embe.C2C.Application/Dtos/Read/Entities/ImageDto.cs](/src/Embe.C2C.Application/Dtos/Read/Entities/ImageDto.cs) | C# | 31 | 0 | 5 | 36 |
| [src/Embe.C2C.Application/Dtos/Read/ValueObjects/DatingPreferences.cs](/src/Embe.C2C.Application/Dtos/Read/ValueObjects/DatingPreferences.cs) | C# | -28 | 0 | -4 | -32 |
| [src/Embe.C2C.Application/Dtos/Read/Variants/Aggregates/SearchProfileVariant.cs](/src/Embe.C2C.Application/Dtos/Read/Variants/Aggregates/SearchProfileVariant.cs) | C# | 91 | 0 | 6 | 97 |
| [src/Embe.C2C.Application/Dtos/Read/Variants/Aggregates/UserVariant.cs](/src/Embe.C2C.Application/Dtos/Read/Variants/Aggregates/UserVariant.cs) | C# | 9 | 0 | 0 | 9 |
| [src/Embe.C2C.Application/Dtos/Write/Aggregates/UserWriteDto.cs](/src/Embe.C2C.Application/Dtos/Write/Aggregates/UserWriteDto.cs) | C# | -1 | 0 | 0 | -1 |
| [src/Embe.C2C.Application/Dtos/Write/ValueObjects/DatingPreferencesWriteDto.cs](/src/Embe.C2C.Application/Dtos/Write/ValueObjects/DatingPreferencesWriteDto.cs) | C# | -10 | 0 | -2 | -12 |
| [src/Embe.C2C.Application/Dtos/Write/ValueObjects/EngagementWriteDto.cs](/src/Embe.C2C.Application/Dtos/Write/ValueObjects/EngagementWriteDto.cs) | C# | 10 | 0 | 2 | 12 |
| [src/Embe.C2C.Application/Enrichment/Aggregates/UserEnriched.cs](/src/Embe.C2C.Application/Enrichment/Aggregates/UserEnriched.cs) | C# | 7 | 0 | 2 | 9 |
| [src/Embe.C2C.Application/Extensions/Domain/Aggregates/JudgementExtensions.cs](/src/Embe.C2C.Application/Extensions/Domain/Aggregates/JudgementExtensions.cs) | C# | 32 | 0 | 2 | 34 |
| [src/Embe.C2C.Application/Extensions/Domain/Aggregates/MatchingExtensions.cs](/src/Embe.C2C.Application/Extensions/Domain/Aggregates/MatchingExtensions.cs) | C# | 63 | 0 | 7 | 70 |
| [src/Embe.C2C.Application/Extensions/Domain/Aggregates/MessageExtensions.cs](/src/Embe.C2C.Application/Extensions/Domain/Aggregates/MessageExtensions.cs) | C# | 28 | 0 | 2 | 30 |
| [src/Embe.C2C.Application/Extensions/Domain/Aggregates/SearchProfileExtensions.cs](/src/Embe.C2C.Application/Extensions/Domain/Aggregates/SearchProfileExtensions.cs) | C# | 22 | 0 | 2 | 24 |
| [src/Embe.C2C.Application/Extensions/Domain/Aggregates/UserExtensions.cs](/src/Embe.C2C.Application/Extensions/Domain/Aggregates/UserExtensions.cs) | C# | 38 | 0 | 5 | 43 |
| [src/Embe.C2C.Application/Extensions/ServiceCollectionExtensions.cs](/src/Embe.C2C.Application/Extensions/ServiceCollectionExtensions.cs) | C# | 33 | 0 | 6 | 39 |
| [src/Embe.C2C.Application/Queries/Geography/GetAdminAreaByIdQuery.cs](/src/Embe.C2C.Application/Queries/Geography/GetAdminAreaByIdQuery.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Queries/Geography/GetAdminAreaQuery.cs](/src/Embe.C2C.Application/Queries/Geography/GetAdminAreaQuery.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Queries/Geography/Handlers/GetAdminAreaByIdHandler.cs](/src/Embe.C2C.Application/Queries/Geography/Handlers/GetAdminAreaByIdHandler.cs) | C# | 24 | 0 | 6 | 30 |
| [src/Embe.C2C.Application/Queries/Geography/Handlers/GetAdminAreaHandler.cs](/src/Embe.C2C.Application/Queries/Geography/Handlers/GetAdminAreaHandler.cs) | C# | 24 | 0 | 4 | 28 |
| [src/Embe.C2C.Application/Queries/Geography/Handlers/GetCountryAdminAreaHandler.cs](/src/Embe.C2C.Application/Queries/Geography/Handlers/GetCountryAdminAreaHandler.cs) | C# | 20 | 0 | 4 | 24 |
| [src/Embe.C2C.Application/Queries/Geography/Handlers/ReverseGeocodeHandler.cs](/src/Embe.C2C.Application/Queries/Geography/Handlers/ReverseGeocodeHandler.cs) | C# | 17 | 0 | 4 | 21 |
| [src/Embe.C2C.Application/Queries/Geography/ReverseGeocodeQuery.cs](/src/Embe.C2C.Application/Queries/Geography/ReverseGeocodeQuery.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Queries/Judgements/Handlers/GetPositiveJudgementsHandler.cs](/src/Embe.C2C.Application/Queries/Judgements/Handlers/GetPositiveJudgementsHandler.cs) | C# | 14 | 0 | -1 | 13 |
| [src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingByIdHandler.cs](/src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingByIdHandler.cs) | C# | 33 | 0 | 0 | 33 |
| [src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingsHandler.cs](/src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingsHandler.cs) | C# | 29 | 0 | 1 | 30 |
| [src/Embe.C2C.Application/Queries/Messages/Handlers/GetMessageByIdHandler.cs](/src/Embe.C2C.Application/Queries/Messages/Handlers/GetMessageByIdHandler.cs) | C# | 3 | 0 | 0 | 3 |
| [src/Embe.C2C.Application/Queries/Messages/Handlers/GetMessagesByMatchingIdHandler.cs](/src/Embe.C2C.Application/Queries/Messages/Handlers/GetMessagesByMatchingIdHandler.cs) | C# | 4 | 0 | 0 | 4 |
| [src/Embe.C2C.Application/Queries/SearchProfiles/GetAllSearchProfilesQuery.cs](/src/Embe.C2C.Application/Queries/SearchProfiles/GetAllSearchProfilesQuery.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Queries/SearchProfiles/GetSearchProfileQuery.cs](/src/Embe.C2C.Application/Queries/SearchProfiles/GetSearchProfileQuery.cs) | C# | 5 | 0 | 1 | 6 |
| [src/Embe.C2C.Application/Queries/SearchProfiles/Handlers/GetAllSearchProfilesQuery.cs](/src/Embe.C2C.Application/Queries/SearchProfiles/Handlers/GetAllSearchProfilesQuery.cs) | C# | 51 | 0 | 6 | 57 |
| [src/Embe.C2C.Application/Queries/SearchProfiles/Handlers/GetSearchProfileHandler.cs](/src/Embe.C2C.Application/Queries/SearchProfiles/Handlers/GetSearchProfileHandler.cs) | C# | 64 | 0 | 7 | 71 |
| [src/Embe.C2C.Application/Queries/Users/GetHasSearchProfileQuery.cs](/src/Embe.C2C.Application/Queries/Users/GetHasSearchProfileQuery.cs) | C# | 6 | 0 | 1 | 7 |
| [src/Embe.C2C.Application/Queries/Users/GetMeQuery.cs](/src/Embe.C2C.Application/Queries/Users/GetMeQuery.cs) | C# | 6 | 0 | 1 | 7 |
| [src/Embe.C2C.Application/Queries/Users/Handlers/GetHasSearchProfileHandler.cs](/src/Embe.C2C.Application/Queries/Users/Handlers/GetHasSearchProfileHandler.cs) | C# | 25 | 0 | 5 | 30 |
| [src/Embe.C2C.Application/Queries/Users/Handlers/GetMeHandler.cs](/src/Embe.C2C.Application/Queries/Users/Handlers/GetMeHandler.cs) | C# | 37 | 0 | 4 | 41 |
| [src/Embe.C2C.Application/Queries/Users/Handlers/GetUserByIdHandler.cs](/src/Embe.C2C.Application/Queries/Users/Handlers/GetUserByIdHandler.cs) | C# | 5 | 0 | -1 | 4 |
| [src/Embe.C2C.Domain/Aggregates/Aggregate.cs](/src/Embe.C2C.Domain/Aggregates/Aggregate.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Domain/Aggregates/Candidates/Candidate.cs](/src/Embe.C2C.Domain/Aggregates/Candidates/Candidate.cs) | C# | 47 | 0 | 8 | 55 |
| [src/Embe.C2C.Domain/Aggregates/Judgements/Judgement.cs](/src/Embe.C2C.Domain/Aggregates/Judgements/Judgement.cs) | C# | -6 | 0 | -1 | -7 |
| [src/Embe.C2C.Domain/Aggregates/Matchings/Matching.cs](/src/Embe.C2C.Domain/Aggregates/Matchings/Matching.cs) | C# | 15 | 0 | 0 | 15 |
| [src/Embe.C2C.Domain/Aggregates/SearchProfiles/SearchProfile.cs](/src/Embe.C2C.Domain/Aggregates/SearchProfiles/SearchProfile.cs) | C# | 203 | 0 | 33 | 236 |
| [src/Embe.C2C.Domain/Aggregates/Users/Events/UserFileRemovedEvent.cs](/src/Embe.C2C.Domain/Aggregates/Users/Events/UserFileRemovedEvent.cs) | C# | -2 | 0 | -1 | -3 |
| [src/Embe.C2C.Domain/Aggregates/Users/Events/UserImageRemovedEvent.cs](/src/Embe.C2C.Domain/Aggregates/Users/Events/UserImageRemovedEvent.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Domain/Aggregates/Users/User.cs](/src/Embe.C2C.Domain/Aggregates/Users/User.cs) | C# | 3 | 2 | 1 | 6 |
| [src/Embe.C2C.Domain/Embe.C2C.Domain.csproj](/src/Embe.C2C.Domain/Embe.C2C.Domain.csproj) | XML | 3 | 0 | 1 | 4 |
| [src/Embe.C2C.Domain/Entities/Entity.cs](/src/Embe.C2C.Domain/Entities/Entity.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Domain/Entities/SearchProfiles/SearchProfileGender.cs](/src/Embe.C2C.Domain/Entities/SearchProfiles/SearchProfileGender.cs) | C# | 25 | 0 | 6 | 31 |
| [src/Embe.C2C.Domain/Services/JudgementService.cs](/src/Embe.C2C.Domain/Services/JudgementService.cs) | C# | 7 | 0 | 1 | 8 |
| [src/Embe.C2C.Domain/ValueObjects/DatingPreferences.cs](/src/Embe.C2C.Domain/ValueObjects/DatingPreferences.cs) | C# | -40 | 0 | -7 | -47 |
| [src/Embe.C2C.Domain/ValueObjects/Engagements/Engagement.cs](/src/Embe.C2C.Domain/ValueObjects/Engagements/Engagement.cs) | C# | 54 | 0 | 9 | 63 |
| [src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/EngagementBoundedness.cs](/src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/EngagementBoundedness.cs) | C# | 7 | 0 | 1 | 8 |
| [src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/EngagementFrequency.cs](/src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/EngagementFrequency.cs) | C# | 10 | 0 | 1 | 11 |
| [src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/EngagementMedium.cs](/src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/EngagementMedium.cs) | C# | 7 | 0 | 2 | 9 |
| [src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/FixedTermEngagementDomainError.cs](/src/Embe.C2C.Domain/ValueObjects/Engagements/Enums/FixedTermEngagementDomainError.cs) | C# | 8 | 0 | 1 | 9 |
| [src/Embe.C2C.Domain/ValueObjects/Location.cs](/src/Embe.C2C.Domain/ValueObjects/Location.cs) | C# | 22 | 0 | 8 | 30 |
| [src/Embe.C2C.Domain/ValueObjects/RelationshipType.cs](/src/Embe.C2C.Domain/ValueObjects/RelationshipType.cs) | C# | 7 | 0 | 1 | 8 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/AdminAreaConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/AdminAreaConfiguration.cs) | C# | 21 | 0 | 3 | 24 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/CandidateConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/CandidateConfiguration.cs) | C# | 29 | 0 | 6 | 35 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/CandidateEntityConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/CandidateEntityConfiguration.cs) | C# | -20 | 0 | -4 | -24 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/JudgementConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/JudgementConfiguration.cs) | C# | -4 | 0 | -1 | -5 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/MatchingConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/MatchingConfiguration.cs) | C# | 7 | 0 | 2 | 9 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/SearchProfileConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/SearchProfileConfiguration.cs) | C# | 45 | 0 | 10 | 55 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/UserConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/UserConfiguration.cs) | C# | -31 | 0 | -4 | -35 |
| [src/Embe.C2C.Infrastructure/Ef/Contexts/C2CContext.cs](/src/Embe.C2C.Infrastructure/Ef/Contexts/C2CContext.cs) | C# | 96 | -10 | 2 | 88 |
| [src/Embe.C2C.Infrastructure/Ef/Entities/AdminArea.cs](/src/Embe.C2C.Infrastructure/Ef/Entities/AdminArea.cs) | C# | 24 | 0 | 10 | 34 |
| [src/Embe.C2C.Infrastructure/Ef/Entities/Candidate.cs](/src/Embe.C2C.Infrastructure/Ef/Entities/Candidate.cs) | C# | -8 | 0 | -3 | -11 |
| [src/Embe.C2C.Infrastructure/Migrations/20260609222237\_InitialCreate.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260609222237_InitialCreate.Designer.cs) | C# | -603 | -2 | -206 | -811 |
| [src/Embe.C2C.Infrastructure/Migrations/20260609222237\_InitialCreate.cs](/src/Embe.C2C.Infrastructure/Migrations/20260609222237_InitialCreate.cs) | C# | -507 | -3 | -56 | -566 |
| [src/Embe.C2C.Infrastructure/Migrations/20260610225115\_GenderFix.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260610225115_GenderFix.Designer.cs) | C# | -604 | -2 | -206 | -812 |
| [src/Embe.C2C.Infrastructure/Migrations/20260610225115\_GenderFix.cs](/src/Embe.C2C.Infrastructure/Migrations/20260610225115_GenderFix.cs) | C# | -28 | -3 | -4 | -35 |
| [src/Embe.C2C.Infrastructure/Migrations/20260610235923\_MatchingUserNavigationProperties.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260610235923_MatchingUserNavigationProperties.Designer.cs) | C# | -611 | -2 | -209 | -822 |
| [src/Embe.C2C.Infrastructure/Migrations/20260610235923\_MatchingUserNavigationProperties.cs](/src/Embe.C2C.Infrastructure/Migrations/20260610235923_MatchingUserNavigationProperties.cs) | C# | -14 | -3 | -6 | -23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611095955\_RenameFileDetailsUrlToName.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611095955_RenameFileDetailsUrlToName.Designer.cs) | C# | -611 | -2 | -209 | -822 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611095955\_RenameFileDetailsUrlToName.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611095955_RenameFileDetailsUrlToName.cs) | C# | -22 | -3 | -4 | -29 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611100118\_SnapshotIsolation.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611100118_SnapshotIsolation.Designer.cs) | C# | -611 | -2 | -209 | -822 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611100118\_SnapshotIsolation.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611100118_SnapshotIsolation.cs) | C# | -30 | -3 | -7 | -40 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611105836\_PrivateConstructors.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611105836_PrivateConstructors.Designer.cs) | C# | -611 | -2 | -209 | -822 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611105836\_PrivateConstructors.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611105836_PrivateConstructors.cs) | C# | -14 | -3 | -6 | -23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611105943\_MatchingCreatedAt.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611105943_MatchingCreatedAt.Designer.cs) | C# | -613 | -2 | -210 | -825 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611105943\_MatchingCreatedAt.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611105943_MatchingCreatedAt.cs) | C# | -24 | -3 | -4 | -31 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611110701\_CreatedAtPrivateSettor.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611110701_CreatedAtPrivateSettor.Designer.cs) | C# | -619 | -2 | -213 | -834 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611110701\_CreatedAtPrivateSettor.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611110701_CreatedAtPrivateSettor.cs) | C# | -42 | -3 | -8 | -53 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611114349\_Message.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611114349_Message.Designer.cs) | C# | -676 | -2 | -234 | -912 |
| [src/Embe.C2C.Infrastructure/Migrations/20260611114349\_Message.cs](/src/Embe.C2C.Infrastructure/Migrations/20260611114349_Message.cs) | C# | -82 | -3 | -11 | -96 |
| [src/Embe.C2C.Infrastructure/Migrations/20260612142730\_ConversationMessagesNavigation.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260612142730_ConversationMessagesNavigation.Designer.cs) | C# | -680 | -2 | -235 | -917 |
| [src/Embe.C2C.Infrastructure/Migrations/20260612142730\_ConversationMessagesNavigation.cs](/src/Embe.C2C.Infrastructure/Migrations/20260612142730_ConversationMessagesNavigation.cs) | C# | -14 | -3 | -6 | -23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260615105845\_Navigations.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260615105845_Navigations.Designer.cs) | C# | -723 | -2 | -252 | -977 |
| [src/Embe.C2C.Infrastructure/Migrations/20260615105845\_Navigations.cs](/src/Embe.C2C.Infrastructure/Migrations/20260615105845_Navigations.cs) | C# | -168 | -3 | -34 | -205 |
| [src/Embe.C2C.Infrastructure/Migrations/20260615155701\_BlockingsAndMessages.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260615155701_BlockingsAndMessages.Designer.cs) | C# | -723 | -2 | -252 | -977 |
| [src/Embe.C2C.Infrastructure/Migrations/20260615155701\_BlockingsAndMessages.cs](/src/Embe.C2C.Infrastructure/Migrations/20260615155701_BlockingsAndMessages.cs) | C# | -88 | -3 | -20 | -111 |
| [src/Embe.C2C.Infrastructure/Migrations/20260616024241\_JudgementNavigations.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260616024241_JudgementNavigations.Designer.cs) | C# | -725 | -2 | -254 | -981 |
| [src/Embe.C2C.Infrastructure/Migrations/20260616024241\_JudgementNavigations.cs](/src/Embe.C2C.Infrastructure/Migrations/20260616024241_JudgementNavigations.cs) | C# | -14 | -3 | -6 | -23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260618131631\_Candidates.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260618131631_Candidates.Designer.cs) | C# | -748 | -2 | -261 | -1,011 |
| [src/Embe.C2C.Infrastructure/Migrations/20260618131631\_Candidates.cs](/src/Embe.C2C.Infrastructure/Migrations/20260618131631_Candidates.cs) | C# | -43 | -3 | -5 | -51 |
| [src/Embe.C2C.Infrastructure/Migrations/20260618133455\_CandidatesNavigations.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260618133455_CandidatesNavigations.Designer.cs) | C# | -749 | -2 | -262 | -1,013 |
| [src/Embe.C2C.Infrastructure/Migrations/20260618133455\_CandidatesNavigations.cs](/src/Embe.C2C.Infrastructure/Migrations/20260618133455_CandidatesNavigations.cs) | C# | -14 | -3 | -6 | -23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260618164502\_MessageNavigations.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260618164502_MessageNavigations.Designer.cs) | C# | -750 | -2 | -263 | -1,015 |
| [src/Embe.C2C.Infrastructure/Migrations/20260618164502\_MessageNavigations.cs](/src/Embe.C2C.Infrastructure/Migrations/20260618164502_MessageNavigations.cs) | C# | -14 | -3 | -6 | -23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260709230440\_InitialCreate.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260709230440_InitialCreate.Designer.cs) | C# | 844 | 2 | 303 | 1,149 |
| [src/Embe.C2C.Infrastructure/Migrations/20260709230440\_InitialCreate.cs](/src/Embe.C2C.Infrastructure/Migrations/20260709230440_InitialCreate.cs) | C# | 752 | 3 | 87 | 842 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710023305\_DomainCandidate.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710023305_DomainCandidate.Designer.cs) | C# | 915 | 2 | 331 | 1,248 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710023305\_DomainCandidate.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710023305_DomainCandidate.cs) | C# | 256 | 3 | 55 | 314 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710023646\_SearchProfilesFix.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710023646_SearchProfilesFix.Designer.cs) | C# | 892 | 2 | 324 | 1,218 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710023646\_SearchProfilesFix.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710023646_SearchProfilesFix.cs) | C# | 44 | 3 | 5 | 52 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710095944\_SearchProfileRelationshipType.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710095944_SearchProfileRelationshipType.Designer.cs) | C# | 894 | 2 | 325 | 1,221 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710095944\_SearchProfileRelationshipType.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710095944_SearchProfileRelationshipType.cs) | C# | 23 | 3 | 4 | 30 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710230332\_SearchProfileActive.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710230332_SearchProfileActive.Designer.cs) | C# | 900 | 2 | 328 | 1,230 |
| [src/Embe.C2C.Infrastructure/Migrations/20260710230332\_SearchProfileActive.cs](/src/Embe.C2C.Infrastructure/Migrations/20260710230332_SearchProfileActive.cs) | C# | 42 | 3 | 8 | 53 |
| [src/Embe.C2C.Infrastructure/Migrations/20260711014915\_SearchProfileGenderKeyNeverGenerated.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260711014915_SearchProfileGenderKeyNeverGenerated.Designer.cs) | C# | 899 | 2 | 328 | 1,229 |
| [src/Embe.C2C.Infrastructure/Migrations/20260711014915\_SearchProfileGenderKeyNeverGenerated.cs](/src/Embe.C2C.Infrastructure/Migrations/20260711014915_SearchProfileGenderKeyNeverGenerated.cs) | C# | 14 | 3 | 6 | 23 |
| [src/Embe.C2C.Infrastructure/Migrations/20260711030456\_MatchingSetNullOnSearchProfileDeleted.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260711030456_MatchingSetNullOnSearchProfileDeleted.Designer.cs) | C# | 899 | 2 | 328 | 1,229 |
| [src/Embe.C2C.Infrastructure/Migrations/20260711030456\_MatchingSetNullOnSearchProfileDeleted.cs](/src/Embe.C2C.Infrastructure/Migrations/20260711030456_MatchingSetNullOnSearchProfileDeleted.cs) | C# | 54 | 3 | 10 | 67 |
| [src/Embe.C2C.Infrastructure/Migrations/C2CContextModelSnapshot.cs](/src/Embe.C2C.Infrastructure/Migrations/C2CContextModelSnapshot.cs) | C# | 149 | 0 | 65 | 214 |
| [src/Embe.C2C.Test/Embe.C2C.Test.csproj](/src/Embe.C2C.Test/Embe.C2C.Test.csproj) | XML | 6 | 0 | 0 | 6 |
| [src/embe.c2c.frontend/next.config.ts](/src/embe.c2c.frontend/next.config.ts) | TypeScript | 5 | 0 | 0 | 5 |
| [src/embe.c2c.frontend/package-lock.json](/src/embe.c2c.frontend/package-lock.json) | JSON | 479 | 0 | 0 | 479 |
| [src/embe.c2c.frontend/package.json](/src/embe.c2c.frontend/package.json) | JSON | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/discover/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/discover/page.tsx) | TypeScript JSX | 13 | 0 | 2 | 15 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/matches/\[matchId\]/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/matches/%5BmatchId%5D/page.tsx) | TypeScript JSX | -7 | 0 | 0 | -7 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/me/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/me/page.tsx) | TypeScript JSX | 16 | 0 | 3 | 19 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/profile/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/profile/page.tsx) | TypeScript JSX | -11 | 0 | -2 | -13 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/\[searchProfileId\]/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/%5BsearchProfileId%5D/page.tsx) | TypeScript JSX | 34 | 0 | 1 | 35 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/page.tsx) | TypeScript JSX | 13 | 0 | 2 | 15 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search/page.tsx) | TypeScript JSX | 26 | 0 | 5 | 31 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/swipe/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/swipe/page.tsx) | TypeScript JSX | -18 | 0 | -2 | -20 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/user/\[userId\]/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/user/%5BuserId%5D/page.tsx) | TypeScript JSX | 23 | 0 | 4 | 27 |
| [src/embe.c2c.frontend/src/app/(pages)/public/(auth)/register/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/public/(auth)/register/page.tsx) | TypeScript JSX | 0 | 0 | -1 | -1 |
| [src/embe.c2c.frontend/src/app/globals.css](/src/embe.c2c.frontend/src/app/globals.css) | PostCSS | 31 | -1 | 9 | 39 |
| [src/embe.c2c.frontend/src/features/auth/actions/action.ts](/src/embe.c2c.frontend/src/features/auth/actions/action.ts) | TypeScript | 15 | 0 | 4 | 19 |
| [src/embe.c2c.frontend/src/features/auth/actions/register/types.ts](/src/embe.c2c.frontend/src/features/auth/actions/register/types.ts) | TypeScript | -4 | 0 | 0 | -4 |
| [src/embe.c2c.frontend/src/features/auth/actions/sign-in/actions.ts](/src/embe.c2c.frontend/src/features/auth/actions/sign-in/actions.ts) | TypeScript | 0 | 0 | -1 | -1 |
| [src/embe.c2c.frontend/src/features/auth/components/BasicProfileForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/BasicProfileForm.tsx) | TypeScript JSX | 90 | 0 | 11 | 101 |
| [src/embe.c2c.frontend/src/features/auth/components/DatingPreferencesForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/DatingPreferencesForm.tsx) | TypeScript JSX | -61 | 0 | -7 | -68 |
| [src/embe.c2c.frontend/src/features/auth/components/LoginForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/LoginForm.tsx) | TypeScript JSX | 3 | 0 | 5 | 8 |
| [src/embe.c2c.frontend/src/features/auth/components/ProfileForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/ProfileForm.tsx) | TypeScript JSX | -56 | 0 | -6 | -62 |
| [src/embe.c2c.frontend/src/features/auth/components/RegisterForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/RegisterForm.tsx) | TypeScript JSX | -41 | 0 | -7 | -48 |
| [src/embe.c2c.frontend/src/features/auth/components/SearchProfileBuilderForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/SearchProfileBuilderForm.tsx) | TypeScript JSX | 100 | 0 | 13 | 113 |
| [src/embe.c2c.frontend/src/features/matches/actions/action.ts](/src/embe.c2c.frontend/src/features/matches/actions/action.ts) | TypeScript | 8 | 0 | 1 | 9 |
| [src/embe.c2c.frontend/src/features/matches/components/ConversationCompact.tsx](/src/embe.c2c.frontend/src/features/matches/components/ConversationCompact.tsx) | TypeScript JSX | 0 | 0 | 5 | 5 |
| [src/embe.c2c.frontend/src/features/matches/components/Match.tsx](/src/embe.c2c.frontend/src/features/matches/components/Match.tsx) | TypeScript JSX | 56 | 0 | 5 | 61 |
| [src/embe.c2c.frontend/src/features/matches/components/MessageBrief.tsx](/src/embe.c2c.frontend/src/features/matches/components/MessageBrief.tsx) | TypeScript JSX | 0 | 0 | 4 | 4 |
| [src/embe.c2c.frontend/src/features/matches/components/UserCompact.tsx](/src/embe.c2c.frontend/src/features/matches/components/UserCompact.tsx) | TypeScript JSX | 5 | 0 | 0 | 5 |
| [src/embe.c2c.frontend/src/features/me/actions/action.ts](/src/embe.c2c.frontend/src/features/me/actions/action.ts) | TypeScript | 29 | 0 | 4 | 33 |
| [src/embe.c2c.frontend/src/features/me/actions/type.ts](/src/embe.c2c.frontend/src/features/me/actions/type.ts) | TypeScript | 0 | 0 | 1 | 1 |
| [src/embe.c2c.frontend/src/features/me/components/Me.tsx](/src/embe.c2c.frontend/src/features/me/components/Me.tsx) | TypeScript JSX | 167 | 0 | 21 | 188 |
| [src/embe.c2c.frontend/src/features/me/components/MyInfoForm.tsx](/src/embe.c2c.frontend/src/features/me/components/MyInfoForm.tsx) | TypeScript JSX | 130 | 0 | 19 | 149 |
| [src/embe.c2c.frontend/src/features/search-profiles/actions.ts](/src/embe.c2c.frontend/src/features/search-profiles/actions.ts) | TypeScript | 46 | 0 | 3 | 49 |
| [src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileBuilder.tsx](/src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileBuilder.tsx) | TypeScript JSX | -8 | 0 | -3 | -11 |
| [src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileEditor.tsx](/src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileEditor.tsx) | TypeScript JSX | -8 | 0 | -1 | -9 |
| [src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileForm.tsx](/src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileForm.tsx) | TypeScript JSX | 323 | 0 | 49 | 372 |
| [src/embe.c2c.frontend/src/features/search-profiles/types.ts](/src/embe.c2c.frontend/src/features/search-profiles/types.ts) | TypeScript | 19 | 0 | 0 | 19 |
| [src/embe.c2c.frontend/src/features/search/actions/action.ts](/src/embe.c2c.frontend/src/features/search/actions/action.ts) | TypeScript | 42 | 0 | 9 | 51 |
| [src/embe.c2c.frontend/src/features/search/actions/type.ts](/src/embe.c2c.frontend/src/features/search/actions/type.ts) | TypeScript | 0 | 0 | 1 | 1 |
| [src/embe.c2c.frontend/src/features/search/components/FindUserDating.tsx](/src/embe.c2c.frontend/src/features/search/components/FindUserDating.tsx) | TypeScript JSX | 25 | 0 | 4 | 29 |
| [src/embe.c2c.frontend/src/features/search/components/JudgeOverlay.tsx](/src/embe.c2c.frontend/src/features/search/components/JudgeOverlay.tsx) | TypeScript JSX | 56 | 0 | 6 | 62 |
| [src/embe.c2c.frontend/src/features/search/components/Search.tsx](/src/embe.c2c.frontend/src/features/search/components/Search.tsx) | TypeScript JSX | 94 | 0 | 13 | 107 |
| [src/embe.c2c.frontend/src/features/search/components/SearchProfileCompact.tsx](/src/embe.c2c.frontend/src/features/search/components/SearchProfileCompact.tsx) | TypeScript JSX | 107 | 0 | 10 | 117 |
| [src/embe.c2c.frontend/src/features/search/components/SearchProfiles.tsx](/src/embe.c2c.frontend/src/features/search/components/SearchProfiles.tsx) | TypeScript JSX | 43 | 0 | 7 | 50 |
| [src/embe.c2c.frontend/src/features/swipe/actions/action.ts](/src/embe.c2c.frontend/src/features/swipe/actions/action.ts) | TypeScript | -32 | 0 | -6 | -38 |
| [src/embe.c2c.frontend/src/features/swipe/actions/type.ts](/src/embe.c2c.frontend/src/features/swipe/actions/type.ts) | TypeScript | 0 | 0 | -1 | -1 |
| [src/embe.c2c.frontend/src/features/swipe/components/JudgeOverlay.tsx](/src/embe.c2c.frontend/src/features/swipe/components/JudgeOverlay.tsx) | TypeScript JSX | -56 | 0 | -6 | -62 |
| [src/embe.c2c.frontend/src/features/swipe/components/Swipe.tsx](/src/embe.c2c.frontend/src/features/swipe/components/Swipe.tsx) | TypeScript JSX | -51 | 0 | -9 | -60 |
| [src/embe.c2c.frontend/src/features/swipe/components/SwipeUserDating.tsx](/src/embe.c2c.frontend/src/features/swipe/components/SwipeUserDating.tsx) | TypeScript JSX | -25 | 0 | -4 | -29 |
| [src/embe.c2c.frontend/src/features/user/actions/action.ts](/src/embe.c2c.frontend/src/features/user/actions/action.ts) | TypeScript | 0 | 0 | -1 | -1 |
| [src/embe.c2c.frontend/src/features/user/actions/type.ts](/src/embe.c2c.frontend/src/features/user/actions/type.ts) | TypeScript | 0 | 0 | -1 | -1 |
| [src/embe.c2c.frontend/src/features/user/components/menu/UserMenu.tsx](/src/embe.c2c.frontend/src/features/user/components/menu/UserMenu.tsx) | TypeScript JSX | -29 | 0 | -8 | -37 |
| [src/embe.c2c.frontend/src/shared/actions/geography/actions.ts](/src/embe.c2c.frontend/src/shared/actions/geography/actions.ts) | TypeScript | 50 | 0 | 8 | 58 |
| [src/embe.c2c.frontend/src/shared/actions/geography/types.ts](/src/embe.c2c.frontend/src/shared/actions/geography/types.ts) | TypeScript | 10 | 0 | 0 | 10 |
| [src/embe.c2c.frontend/src/shared/actions/user/action.ts](/src/embe.c2c.frontend/src/shared/actions/user/action.ts) | TypeScript | 32 | 0 | 2 | 34 |
| [src/embe.c2c.frontend/src/shared/actions/user/type.ts](/src/embe.c2c.frontend/src/shared/actions/user/type.ts) | TypeScript | 0 | 0 | 1 | 1 |
| [src/embe.c2c.frontend/src/shared/cache.ts](/src/embe.c2c.frontend/src/shared/cache.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/shared/components/buttons/Button.tsx](/src/embe.c2c.frontend/src/shared/components/buttons/Button.tsx) | TypeScript JSX | 23 | 0 | 4 | 27 |
| [src/embe.c2c.frontend/src/shared/components/image-gallery/ImageGallery.tsx](/src/embe.c2c.frontend/src/shared/components/image-gallery/ImageGallery.tsx) | TypeScript JSX | -30 | 0 | -5 | -35 |
| [src/embe.c2c.frontend/src/shared/components/images/ImageGallery.module.css](/src/embe.c2c.frontend/src/shared/components/images/ImageGallery.module.css) | PostCSS | 5 | 0 | 0 | 5 |
| [src/embe.c2c.frontend/src/shared/components/images/ImageGallery.tsx](/src/embe.c2c.frontend/src/shared/components/images/ImageGallery.tsx) | TypeScript JSX | 73 | 0 | 9 | 82 |
| [src/embe.c2c.frontend/src/shared/components/infinite-scroll/InfiniteScroll.tsx](/src/embe.c2c.frontend/src/shared/components/infinite-scroll/InfiniteScroll.tsx) | TypeScript JSX | -107 | 0 | -33 | -140 |
| [src/embe.c2c.frontend/src/shared/components/infos/InfoWindow.tsx](/src/embe.c2c.frontend/src/shared/components/infos/InfoWindow.tsx) | TypeScript JSX | 20 | 0 | 6 | 26 |
| [src/embe.c2c.frontend/src/shared/components/inputs/checkbox-input/CheckBoxInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/checkbox-input/CheckBoxInput.tsx) | TypeScript JSX | 9 | 0 | 3 | 12 |
| [src/embe.c2c.frontend/src/shared/components/inputs/date-input/DateInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/date-input/DateInput.tsx) | TypeScript JSX | 4 | 0 | 1 | 5 |
| [src/embe.c2c.frontend/src/shared/components/inputs/dropdown-input/DropDownInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/dropdown-input/DropDownInput.tsx) | TypeScript JSX | 38 | 0 | 1 | 39 |
| [src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGallery.module.css](/src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGallery.module.css) | PostCSS | 0 | 0 | -1 | -1 |
| [src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGallery.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGallery.tsx) | TypeScript JSX | -102 | 0 | -16 | -118 |
| [src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGalleryInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGalleryInput.tsx) | TypeScript JSX | 112 | 0 | 16 | 128 |
| [src/embe.c2c.frontend/src/shared/components/inputs/location-input/LocationInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/location-input/LocationInput.tsx) | TypeScript JSX | 354 | 7 | 71 | 432 |
| [src/embe.c2c.frontend/src/shared/components/inputs/select-input/SelectInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/select-input/SelectInput.tsx) | TypeScript JSX | 11 | 0 | 1 | 12 |
| [src/embe.c2c.frontend/src/shared/components/inputs/text-area-input/TextAreaInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/text-area-input/TextAreaInput.tsx) | TypeScript JSX | 29 | 0 | 4 | 33 |
| [src/embe.c2c.frontend/src/shared/components/inputs/text-input/TextInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/text-input/TextInput.tsx) | TypeScript JSX | 3 | 0 | 2 | 5 |
| [src/embe.c2c.frontend/src/shared/components/modal/Modal.module.css](/src/embe.c2c.frontend/src/shared/components/modal/Modal.module.css) | PostCSS | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/shared/components/modal/Modal.tsx](/src/embe.c2c.frontend/src/shared/components/modal/Modal.tsx) | TypeScript JSX | -4 | 0 | 0 | -4 |
| [src/embe.c2c.frontend/src/shared/components/nav/MainNav.tsx](/src/embe.c2c.frontend/src/shared/components/nav/MainNav.tsx) | TypeScript JSX | 6 | 0 | 0 | 6 |
| [src/embe.c2c.frontend/src/shared/components/profile-builder/ProfileBuilder.tsx](/src/embe.c2c.frontend/src/shared/components/profile-builder/ProfileBuilder.tsx) | TypeScript JSX | -9 | 0 | -7 | -16 |
| [src/embe.c2c.frontend/src/shared/components/scroll/infinite-scroll/InfiniteScroll.tsx](/src/embe.c2c.frontend/src/shared/components/scroll/infinite-scroll/InfiniteScroll.tsx) | TypeScript JSX | 111 | 0 | 34 | 145 |
| [src/embe.c2c.frontend/src/shared/components/surfaces/Surface.tsx](/src/embe.c2c.frontend/src/shared/components/surfaces/Surface.tsx) | TypeScript JSX | 1 | 0 | 1 | 2 |
| [src/embe.c2c.frontend/src/shared/components/user/Profile.tsx](/src/embe.c2c.frontend/src/shared/components/user/Profile.tsx) | TypeScript JSX | 74 | 0 | 3 | 77 |
| [src/embe.c2c.frontend/src/shared/distance.ts](/src/embe.c2c.frontend/src/shared/distance.ts) | TypeScript | 6 | 0 | 0 | 6 |
| [src/embe.c2c.frontend/src/shared/enums.ts](/src/embe.c2c.frontend/src/shared/enums.ts) | TypeScript | 61 | 0 | 6 | 67 |
| [src/embe.c2c.frontend/src/shared/stores/current-user.ts](/src/embe.c2c.frontend/src/shared/stores/current-user.ts) | TypeScript | 12 | 0 | 2 | 14 |
| [src/embe.c2c.frontend/src/shared/time.ts](/src/embe.c2c.frontend/src/shared/time.ts) | TypeScript | 20 | 0 | 3 | 23 |
| [src/embe.c2c.frontend/src/shared/types/domain/aggregates.ts](/src/embe.c2c.frontend/src/shared/types/domain/aggregates.ts) | TypeScript | 20 | 0 | 2 | 22 |
| [src/embe.c2c.frontend/src/shared/types/domain/value-objects.ts](/src/embe.c2c.frontend/src/shared/types/domain/value-objects.ts) | TypeScript | 31 | 0 | 4 | 35 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details