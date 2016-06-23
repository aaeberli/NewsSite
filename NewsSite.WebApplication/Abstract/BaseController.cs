using Microsoft.AspNet.Identity.Owin;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Web;
using System.Web.Mvc;

namespace NewsSite.WebApplication.Abstract
{
    /// <summary>
    /// Base class for controller including Logger and Authentication facilites
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;
        protected ApplicationRoleManager _roleManager;
        protected AspNetUser _user;
        protected IMapperAdapter _mapper;
        protected ISolutionLogger _logger;
        protected IUserProvider _userProvider;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext()?.Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext?.GetOwinContext()?.GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext?.GetOwinContext()?.Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public ISolutionLogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public BaseController(IMapperAdapter mapper, ISolutionLogger logger, IUserProvider userProvider)
        {
            if (mapper == null) throw new NullReferenceException("IMapperAdapter not initialized");
            if (logger == null) throw new NullReferenceException("ISolutionLogger not initialized");
            if (userProvider == null) throw new NullReferenceException("IUserProvider not initialized");
            _mapper = mapper;
            _logger = logger;
            _userProvider = userProvider;
            _userProvider.Register(User, UserManager);
        }

    }
}