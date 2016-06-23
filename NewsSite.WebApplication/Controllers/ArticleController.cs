using AutoMapper;
using Microsoft.AspNet.Identity;
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
    /// <summary>
    /// Controller containing all the logic to manage articles
    /// </summary>
    public class ArticleController : BaseControllerWithRole
    {
        private INewsService<ApplicationRule> _newsService;

        public ArticleController(INewsService<ApplicationRule> newsService, IMapperAdapter mapper, ISolutionLogger logger, IUserProvider userProvider)
            : base(mapper, logger, userProvider)
        {
            if (newsService == null) throw new NullReferenceException("INewsService not initialized");
            _newsService = newsService;
        }

        [HttpGet]
        [Authorize]
        public ViewResult ArticleList()
        {
            IEnumerable<Article> articleList = _newsService.GetArticlesList(_user);
            IEnumerable<ArticleViewModel> articleVM = _mapper.MapCollection<Article, ArticleViewModel>(articleList)?.AddCurrentUserInfo(_user);
            return View("ArticleList", articleVM);
        }

        [HttpGet]
        [Authorize(Roles = "Publisher")]
        public ViewResult PersonalArticleList()
        {
            IEnumerable<Article> articleList = _newsService.GetArticlesList(_user)?.Where(a => a.AuthorId == _user.Id);
            IEnumerable<ArticleViewModel> articleVM = _mapper.MapCollection<Article, ArticleViewModel>(articleList)?.AddCurrentUserInfo(_user);
            return View("ArticleList", articleVM);
        }

        [HttpGet]
        [Authorize(Roles = "Publisher")]
        public ViewResult EditArticle(int? articleId)
        {
            if (articleId != null)
            {
                ViewBag.ArticleAction = "EditArticle";
                ViewBag.Title = "Edit";
                Article article = _newsService.GetSingleArticle(_user, new Article { Id = (int)articleId });
                if (article == null) return View("Error");
                else
                {
                    ArticleViewModel articleVM = _mapper.Map<Article, ArticleViewModel>(article).AddCurrentUserInfo(_user);
                    return View("CreateArticle", articleVM);
                }
            }
            else return View("Error");
        }

        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public ActionResult EditArticle(ArticleViewModel inputArticle)
        {
            ViewBag.ArticleAction = "EditArticle";
            ViewBag.Title = "Edit";
            if (ModelState.IsValid)
            {
                Article mappedArticle = _mapper.Map<ArticleViewModel, Article>(inputArticle);
                Article createdArticle = _newsService.EditArticle(_user, mappedArticle);
                if (createdArticle == null) return View("Error");
                else return Redirect("ArticleList");
            }
            else return View("CreateArticle");
        }

        [HttpGet]
        [Authorize(Roles = "Publisher")]
        public ViewResult CreateArticle()
        {
            ViewBag.ArticleAction = "CreateArticle";
            ViewBag.Title = "Create";
            return View("CreateArticle");
        }

        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public ActionResult CreateArticle([Bind(Exclude = "Id")] ArticleViewModel inputArticle)
        {
            ViewBag.ArticleAction = "CreateArticle";
            ViewBag.Title = "Create";
            if (ModelState.IsValid)
            {
                Article mappedArticle = _mapper.Map<ArticleViewModel, Article>(inputArticle);
                Article createdArticle = _newsService.AddArticle(_user, mappedArticle);
                if (createdArticle == null) return View("Error");
                else return Redirect("ArticleList");
            }
            else return View("CreateArticle");
        }

        [HttpGet]
        [Authorize]
        public ViewResult ViewArticle(int? articleId)
        {
            if (articleId != null)
            {
                Article article = _newsService.GetSingleArticle(_user, new Article { Id = (int)articleId });
                if (article == null) return View("Error");
                else
                {
                    ArticleViewModel articleVM = _mapper.Map<Article, ArticleViewModel>(article).AddCurrentUserInfo(_user);
                    return View("ViewArticle", articleVM);
                }
            }
            else return View("Error");
        }

        [HttpGet]
        [Authorize]
        public ActionResult LikeArticle(int? articleId)
        {
            ActionResult result = Redirect($"ViewArticle?articleId={articleId}");
            if (articleId != null)
            {
                Like addedLike = _newsService.AddLike(_user, new Article { Id = (int)articleId });
                if (addedLike == null)
                {
                    if (_newsService.ApplicationRules.Count(r => !r.Result && r.Reason == ReasonEnum.MaxLikes) != 0) ViewBag.MaxLike = true;
                    else result = View("Error");
                }

            }
            else result = View("Error");

            return result;
        }

        [HttpGet]
        [Authorize]
        public ActionResult UnlikeArticle(int? articleId, int? likeId)
        {
            ActionResult result = Redirect($"ViewArticle?articleId={articleId}");
            if (articleId != null && likeId != null)
            {
                Like removedLike = _newsService.RemoveLike(_user, new Article { Id = (int)articleId }, new Like { Id = (int)likeId });
                if (removedLike == null) result = View("Error");
            }
            else result = View("Error");

            return result;
        }

        [HttpGet]
        [Authorize]
        public ActionResult RemoveArticle(int? articleId)
        {
            if (articleId != null)
            {
                Article article = _newsService.RemoveArticle(_user, new Article { Id = (int)articleId });
                if (article == null) return View("Error");
                return Redirect("ArticleList");
            }
            else return View("Error");
        }
    }
}