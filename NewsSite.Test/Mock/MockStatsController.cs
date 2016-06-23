using NewsSite.Application.Abstract;
using NewsSite.Common;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using NewsSite.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace NewsSite.Test.Mock
{
    internal class MockStatsController : StatsController
    {
        public MockStatsController(INewsService<ApplicationRule> newsService, IMapperAdapter mapper, ISolutionLogger logger, IUserProvider userProvider)
            : base(newsService, mapper, logger, userProvider)
        {

        }

        public void BeginExecute()
        {
            BeginExecute(null, null, null);
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            _user = new AspNetUser()
            {
                Id = _userProvider.GetUserId()
            };

            return null;
        }
    }
}
