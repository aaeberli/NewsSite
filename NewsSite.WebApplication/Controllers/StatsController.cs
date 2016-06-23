using NewsSite.Application.Abstract;
using NewsSite.Common;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using NewsSite.WebApplication.Abstract;
using NewsSite.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsSite.WebApplication.Controllers
{
    public class StatsController : BaseControllerWithRole
    {
        private INewsService<ApplicationRule> _newsService;

        public StatsController(INewsService<ApplicationRule> newsService, IMapperAdapter mapper, ISolutionLogger logger, IUserProvider userProvider)
            : base(mapper, logger, userProvider)
        {
            if (newsService == null) throw new NullReferenceException("INewsService not initialized");
            _newsService = newsService;
        }

        [HttpGet]
        [Authorize(Roles = "Publisher")]
        public ViewResult GetStats()
        {
            IEnumerable<ArticlesStats> articleList = _newsService.GetTopTenArticles(_user);
            if (articleList != null)
            {
                IEnumerable<ArticlesStatsViewModel> articleVM = _mapper.MapCollection<ArticlesStats, ArticlesStatsViewModel>(articleList);
                return View("GetStats", articleVM);
            }
            else return View("Error");
        }
    }
}