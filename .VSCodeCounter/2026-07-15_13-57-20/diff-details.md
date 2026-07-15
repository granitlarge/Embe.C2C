# Diff Details

Date : 2026-07-15 13:57:20

Directory c:\\Users\\Miro\\source\\repos\\Embe.C2C

Total : 191 files,  10731 codes, 44 comments, 3018 blanks, all 13793 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [.github/workflows/main\_relationshipfinder-api.yml](/.github/workflows/main_relationshipfinder-api.yml) | YAML | 45 | 2 | 12 | 59 |
| [.github/workflows/main\_relationshipfinder.yml](/.github/workflows/main_relationshipfinder.yml) | YAML | 33 | 2 | 9 | 44 |
| [data/ADM.json](/data/ADM.json) | JSON | 1 | 0 | 0 | 1 |
| [data/scripts/LoadADMFileToDb.sql](/data/scripts/LoadADMFileToDb.sql) | MS SQL | 0 | 0 | 1 | 1 |
| [settings.json](/settings.json) | JSON with Comments | 12 | 0 | 1 | 13 |
| [src/Embe.C2C.Api/Embe.C2C.Api.csproj](/src/Embe.C2C.Api/Embe.C2C.Api.csproj) | XML | 1 | 0 | 0 | 1 |
| [src/Embe.C2C.Api/EndPoints/SearchProfileEndPoints.cs](/src/Embe.C2C.Api/EndPoints/SearchProfileEndPoints.cs) | C# | 11 | 0 | 1 | 12 |
| [src/Embe.C2C.Api/EndPoints/SignalREndPoints.cs](/src/Embe.C2C.Api/EndPoints/SignalREndPoints.cs) | C# | 26 | 0 | 3 | 29 |
| [src/Embe.C2C.Api/EndPoints/UserEndPoints.cs](/src/Embe.C2C.Api/EndPoints/UserEndPoints.cs) | C# | 6 | 0 | 1 | 7 |
| [src/Embe.C2C.Api/OpenApi/OpenApiConfiguration.cs](/src/Embe.C2C.Api/OpenApi/OpenApiConfiguration.cs) | C# | 12 | 0 | 2 | 14 |
| [src/Embe.C2C.Api/OpenApi/OpenApiDocumentTransformer.cs](/src/Embe.C2C.Api/OpenApi/OpenApiDocumentTransformer.cs) | C# | 15 | 0 | 6 | 21 |
| [src/Embe.C2C.Api/OpenApi/OpenApiEndpointExtensions.cs](/src/Embe.C2C.Api/OpenApi/OpenApiEndpointExtensions.cs) | C# | 11 | 0 | 1 | 12 |
| [src/Embe.C2C.Api/Program.cs](/src/Embe.C2C.Api/Program.cs) | C# | 24 | 0 | 3 | 27 |
| [src/Embe.C2C.Api/appsettings.Development.json](/src/Embe.C2C.Api/appsettings.Development.json) | JSON | 8 | 0 | 0 | 8 |
| [src/Embe.C2C.Application/Abstractions/Services/ContentSafetyService.cs](/src/Embe.C2C.Application/Abstractions/Services/ContentSafetyService.cs) | C# | 5 | 6 | 1 | 12 |
| [src/Embe.C2C.Application/Abstractions/Services/FileService.cs](/src/Embe.C2C.Application/Abstractions/Services/FileService.cs) | C# | 22 | 0 | 4 | 26 |
| [src/Embe.C2C.Application/Abstractions/Services/WorkItemServices/WorkItems/MoveImage.cs](/src/Embe.C2C.Application/Abstractions/Services/WorkItemServices/WorkItems/MoveImage.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Authorizations/FactGenerators/AuthorizationFactGenerator.cs](/src/Embe.C2C.Application/Authorizations/FactGenerators/AuthorizationFactGenerator.cs) | C# | 0 | 0 | -1 | -1 |
| [src/Embe.C2C.Application/Authorizations/FactStores/Users/Facts/CandidateUserFact.cs](/src/Embe.C2C.Application/Authorizations/FactStores/Users/Facts/CandidateUserFact.cs) | C# | -1 | 0 | -1 | -2 |
| [src/Embe.C2C.Application/Authorizations/SearchProfileAuthorizationService.cs](/src/Embe.C2C.Application/Authorizations/SearchProfileAuthorizationService.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Commands/Auth/Handlers/SignInHandler.cs](/src/Embe.C2C.Application/Commands/Auth/Handlers/SignInHandler.cs) | C# | -1 | 0 | 0 | -1 |
| [src/Embe.C2C.Application/Commands/CommandHandler.cs](/src/Embe.C2C.Application/Commands/CommandHandler.cs) | C# | 48 | 0 | 8 | 56 |
| [src/Embe.C2C.Application/Commands/Images/Handlers/ProcessUploadedImageHandler.cs](/src/Embe.C2C.Application/Commands/Images/Handlers/ProcessUploadedImageHandler.cs) | C# | 50 | 0 | 6 | 56 |
| [src/Embe.C2C.Application/Commands/Images/ProcessUploadedImageCommand.cs](/src/Embe.C2C.Application/Commands/Images/ProcessUploadedImageCommand.cs) | C# | 6 | 0 | 1 | 7 |
| [src/Embe.C2C.Application/Commands/Judgements/Handlers/JudgeHandler.cs](/src/Embe.C2C.Application/Commands/Judgements/Handlers/JudgeHandler.cs) | C# | 15 | 0 | 1 | 16 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/DeleteSearchProfileCommand.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/DeleteSearchProfileCommand.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/CreateSearchProfileHandler.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/CreateSearchProfileHandler.cs) | C# | 17 | 0 | 2 | 19 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/DeleteSearchProfileHandler.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/DeleteSearchProfileHandler.cs) | C# | 65 | 0 | 6 | 71 |
| [src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/UpdateSearchProfileHandler.cs](/src/Embe.C2C.Application/Commands/SearchProfiles/Handlers/UpdateSearchProfileHandler.cs) | C# | 23 | 0 | 1 | 24 |
| [src/Embe.C2C.Application/Commands/TransactionalCommandHandler.cs](/src/Embe.C2C.Application/Commands/TransactionalCommandHandler.cs) | C# | -47 | 0 | -9 | -56 |
| [src/Embe.C2C.Application/Commands/Users/AddImageCommand.cs](/src/Embe.C2C.Application/Commands/Users/AddImageCommand.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/AddImageHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/AddImageHandler.cs) | C# | 59 | 0 | 6 | 65 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/GenerateCandidatesHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/GenerateCandidatesHandler.cs) | C# | 0 | 0 | 1 | 1 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/RegisterHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/RegisterHandler.cs) | C# | 0 | 0 | 5 | 5 |
| [src/Embe.C2C.Application/Commands/Users/Handlers/UpdateHandler.cs](/src/Embe.C2C.Application/Commands/Users/Handlers/UpdateHandler.cs) | C# | -4 | 1 | 0 | -3 |
| [src/Embe.C2C.Application/Commands/Users/UpdateCommand.cs](/src/Embe.C2C.Application/Commands/Users/UpdateCommand.cs) | C# | -1 | 0 | 0 | -1 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/Matching.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/Matching.cs) | C# | 6 | 0 | 0 | 6 |
| [src/Embe.C2C.Application/Dtos/Read/Aggregates/User.cs](/src/Embe.C2C.Application/Dtos/Read/Aggregates/User.cs) | C# | 6 | 0 | 1 | 7 |
| [src/Embe.C2C.Application/Dtos/Read/ValueObjects/FileDetailsDto.cs](/src/Embe.C2C.Application/Dtos/Read/ValueObjects/FileDetailsDto.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Dtos/Read/Variants/Aggregates/UserVariant.cs](/src/Embe.C2C.Application/Dtos/Read/Variants/Aggregates/UserVariant.cs) | C# | -1 | 0 | -1 | -2 |
| [src/Embe.C2C.Application/Dtos/UrlGenerator.cs](/src/Embe.C2C.Application/Dtos/UrlGenerator.cs) | C# | 5 | 0 | 0 | 5 |
| [src/Embe.C2C.Application/EventHandlers/DomainEventHandler.cs](/src/Embe.C2C.Application/EventHandlers/DomainEventHandler.cs) | C# | 29 | 0 | 2 | 31 |
| [src/Embe.C2C.Application/EventHandlers/IntegrationEventHandler.cs](/src/Embe.C2C.Application/EventHandlers/IntegrationEventHandler.cs) | C# | 46 | 0 | 3 | 49 |
| [src/Embe.C2C.Application/Events/Images/ImageRemovedEvent.cs](/src/Embe.C2C.Application/Events/Images/ImageRemovedEvent.cs) | C# | 3 | 0 | 3 | 6 |
| [src/Embe.C2C.Application/Events/Images/ImageStatusChangedEvent.cs](/src/Embe.C2C.Application/Events/Images/ImageStatusChangedEvent.cs) | C# | 3 | 0 | 2 | 5 |
| [src/Embe.C2C.Application/Events/IntegrationEvent.cs](/src/Embe.C2C.Application/Events/IntegrationEvent.cs) | C# | 2 | 0 | 1 | 3 |
| [src/Embe.C2C.Application/Extensions/Domain/Aggregates/MatchingExtensions.cs](/src/Embe.C2C.Application/Extensions/Domain/Aggregates/MatchingExtensions.cs) | C# | 6 | 0 | 0 | 6 |
| [src/Embe.C2C.Application/Extensions/ServiceCollectionExtensions.cs](/src/Embe.C2C.Application/Extensions/ServiceCollectionExtensions.cs) | C# | 4 | 0 | 1 | 5 |
| [src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingByIdHandler.cs](/src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingByIdHandler.cs) | C# | 9 | 0 | 0 | 9 |
| [src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingsHandler.cs](/src/Embe.C2C.Application/Queries/Matchings/Handlers/GetMatchingsHandler.cs) | C# | 8 | 0 | 0 | 8 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/SKILL.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/SKILL.md) | Skill | 169 | 0 | 54 | 223 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/aws.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/aws.md) | Markdown | 119 | 0 | 58 | 177 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/azure.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/azure.md) | Markdown | 225 | 0 | 92 | 317 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/cicd.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/cicd.md) | Markdown | 259 | 0 | 84 | 343 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/docker-compose.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/docker-compose.md) | Markdown | 101 | 0 | 55 | 156 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/github-actions-azure-csharp.yml](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/github-actions-azure-csharp.yml) | YAML | 43 | 3 | 8 | 54 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/github-actions-azure-typescript.yml](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/github-actions-azure-typescript.yml) | YAML | 42 | 3 | 9 | 54 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/javascript.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/javascript.md) | Markdown | 95 | 0 | 32 | 127 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/kubernetes.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/kubernetes.md) | Markdown | 158 | 0 | 79 | 237 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/preflight.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-deployment/references/preflight.md) | Markdown | 127 | 0 | 63 | 190 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-init/SKILL.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-init/SKILL.md) | Skill | 118 | 0 | 29 | 147 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-init/references/init-workflow.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-init/references/init-workflow.md) | Markdown | 95 | 0 | 29 | 124 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-init/references/templates.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-init/references/templates.md) | Markdown | 69 | 0 | 24 | 93 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/SKILL.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/SKILL.md) | Skill | 146 | 0 | 52 | 198 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/references/diagnostics-bridge.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/references/diagnostics-bridge.md) | Markdown | 156 | 0 | 54 | 210 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/references/monitoring.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/references/monitoring.md) | Markdown | 111 | 0 | 51 | 162 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/references/playwright-handoff.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-monitoring/references/playwright-handoff.md) | Markdown | 15 | 0 | 7 | 22 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/SKILL.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/SKILL.md) | Skill | 167 | 0 | 38 | 205 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/agent-workflows.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/agent-workflows.md) | Markdown | 83 | 0 | 37 | 120 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/app-commands.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/app-commands.md) | Markdown | 95 | 0 | 29 | 124 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/detection.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/detection.md) | Markdown | 111 | 1 | 49 | 161 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/resource-management.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/resource-management.md) | Markdown | 29 | 0 | 10 | 39 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/safety-guardrails.md](/src/Embe.C2C.Aspire/.agents/skills/aspire-orchestration/references/safety-guardrails.md) | Markdown | 189 | 0 | 84 | 273 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire/SKILL.md](/src/Embe.C2C.Aspire/.agents/skills/aspire/SKILL.md) | Skill | 133 | 0 | 27 | 160 |
| [src/Embe.C2C.Aspire/.agents/skills/aspire/references/aspire-13-3-breaking-changes.md](/src/Embe.C2C.Aspire/.agents/skills/aspire/references/aspire-13-3-breaking-changes.md) | Markdown | 88 | 0 | 23 | 111 |
| [src/Embe.C2C.Aspire/apphost.cs](/src/Embe.C2C.Aspire/apphost.cs) | C# | 52 | 0 | 10 | 62 |
| [src/Embe.C2C.Aspire/apphost.run.json](/src/Embe.C2C.Aspire/apphost.run.json) | JSON | 30 | 0 | 1 | 31 |
| [src/Embe.C2C.Aspire/aspire.config.json](/src/Embe.C2C.Aspire/aspire.config.json) | JSON | 8 | 0 | 0 | 8 |
| [src/Embe.C2C.Domain/Aggregates/SearchProfiles/SearchProfile.cs](/src/Embe.C2C.Domain/Aggregates/SearchProfiles/SearchProfile.cs) | C# | 4 | 0 | 2 | 6 |
| [src/Embe.C2C.Domain/Aggregates/Users/Events/UserImageStatusChangedEvent.cs](/src/Embe.C2C.Domain/Aggregates/Users/Events/UserImageStatusChangedEvent.cs) | C# | 8 | 0 | 2 | 10 |
| [src/Embe.C2C.Domain/Aggregates/Users/User.cs](/src/Embe.C2C.Domain/Aggregates/Users/User.cs) | C# | 9 | 0 | 1 | 10 |
| [src/Embe.C2C.Domain/Entities/File.cs](/src/Embe.C2C.Domain/Entities/File.cs) | C# | 4 | 0 | 0 | 4 |
| [src/Embe.C2C.Domain/Services/SearchProfileService.cs](/src/Embe.C2C.Domain/Services/SearchProfileService.cs) | C# | 76 | 0 | 9 | 85 |
| [src/Embe.C2C.Domain/ValueObjects/FileDetails.cs](/src/Embe.C2C.Domain/ValueObjects/FileDetails.cs) | C# | 8 | 0 | 1 | 9 |
| [src/Embe.C2C.Functions/Embe.C2C.Functions.csproj](/src/Embe.C2C.Functions/Embe.C2C.Functions.csproj) | XML | 24 | 0 | 5 | 29 |
| [src/Embe.C2C.Functions/ImageProcessor.cs](/src/Embe.C2C.Functions/ImageProcessor.cs) | C# | 20 | 0 | 3 | 23 |
| [src/Embe.C2C.Functions/Program.cs](/src/Embe.C2C.Functions/Program.cs) | C# | 17 | 0 | 5 | 22 |
| [src/Embe.C2C.Functions/Properties/launchSettings.json](/src/Embe.C2C.Functions/Properties/launchSettings.json) | JSON | 9 | 0 | 0 | 9 |
| [src/Embe.C2C.Functions/host.json](/src/Embe.C2C.Functions/host.json) | JSON | 4 | 0 | 1 | 5 |
| [src/Embe.C2C.Infrastructure/ApiConfiguration.cs](/src/Embe.C2C.Infrastructure/ApiConfiguration.cs) | C# | 6 | 0 | 1 | 7 |
| [src/Embe.C2C.Infrastructure/Azure/AzureAIContentSafetyService.cs](/src/Embe.C2C.Infrastructure/Azure/AzureAIContentSafetyService.cs) | C# | 29 | 0 | 5 | 34 |
| [src/Embe.C2C.Infrastructure/Azure/BlobStorageFileService.cs](/src/Embe.C2C.Infrastructure/Azure/BlobStorageFileService.cs) | C# | 75 | 0 | 11 | 86 |
| [src/Embe.C2C.Infrastructure/Azure/ServiceBusWorkItemService.cs](/src/Embe.C2C.Infrastructure/Azure/ServiceBusWorkItemService.cs) | C# | 2 | 0 | 0 | 2 |
| [src/Embe.C2C.Infrastructure/Ef/Configurations/UserConfiguration.cs](/src/Embe.C2C.Infrastructure/Ef/Configurations/UserConfiguration.cs) | C# | 1 | 0 | 0 | 1 |
| [src/Embe.C2C.Infrastructure/Ef/Contexts/C2CContext.cs](/src/Embe.C2C.Infrastructure/Ef/Contexts/C2CContext.cs) | C# | 6 | 0 | 2 | 8 |
| [src/Embe.C2C.Infrastructure/Embe.C2C.Infrastructure.csproj](/src/Embe.C2C.Infrastructure/Embe.C2C.Infrastructure.csproj) | XML | 2 | 0 | 0 | 2 |
| [src/Embe.C2C.Infrastructure/Extensions/ServiceCollectionExtensions.cs](/src/Embe.C2C.Infrastructure/Extensions/ServiceCollectionExtensions.cs) | C# | 20 | 0 | 1 | 21 |
| [src/Embe.C2C.Infrastructure/Migrations/20260712110904\_RequireGenderAndLocation.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260712110904_RequireGenderAndLocation.Designer.cs) | C# | 900 | 2 | 328 | 1,230 |
| [src/Embe.C2C.Infrastructure/Migrations/20260712110904\_RequireGenderAndLocation.cs](/src/Embe.C2C.Infrastructure/Migrations/20260712110904_RequireGenderAndLocation.cs) | C# | 46 | 3 | 6 | 55 |
| [src/Embe.C2C.Infrastructure/Migrations/20260712112405\_AspNetUserDomainUserCascadeDelete.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260712112405_AspNetUserDomainUserCascadeDelete.Designer.cs) | C# | 900 | 2 | 328 | 1,230 |
| [src/Embe.C2C.Infrastructure/Migrations/20260712112405\_AspNetUserDomainUserCascadeDelete.cs](/src/Embe.C2C.Infrastructure/Migrations/20260712112405_AspNetUserDomainUserCascadeDelete.cs) | C# | 33 | 3 | 6 | 42 |
| [src/Embe.C2C.Infrastructure/Migrations/20260713072714\_MakeGenderAndLocationOptional.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260713072714_MakeGenderAndLocationOptional.Designer.cs) | C# | 899 | 2 | 328 | 1,229 |
| [src/Embe.C2C.Infrastructure/Migrations/20260713072714\_MakeGenderAndLocationOptional.cs](/src/Embe.C2C.Infrastructure/Migrations/20260713072714_MakeGenderAndLocationOptional.cs) | C# | 46 | 3 | 6 | 55 |
| [src/Embe.C2C.Infrastructure/Migrations/20260714125326\_ImageStatus.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260714125326_ImageStatus.Designer.cs) | C# | 901 | 2 | 329 | 1,232 |
| [src/Embe.C2C.Infrastructure/Migrations/20260714125326\_ImageStatus.cs](/src/Embe.C2C.Infrastructure/Migrations/20260714125326_ImageStatus.cs) | C# | 23 | 3 | 4 | 30 |
| [src/Embe.C2C.Infrastructure/Migrations/20260714142326\_ImageNameUniqueIndex.Designer.cs](/src/Embe.C2C.Infrastructure/Migrations/20260714142326_ImageNameUniqueIndex.Designer.cs) | C# | 903 | 2 | 330 | 1,235 |
| [src/Embe.C2C.Infrastructure/Migrations/20260714142326\_ImageNameUniqueIndex.cs](/src/Embe.C2C.Infrastructure/Migrations/20260714142326_ImageNameUniqueIndex.cs) | C# | 22 | 3 | 4 | 29 |
| [src/Embe.C2C.Infrastructure/Migrations/C2CContextModelSnapshot.cs](/src/Embe.C2C.Infrastructure/Migrations/C2CContextModelSnapshot.cs) | C# | 4 | 0 | 2 | 6 |
| [src/Embe.C2C.Infrastructure/SignalR/SignalRNotificationService.cs](/src/Embe.C2C.Infrastructure/SignalR/SignalRNotificationService.cs) | C# | 18 | 0 | 2 | 20 |
| [src/Embe.C2C.Infrastructure/SignalR/SignalRServiceHubContextPool.cs](/src/Embe.C2C.Infrastructure/SignalR/SignalRServiceHubContextPool.cs) | C# | 19 | 0 | 4 | 23 |
| [src/Embe.C2C.slnx](/src/Embe.C2C.slnx) | XML | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/next.config.ts](/src/embe.c2c.frontend/next.config.ts) | TypeScript | 15 | 0 | 0 | 15 |
| [src/embe.c2c.frontend/package-lock.json](/src/embe.c2c.frontend/package-lock.json) | JSON | 1,336 | 0 | 0 | 1,336 |
| [src/embe.c2c.frontend/package.json](/src/embe.c2c.frontend/package.json) | JSON | 7 | 0 | 0 | 7 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/matches/\[matchId\]/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/matches/%5BmatchId%5D/page.tsx) | TypeScript JSX | -1 | 0 | 0 | -1 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/\[searchProfileId\]/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/%5BsearchProfileId%5D/page.tsx) | TypeScript JSX | -1 | 0 | 0 | -1 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/new/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/new/page.tsx) | TypeScript JSX | 17 | 0 | 3 | 20 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search-profile/page.tsx) | TypeScript JSX | 1 | 0 | 1 | 2 |
| [src/embe.c2c.frontend/src/app/(pages)/protected/search/page.tsx](/src/embe.c2c.frontend/src/app/(pages)/protected/search/page.tsx) | TypeScript JSX | 2 | 0 | 0 | 2 |
| [src/embe.c2c.frontend/src/app/globals.css](/src/embe.c2c.frontend/src/app/globals.css) | PostCSS | 42 | 0 | 5 | 47 |
| [src/embe.c2c.frontend/src/app/layout.tsx](/src/embe.c2c.frontend/src/app/layout.tsx) | TypeScript JSX | 1 | 0 | -1 | 0 |
| [src/embe.c2c.frontend/src/features/auth/actions/account-exists/actions.ts](/src/embe.c2c.frontend/src/features/auth/actions/account-exists/actions.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/features/auth/actions/action.ts](/src/embe.c2c.frontend/src/features/auth/actions/action.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/features/auth/actions/register/types.ts](/src/embe.c2c.frontend/src/features/auth/actions/register/types.ts) | TypeScript | 2 | 0 | 0 | 2 |
| [src/embe.c2c.frontend/src/features/auth/components/BasicProfileForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/BasicProfileForm.tsx) | TypeScript JSX | 15 | 0 | 0 | 15 |
| [src/embe.c2c.frontend/src/features/auth/components/ImagesForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/ImagesForm.tsx) | TypeScript JSX | -18 | 0 | -5 | -23 |
| [src/embe.c2c.frontend/src/features/auth/components/LoginForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/LoginForm.tsx) | TypeScript JSX | -1 | 0 | 0 | -1 |
| [src/embe.c2c.frontend/src/features/auth/components/RegisterForm.tsx](/src/embe.c2c.frontend/src/features/auth/components/RegisterForm.tsx) | TypeScript JSX | -23 | 0 | -5 | -28 |
| [src/embe.c2c.frontend/src/features/likes/components/Likes.tsx](/src/embe.c2c.frontend/src/features/likes/components/Likes.tsx) | TypeScript JSX | 2 | 0 | 0 | 2 |
| [src/embe.c2c.frontend/src/features/matches/actions/action.ts](/src/embe.c2c.frontend/src/features/matches/actions/action.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/features/matches/components/Match.tsx](/src/embe.c2c.frontend/src/features/matches/components/Match.tsx) | TypeScript JSX | 9 | 0 | 5 | 14 |
| [src/embe.c2c.frontend/src/features/matches/components/MatchCompact.tsx](/src/embe.c2c.frontend/src/features/matches/components/MatchCompact.tsx) | TypeScript JSX | 5 | 0 | 0 | 5 |
| [src/embe.c2c.frontend/src/features/matches/components/Matches.tsx](/src/embe.c2c.frontend/src/features/matches/components/Matches.tsx) | TypeScript JSX | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/features/matches/components/MessageBrief.tsx](/src/embe.c2c.frontend/src/features/matches/components/MessageBrief.tsx) | TypeScript JSX | 8 | 0 | 0 | 8 |
| [src/embe.c2c.frontend/src/features/matches/components/UserCompact.tsx](/src/embe.c2c.frontend/src/features/matches/components/UserCompact.tsx) | TypeScript JSX | -3 | 0 | 0 | -3 |
| [src/embe.c2c.frontend/src/features/me/actions/action.ts](/src/embe.c2c.frontend/src/features/me/actions/action.ts) | TypeScript | 1 | 0 | 3 | 4 |
| [src/embe.c2c.frontend/src/features/me/actions/type.ts](/src/embe.c2c.frontend/src/features/me/actions/type.ts) | TypeScript | 5 | 0 | 0 | 5 |
| [src/embe.c2c.frontend/src/features/me/components/Me.tsx](/src/embe.c2c.frontend/src/features/me/components/Me.tsx) | TypeScript JSX | 85 | 1 | 18 | 104 |
| [src/embe.c2c.frontend/src/features/me/components/MyInfoForm.tsx](/src/embe.c2c.frontend/src/features/me/components/MyInfoForm.tsx) | TypeScript JSX | 4 | 0 | 0 | 4 |
| [src/embe.c2c.frontend/src/features/notifications/actions/action.ts](/src/embe.c2c.frontend/src/features/notifications/actions/action.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/features/search-profiles/actions.ts](/src/embe.c2c.frontend/src/features/search-profiles/actions.ts) | TypeScript | 10 | 0 | 1 | 11 |
| [src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileForm.tsx](/src/embe.c2c.frontend/src/features/search-profiles/components/SearchProfileForm.tsx) | TypeScript JSX | 19 | 0 | 8 | 27 |
| [src/embe.c2c.frontend/src/features/search/actions/action.ts](/src/embe.c2c.frontend/src/features/search/actions/action.ts) | TypeScript | 2 | 0 | 0 | 2 |
| [src/embe.c2c.frontend/src/features/search/actions/type.ts](/src/embe.c2c.frontend/src/features/search/actions/type.ts) | TypeScript | 10 | 0 | 1 | 11 |
| [src/embe.c2c.frontend/src/features/search/components/FindUserDating.tsx](/src/embe.c2c.frontend/src/features/search/components/FindUserDating.tsx) | TypeScript JSX | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/features/search/components/Search.tsx](/src/embe.c2c.frontend/src/features/search/components/Search.tsx) | TypeScript JSX | -1 | 0 | -1 | -2 |
| [src/embe.c2c.frontend/src/features/search/components/SearchProfileCompact.tsx](/src/embe.c2c.frontend/src/features/search/components/SearchProfileCompact.tsx) | TypeScript JSX | 21 | 0 | -2 | 19 |
| [src/embe.c2c.frontend/src/features/search/components/SearchProfiles.tsx](/src/embe.c2c.frontend/src/features/search/components/SearchProfiles.tsx) | TypeScript JSX | 9 | 0 | 0 | 9 |
| [src/embe.c2c.frontend/src/middleware.ts](/src/embe.c2c.frontend/src/middleware.ts) | TypeScript | 54 | 0 | 9 | 63 |
| [src/embe.c2c.frontend/src/proxy.ts](/src/embe.c2c.frontend/src/proxy.ts) | TypeScript | -22 | 0 | -1 | -23 |
| [src/embe.c2c.frontend/src/shared/actions/geography/actions.ts](/src/embe.c2c.frontend/src/shared/actions/geography/actions.ts) | TypeScript | 3 | 0 | 0 | 3 |
| [src/embe.c2c.frontend/src/shared/actions/user/action.ts](/src/embe.c2c.frontend/src/shared/actions/user/action.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/shared/api.ts](/src/embe.c2c.frontend/src/shared/api.ts) | TypeScript | -100 | 0 | -29 | -129 |
| [src/embe.c2c.frontend/src/shared/apis/api.ts](/src/embe.c2c.frontend/src/shared/apis/api.ts) | TypeScript | 83 | 0 | 28 | 111 |
| [src/embe.c2c.frontend/src/shared/apis/type.ts](/src/embe.c2c.frontend/src/shared/apis/type.ts) | TypeScript | 23 | 0 | 4 | 27 |
| [src/embe.c2c.frontend/src/shared/components/Links/Link.tsx](/src/embe.c2c.frontend/src/shared/components/Links/Link.tsx) | TypeScript JSX | 19 | 0 | 4 | 23 |
| [src/embe.c2c.frontend/src/shared/components/buttons/Button.tsx](/src/embe.c2c.frontend/src/shared/components/buttons/Button.tsx) | TypeScript JSX | 5 | 0 | 1 | 6 |
| [src/embe.c2c.frontend/src/shared/components/images/Image.tsx](/src/embe.c2c.frontend/src/shared/components/images/Image.tsx) | TypeScript JSX | 36 | 0 | 4 | 40 |
| [src/embe.c2c.frontend/src/shared/components/images/ImageGallery.tsx](/src/embe.c2c.frontend/src/shared/components/images/ImageGallery.tsx) | TypeScript JSX | -2 | 0 | 0 | -2 |
| [src/embe.c2c.frontend/src/shared/components/infos/Alert.tsx](/src/embe.c2c.frontend/src/shared/components/infos/Alert.tsx) | TypeScript JSX | 25 | 0 | 3 | 28 |
| [src/embe.c2c.frontend/src/shared/components/infos/AlertDialog.tsx](/src/embe.c2c.frontend/src/shared/components/infos/AlertDialog.tsx) | TypeScript JSX | 39 | 0 | 0 | 39 |
| [src/embe.c2c.frontend/src/shared/components/infos/InfoModal.tsx](/src/embe.c2c.frontend/src/shared/components/infos/InfoModal.tsx) | TypeScript JSX | 34 | 0 | 5 | 39 |
| [src/embe.c2c.frontend/src/shared/components/infos/InfoSurface.tsx](/src/embe.c2c.frontend/src/shared/components/infos/InfoSurface.tsx) | TypeScript JSX | 23 | 0 | 6 | 29 |
| [src/embe.c2c.frontend/src/shared/components/infos/InfoWindow.tsx](/src/embe.c2c.frontend/src/shared/components/infos/InfoWindow.tsx) | TypeScript JSX | -20 | 0 | -6 | -26 |
| [src/embe.c2c.frontend/src/shared/components/inputs/ErrorMessage.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/ErrorMessage.tsx) | TypeScript JSX | 13 | 0 | 0 | 13 |
| [src/embe.c2c.frontend/src/shared/components/inputs/date-input/DateInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/date-input/DateInput.tsx) | TypeScript JSX | 5 | 0 | 1 | 6 |
| [src/embe.c2c.frontend/src/shared/components/inputs/dropdown-input/DropDownInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/dropdown-input/DropDownInput.tsx) | TypeScript JSX | 1 | 0 | 1 | 2 |
| [src/embe.c2c.frontend/src/shared/components/inputs/dual-range-input/DualRangeInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/dual-range-input/DualRangeInput.tsx) | TypeScript JSX | 3 | 0 | 0 | 3 |
| [src/embe.c2c.frontend/src/shared/components/inputs/email-input/EmailInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/email-input/EmailInput.tsx) | TypeScript JSX | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGalleryInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/image/gallery/ImageGalleryInput.tsx) | TypeScript JSX | 7 | 0 | -1 | 6 |
| [src/embe.c2c.frontend/src/shared/components/inputs/location-input/LocationInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/location-input/LocationInput.tsx) | TypeScript JSX | 12 | 0 | 0 | 12 |
| [src/embe.c2c.frontend/src/shared/components/inputs/select-input/SelectInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/select-input/SelectInput.tsx) | TypeScript JSX | 22 | 0 | 0 | 22 |
| [src/embe.c2c.frontend/src/shared/components/inputs/single-range-input/SingleRangeInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/single-range-input/SingleRangeInput.tsx) | TypeScript JSX | 25 | 0 | 3 | 28 |
| [src/embe.c2c.frontend/src/shared/components/inputs/text-area-input/TextAreaInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/text-area-input/TextAreaInput.tsx) | TypeScript JSX | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/shared/components/inputs/text-input/TextInput.tsx](/src/embe.c2c.frontend/src/shared/components/inputs/text-input/TextInput.tsx) | TypeScript JSX | 4 | 0 | 0 | 4 |
| [src/embe.c2c.frontend/src/shared/components/modal/LargeModal.module.css](/src/embe.c2c.frontend/src/shared/components/modal/LargeModal.module.css) | PostCSS | 11 | 0 | 3 | 14 |
| [src/embe.c2c.frontend/src/shared/components/modal/LargeModal.tsx](/src/embe.c2c.frontend/src/shared/components/modal/LargeModal.tsx) | TypeScript JSX | 50 | 0 | 8 | 58 |
| [src/embe.c2c.frontend/src/shared/components/modal/Modal.module.css](/src/embe.c2c.frontend/src/shared/components/modal/Modal.module.css) | PostCSS | -15 | 0 | -2 | -17 |
| [src/embe.c2c.frontend/src/shared/components/modal/Modal.tsx](/src/embe.c2c.frontend/src/shared/components/modal/Modal.tsx) | TypeScript JSX | -46 | 0 | -7 | -53 |
| [src/embe.c2c.frontend/src/shared/components/modal/SmallModal.module.css](/src/embe.c2c.frontend/src/shared/components/modal/SmallModal.module.css) | PostCSS | 5 | 0 | 2 | 7 |
| [src/embe.c2c.frontend/src/shared/components/modal/SmallModal.tsx](/src/embe.c2c.frontend/src/shared/components/modal/SmallModal.tsx) | TypeScript JSX | 49 | 0 | 9 | 58 |
| [src/embe.c2c.frontend/src/shared/components/scroll/infinite-scroll/InfiniteScroll.tsx](/src/embe.c2c.frontend/src/shared/components/scroll/infinite-scroll/InfiniteScroll.tsx) | TypeScript JSX | 27 | 0 | 8 | 35 |
| [src/embe.c2c.frontend/src/shared/components/user/Profile.tsx](/src/embe.c2c.frontend/src/shared/components/user/Profile.tsx) | TypeScript JSX | 5 | 0 | 1 | 6 |
| [src/embe.c2c.frontend/src/shared/security/constants.ts](/src/embe.c2c.frontend/src/shared/security/constants.ts) | TypeScript | -1 | 0 | 0 | -1 |
| [src/embe.c2c.frontend/src/shared/security/functions.ts](/src/embe.c2c.frontend/src/shared/security/functions.ts) | TypeScript | -8 | 0 | -2 | -10 |
| [src/embe.c2c.frontend/src/shared/security/types.ts](/src/embe.c2c.frontend/src/shared/security/types.ts) | TypeScript | 9 | 0 | 3 | 12 |
| [src/embe.c2c.frontend/src/shared/signal-r.ts](/src/embe.c2c.frontend/src/shared/signal-r.ts) | TypeScript | 83 | 0 | 30 | 113 |
| [src/embe.c2c.frontend/src/shared/types/domain/aggregates.ts](/src/embe.c2c.frontend/src/shared/types/domain/aggregates.ts) | TypeScript | 4 | 0 | 0 | 4 |
| [src/embe.c2c.frontend/src/shared/types/domain/entities.ts](/src/embe.c2c.frontend/src/shared/types/domain/entities.ts) | TypeScript | 1 | 0 | 0 | 1 |
| [src/embe.c2c.frontend/src/shared/types/domain/value-objects.ts](/src/embe.c2c.frontend/src/shared/types/domain/value-objects.ts) | TypeScript | 6 | 0 | 0 | 6 |
| [src/embe.c2c.frontend/src/shared/types/dtos/types.ts](/src/embe.c2c.frontend/src/shared/types/dtos/types.ts) | TypeScript | 1 | 0 | 0 | 1 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details