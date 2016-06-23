using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using NewsSite.Common;
using NewsSite.Common.Abstract;
using NewsSite.WebApplication.Abstract;
using NewsSite.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NewsSite.WebApplication.Controllers
{
    [Authorize]
    public class HomeController : BaseControllerWithRole
    {
        public HomeController(IMapperAdapter mapper, ISolutionLogger logger, IUserProvider userProvider)
            : base(mapper, logger, userProvider)
        {

        }
        public ActionResult Index()
        {
            return View();
        }
    }
}
