using System.Diagnostics;
using Embe.C2C.Application;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Infrastructure.Ef;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var authService = serviceProvider.GetRequiredService<IAuthService>();
        var context = serviceProvider.GetRequiredService<C2CContext>();
        var searchProfileService = serviceProvider.GetRequiredService<SearchProfileService>();

        var identityUser1 = await authService.RegisterUserAsync("123@123.com", "Hejhopp123!");
        var identityUser2 = await authService.RegisterUserAsync("1234@1234.com", "Hejhopp123!");

        var user1 = User.Register(
            Email.Create("123@123.com").Value,
            Alias.Create("123").Value,
            BirthDate.Create(new DateOnly(1996, 08, 23)).Value,
            Gender.Male,
            null,
            [],
            null,
            identityUser1.Value.Id
        ).Value;

        var user2 = User.Register(
            Email.Create("1234@1234.com").Value,
            Alias.Create("1234").Value,
            BirthDate.Create(new DateOnly(1996, 08, 23)).Value,
            Gender.Male,
            null,
            [],
            null,
            identityUser2.Value.Id
        ).Value;

        context.DomainUsers.Add(user1);
        context.DomainUsers.Add(user2);

        var sp1 = searchProfileService.Create(user1, "123", "123", RelationshipType.Romantic,
        Engagement.Create(
            Domain.ValueObjects.Engagements.Enums.EngagementMedium.Hybrid,
            Domain.ValueObjects.Engagements.Enums.EngagementBoundedness.Ongoing,
            Domain.ValueObjects.Engagements.Enums.EngagementFrequency.Daily,
            null,
            null
        ).Value, [Gender.Male], null, null, null).Value;

        var sp2 = searchProfileService.Create(user2, "1234", "1234", RelationshipType.Romantic,
        Engagement.Create(
            Domain.ValueObjects.Engagements.Enums.EngagementMedium.Hybrid,
            Domain.ValueObjects.Engagements.Enums.EngagementBoundedness.Ongoing,
            Domain.ValueObjects.Engagements.Enums.EngagementFrequency.Daily,
            null,
            null
        ).Value, [Gender.Male], null, null, null).Value;

        var embedding = new float[1536];
        var spe1 = new SearchProfileEmbedding
        {
            SearchProfileId = sp1.Id,
            Embedding = new Pgvector.Vector(embedding)
        };
        var spe2 = new SearchProfileEmbedding
        {
            SearchProfileId = sp2.Id,
            Embedding = new Pgvector.Vector(embedding)
        };

        context.SearchProfiles.AddRange([sp1, sp2]);
        context.SearchProfileEmbeddings.AddRange([spe1, spe2]);

        await context.SaveChangesAsync();

    }
}