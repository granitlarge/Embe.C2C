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
    UserManager<MyIdentityUser> userManager
) : IdentityDbContext<MyIdentityUser>(options), IC2CContext
{
    private readonly UserManager<MyIdentityUser> _userManager = userManager;

    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Judgement> Judgements { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

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

    public async Task<TypedResult<RegisterUserFailureReason, IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var identityUser = new MyIdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(identityUser, password);
        if (result.Succeeded)
        {
            return TypedResult<RegisterUserFailureReason, IIdentityUser>.Success(identityUser);
        }
        else
        {
            var failureReason = result.Errors.Any(e => e.Code == "PasswordTooShort" || e.Code == "PasswordRequiresNonAlphanumeric" || e.Code == "PasswordRequiresDigit" || e.Code == "PasswordRequiresUpper" || e.Code == "PasswordRequiresLower")
                ? RegisterUserFailureReason.WeakPassword
                : RegisterUserFailureReason.UnknownError;

            return TypedResult<RegisterUserFailureReason, IIdentityUser>.Failure(failureReason, string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<ResultBase<ResetPasswordFailureReason>> ResetPasswordAsync(string identityUserId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return ResultBase<ResetPasswordFailureReason>.Failure(ResetPasswordFailureReason.UserNotFound, "User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            return ResultBase<ResetPasswordFailureReason>.Success();
        }
        else
        {
            var failureReason = result.Errors.Any(e => e.Code == "PasswordTooShort" || e.Code == "PasswordRequiresNonAlphanumeric" || e.Code == "PasswordRequiresDigit" || e.Code == "PasswordRequiresUpper" || e.Code == "PasswordRequiresLower")
                ? ResetPasswordFailureReason.WeakPassword
                : ResetPasswordFailureReason.UnknownError;

            return ResultBase<ResetPasswordFailureReason>.Failure(failureReason, string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<ResultBase<DeleteUserFailureReason>> DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return ResultBase<DeleteUserFailureReason>.Failure(DeleteUserFailureReason.UserNotFound, "User not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return ResultBase<DeleteUserFailureReason>.Success();
        }
        else
        {
            return ResultBase<DeleteUserFailureReason>.Failure(DeleteUserFailureReason.UnknownError, string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
        }
    }
}