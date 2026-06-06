using Embe.C2C.Application.Abstractions.Repos;
using Microsoft.AspNetCore.Identity;

namespace Embe.C2C.Infrastructure.Identity;

public class MyIdentityUser : IdentityUser, IIdentityUser
{

}