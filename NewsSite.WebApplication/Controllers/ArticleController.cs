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
    public class ArticleController : BaseControllerWithRole
    {
        #region Utils
        private IEnumerable<ArticleViewModel> GetCurrentUserLike(IEnumerable<ArticleViewModel> articleVM)
        {
            return articleVM.Select(a => { a.CurrentUserLike = a.Likers.Contains(_user.Id); return a; });
        }

        private ArticleViewModel GetCurrentUserLike(ArticleViewModel articleVM)
        {
            articleVM.CurrentUserLike = articleVM.Likers.Contains(_user.Id);
            return articleVM;
        }
        #endregion

        private INewsService _newsService;

        public ArticleController(INewsService newsService, IMapperAdapter mapper)
            : base(mapper)
        {
            if (newsService == null) throw new NullReferenceException("INewsService not initialized");
            _newsService = newsService;
        }

        [Authorize]
        public ActionResult Index()
        {
            IEnumerable<Article> articleList = _newsService.GetArticlesList(_user);
            IEnumerable<ArticleViewModel> articleVM = _mapper.MapCollection<Article, ArticleViewModel>(articleList);
            articleVM = GetCurrentUserLike(articleVM);
            return View("ArticleList", articleVM);
        }

        [Authorize(Roles = "Publisher")]
        public ActionResult Personal()
        {
            IEnumerable<Article> articleList = _newsService.GetArticlesList(_user).Where(a => a.AuthorId == _user.Id);
            IEnumerable<ArticleViewModel> articleVM = _mapper.MapCollection<Article, ArticleViewModel>(articleList);
            return View("ArticleList", articleList);
        }

        [HttpGet]
        [Authorize(Roles = "Publisher")]
        public ActionResult EditArticle(int? articleId)
        {
            if (articleId != null)
            {
                ViewBag.ArticleAction = "EditArticle";
                ViewBag.ActionDesc = "Edit";
                Article article = _newsService.GetSingleArticle(_user, new Article { Id = (int)articleId });
                ArticleViewModel articleVM = _mapper.Map<Article, ArticleViewModel>(article);
                articleVM = GetCurrentUserLike(articleVM);
                return View("CreateArticle", articleVM);
            }
            else return View("Error");
        }

        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public ActionResult EditArticle(ArticleViewModel inputArticle)
        {
            ViewBag.ArticleAction = "EditArticle";
            ViewBag.ActionDesc = "Edit";
            if (ModelState.IsValid)
            {
                Article mappedArticle = _mapper.Map<ArticleViewModel, Article>(inputArticle);
                Article createdArticle = _newsService.EditArticle(_user, mappedArticle);
                if (createdArticle == null) return View("Error");
                else return Redirect("Index");
            }
            else return View("CreateArticle");
        }

        [Authorize(Roles = "Publisher")]
        public ActionResult CreateArticle()
        {
            ViewBag.ArticleAction = "CreateArticle";
            ViewBag.ActionDesc = "Create";
            return View("CreateArticle");
        }

        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public ActionResult CreateArticle([Bind(Exclude = "Id")] ArticleViewModel inputArticle)
        {
            ViewBag.ArticleAction = "CreateArticle";
            ViewBag.ActionDesc = "Create";
            if (ModelState.IsValid)
            {
                Article mappedArticle = _mapper.Map<ArticleViewModel, Article>(inputArticle);
                Article createdArticle = _newsService.AddArticle(_user, mappedArticle);
                if (createdArticle == null) return View("Error");
                else return Redirect("Index");
            }
            else return View();
        }

        [Authorize]
        public ActionResult ViewArticle(int? articleId)
        {
            if (articleId != null)
            {
                Article article = _newsService.GetSingleArticle(_user, new Article { Id = (int)articleId });
                ArticleViewModel articleVM = _mapper.Map<Article, ArticleViewModel>(article);
                articleVM = GetCurrentUserLike(articleVM);
                return View("ViewArticle", articleVM);
            }
            else return View("Error");
        }
    }
}