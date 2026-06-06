using Embe.C2C.Application.Commands.Users.Handlers;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Application.Abstractions.Repos
{
    public interface IIdentityUser
    {
        string Id { get; set; }
        string? Email { get; set; }
        string? PasswordHash { get; set; }
    }

    public interface IC2CContext
    {
        public DbSet<User> DomainUsers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Judgement> Judgements { get; set; }
        public DbSet<Matching> Matchings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public Task<TypedResult<RegisterUserFailureReason, IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default);
        public Task<ResultBase<ResetPasswordFailureReason>> ResetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken = default);
        public Task<ResultBase<DeleteUserFailureReason>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    }
}

public enum DeleteUserFailureReason
{
    UserNotFound,
    UnknownError
}

public enum ResetPasswordFailureReason
{
    UserNotFound,
    WeakPassword,
    UnknownError
}