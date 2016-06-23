using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Principal;

namespace NewsSite.Common.Abstract
{
    public interface IUserProvider
    {
        void Register<TUser>(IPrincipal user, UserManager<TUser> userManager) where TUser : class, IUser<string>;

        IList<string> GetRoles();
        string GetUserId();
    }
}