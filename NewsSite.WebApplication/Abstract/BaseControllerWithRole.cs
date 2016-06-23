using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NewsSite.WebApplication.Abstract
{
    /// <summary>
    /// Enhances Base Controller with Role's facilities
    /// </summary>
    public abstract class BaseControllerWithRole : BaseController
    {

        public BaseControllerWithRole(IMapperAdapter mapper, ISolutionLogger logger, IUserProvider userProvider)
            : base(mapper, logger, userProvider)
        {
            if (mapper == null) throw new NullReferenceException("IMapperAdapter not initialized");
            _mapper = mapper;
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            var beginExecute = base.BeginExecute(requestContext, callback, state);
            ViewBag.Role = GetRole();
            _user = new AspNetUser()
            {
                Id = _userProvider.GetUserId()
            };
            return beginExecute;
        }

        public string GetRole()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                //_userManager.IsInRoleAsync(user.Id,RoleType.Employee.ToString());
                var role = _userProvider.GetRoles().First();
                return role;
            }
            else return string.Empty;
        }

    }
}