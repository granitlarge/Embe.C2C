using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class NotificationRepository(C2CContext context) : INotificationRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<Notification> Set => new MyDbSet<Notification>(_context.Notifications);

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Notifications.SingleOrDefaultAsync(not => not.Id == id, cancellationToken);
    }

    public Task<bool> HasUnreadAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context.Notifications.AnyAsync(not => not.RecipientUserId == userId && not.ReadAt == null, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Notification>> GetNotificationsAsync(Guid recipientUserId, int page, int pageSize, CancellationToken cancellationToken)
    {
        return _context.Notifications
            .Where(n => n.RecipientUserId == recipientUserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}