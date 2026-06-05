using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Abstractions.Repos
{
    public abstract class C2CContext : DbContext
    {
        public abstract DbSet<User> Users { get; set; }
        public abstract DbSet<Account> Accounts { get; set; }
        public abstract DbSet<Judgement> Judgements { get; set; }
        public abstract DbSet<Matching> Matchings { get; set; }
    }
}