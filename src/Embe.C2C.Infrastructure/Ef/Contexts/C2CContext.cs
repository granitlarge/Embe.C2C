using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Entities;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Contexts;

public class MyDbSet<T>(DbSet<T> dbSet) : IDbSet<T> where T : class
{
    private readonly DbSet<T> _dbSet = dbSet;

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public ValueTask<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync(keyValues, cancellationToken);
    }
}

public class C2CContext
(
    DbContextOptions<C2CContext> options
) : IdentityDbContext<MyIdentityUser>(options), IRepository
{
    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Judgement> Judgements { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Blocking> Blockings { get; set; }
    public DbSet<CandidateEntity> Candidates { get; set; }
    public DbSet<SearchProfile> SearchProfiles { get; set; }
    public DbSet<AdminArea> AdminAreas { get; set; }

    public IImmutableList<DomainEvent> DomainEvents
    {
        get
        {
            return ChangeTracker.Entries()
                .Select(e => e.Entity)
                .OfType<DomainEventCollector>()
                .SelectMany(c => c.DomainEvents)
                .ToImmutableList();
        }
    }

    public IQueryable<User> DomainUsersQuery
    {
        get
        {
            return DomainUsers;
        }
    }

    public IQueryable<Account> AccountsQuery
    {
        get
        {
            return Accounts;
        }
    }

    public IQueryable<Judgement> JudgementsQuery
    {
        get
        {
            return Judgements;
        }
    }

    public IQueryable<Matching> MatchingsQuery
    {
        get
        {
            return Matchings.Include(m => m.Conversation);
        }
    }

    public IQueryable<Notification> NotificationsQuery
    {
        get
        {
            return Notifications;
        }
    }

    IDbSet<User> ISparseRepository.DomainUsers
    {
        get
        {
            return new MyDbSet<User>(DomainUsers);
        }
    }

    IDbSet<Account> ISparseRepository.Accounts
    {
        get
        {
            return new MyDbSet<Account>(Accounts);
        }
    }

    IDbSet<Judgement> ISparseRepository.Judgements
    {
        get
        {
            return new MyDbSet<Judgement>(Judgements);
        }
    }

    IDbSet<Matching> ISparseRepository.Matchings
    {
        get
        {
            return new MyDbSet<Matching>(Matchings);
        }
    }

    IDbSet<Notification> ISparseRepository.Notifications
    {
        get
        {
            return new MyDbSet<Notification>(Notifications);
        }
    }

    public IQueryable<Message> MessagesQuery => Messages;

    IDbSet<Message> ISparseRepository.Messages => new MyDbSet<Message>(Messages);

    IDbSet<Blocking> ISparseRepository.Blockings => new MyDbSet<Blocking>(Blockings);

    public IQueryable<Blocking> BlockingsQuery => Blockings;

    IDbSet<SearchProfile> ISparseRepository.SearchProfiles => new MyDbSet<SearchProfile>(SearchProfiles);

    public IQueryable<SearchProfile> SearchProfilesQuery => SearchProfiles;

    public IQueryable<IAdminArea> AdminAreasQuery => AdminAreas;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(System.Data.IsolationLevel.Snapshot, cancellationToken);
    }

    public async Task<List<User>> GenerateCandidatesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingCandidates = await Candidates
            .Include(c => c.Candidate)
            .Where(c => c.UserId == userId).ToListAsync(cancellationToken);

        if (existingCandidates.Count != 0)
        {
            return [.. existingCandidates.Select(c => c.Candidate!)];
        }

        // User has not blocked Candidate
        // Candidate has not blocked User
        // User's preferences align with Candidate's information.
        // Candidate's preferences align with User's information.
        // User and Candidate are not matched.
        // User has not already judged Candidate. 2nd chances? Show the same candidate again after a certain period of time? (e.g., 1 month)
        // Candidate has not already judged User negatively.

        // Then we need to prioritize candidates based on the following criteria:
        // 1. Candidates who have liked the user
        // 2. Candidates who have not been judged by the user yet

        var query =
        (

            from user in DomainUsers
            from candidate in DomainUsers

            where 1 == 1 &&
            user.Id == userId &&
            user.Id != candidate.Id &&

            !user.Blocked!.Any(b => b.BlockedUserId == candidate.Id) &&
            !user.BlockedBy!.Any(b => b.BlockerUserId == candidate.Id) &&

            !user.Matchings1!.Any(m => m.UserId2 == candidate.Id) &&
            !user.Matchings2!.Any(m => m.UserId1 == candidate.Id) &&

            !user.JudgementsPassed!.Any(j => j.JudgeUserId == user.Id && j.JudgeeUserId == candidate.Id && !j.IsPositive) &&
            !user.JudgementsReceived!.Any(j => j.JudgeUserId == candidate.Id && j.JudgeeUserId == user.Id && !j.IsPositive)

            select candidate

        );

        var users = await query.Take(20).ToListAsync(cancellationToken);
        var candidates = users.Select(u => new CandidateEntity(userId, u.Id));
        Candidates.AddRange(candidates);
        return users;
    }

    public Task<bool> IsCandidateForUserIdAsync(Guid userId, Guid candidateUserId, CancellationToken cancellationToken = default)
    {
        return Candidates.AnyAsync(c => c.UserId == userId && c.CandidateUserId == candidateUserId, cancellationToken);
    }

    public async Task ClearCandidateForUserIdAsync(Guid userId, Guid candidateUserId, CancellationToken cancellationToken = default)
    {
        var candidate = await Candidates.FirstOrDefaultAsync(c => c.UserId == userId && c.CandidateUserId == candidateUserId, cancellationToken);
        if (candidate is not null)
        {
            Candidates.Remove(candidate);
        }
    }

    public async Task<List<IAdminArea>> SearchAdminAreasAsync
    (
        string? parentId, 
        double? longitude, 
        double? latitude, 
        int page,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var maxDistanceMeters = 1000;
        List<AdminArea> result = [];
        while (result.Count == 0  && maxDistanceMeters <= 25_000_000)
        {
            var pointFilter = (longitude.HasValue && latitude.HasValue) ? new NetTopologySuite.Geometries.Point(longitude.Value, latitude.Value) { SRID = 4326 } : null;
            var parentIdFilter = !string.IsNullOrEmpty(parentId) ? parentId : null;
            result = await AdminAreas
            .AsNoTracking()
            .Where(aa => pointFilter == null || aa.Point != null && aa.Point.Distance(pointFilter) <= maxDistanceMeters)
            .Where(aa => parentIdFilter == null || aa.ParentId == parentIdFilter)
            .OrderBy(aa => pointFilter != null && aa.Point != null ? aa.Point.Distance(pointFilter) : int.MaxValue)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
            maxDistanceMeters *= 2;
        }

        return [.. result.Cast<IAdminArea>()];
    }
}