using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using NewsSite.WebApplication.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NewsSite.Test.Fakes
{
    internal class FakeUserProvider : IUserProvider
    {
        public IList<string> FakeRoles { get; set; }
        public string FakeUserId { get; set; }

        public IList<string> GetRoles()
        {
            return FakeRoles;
        }

        public string GetUserId()
        {
            return FakeUserId;
        }

        public void Register<TUser>(IPrincipal user, UserManager<TUser> userManager) where TUser : class, IUser<string>
        {
            // Ignore parameters
        }
    }
}
