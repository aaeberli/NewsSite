using Microsoft.AspNet.Identity;
using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace NewsSite.WebApplication.Infrastrucutre
{
    /// <summary>
    /// Standards implementation for IUserProvider
    /// </summary>
    public class UserProvider : IUserProvider
    {
        private string userId;
        private IList<string> roles;

        public void Register<TUser>(IPrincipal user, UserManager<TUser> userManager)
            where TUser : class, IUser<string>
        {
            userId = user.Identity.GetUserId();
            if (userId != null)
                roles = userManager.GetRoles(userId);
        }

        public string GetUserId()
        {
            return userId;
        }

        public IList<string> GetRoles()
        {
            return roles;
        }
    }
}