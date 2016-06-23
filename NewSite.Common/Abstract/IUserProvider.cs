using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Principal;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Defines an abstraction over a facility for getting current user and role
    /// </summary>
    public interface IUserProvider
    {
        /// <summary>
        /// Initialize the component
        /// </summary>
        /// <typeparam name="TUser">Type of User</typeparam>
        /// <param name="user">The User</param>
        /// <param name="userManager">UserManager</param>
        void Register<TUser>(IPrincipal user, UserManager<TUser> userManager) where TUser : class, IUser<string>;

        /// <summary>
        /// Gets the list of current roles
        /// </summary>
        /// <returns>Current roles</returns>
        IList<string> GetRoles();

        /// <summary>
        /// Gets the current user id
        /// </summary>
        /// <returns>Current user id</returns>
        string GetUserId();
    }
}