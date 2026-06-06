using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Commands.Users.Handlers;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Entities;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Contexts;

public class C2CContext
(
    DbContextOptions<C2CContext> options,
    IPasswordHasher<MyIdentityUser> passwordHasher
) : IdentityDbContext<MyIdentityUser>(options), IC2CContext
{
    private readonly IPasswordHasher<MyIdentityUser> _passwordHasher = passwordHasher;

    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Judgement> Judgements { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    public async Task<TypedResult<RegisterUserFailureReason, IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var emailExists = await Users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken);
        if (emailExists)
        {
            return TypedResult<RegisterUserFailureReason, IIdentityUser>.Failure(RegisterUserFailureReason.EmailAlreadyExists, "Email already exists.");
        }

        var identityUser = new MyIdentityUser
        {
            UserName = email,
            Email = email,
        };

        var passwordHash = _passwordHasher.HashPassword(identityUser, password);
        identityUser.PasswordHash = passwordHash;

        Users.Add(identityUser);
        return TypedResult<RegisterUserFailureReason, IIdentityUser>.Success(identityUser);
    }

    public async Task<ResultBase<ResetPasswordFailureReason>> ResetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return ResultBase<ResetPasswordFailureReason>.Failure(ResetPasswordFailureReason.UserNotFound, "User not found.");
        }
        var newPasswordHash = _passwordHasher.HashPassword(user, newPassword);
        user.PasswordHash = newPasswordHash;
        return ResultBase<ResetPasswordFailureReason>.Success();
    }

    public async Task<ResultBase<DeleteUserFailureReason>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return ResultBase<DeleteUserFailureReason>.Failure(DeleteUserFailureReason.UserNotFound, "User not found.");
        }

        Users.Remove(user);
        return ResultBase<DeleteUserFailureReason>.Success();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.CommitTransactionAsync(cancellationToken);
    }
}