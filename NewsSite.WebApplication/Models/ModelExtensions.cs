using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsSite.WebApplication.Models
{
    public static class ModelExtensions
    {
        private static ArticleViewModel _AddCurrentUserInfo(ArticleViewModel articleVM, AspNetUser user)
        {
            bool hasCurrentUserLike = articleVM.Likers.Count(kvp => kvp.Key == user.Id) > 0;
            if (hasCurrentUserLike)
            {
                articleVM.CurrentUserLike = true;
                articleVM.CurrentUserLikeId = articleVM.Likers.Single(kvp => kvp.Key == user.Id).Value;
            }
            articleVM.CurrentUserId = user.Id;
            return articleVM;
        }

        public static IEnumerable<ArticleViewModel> AddCurrentUserInfo(this IEnumerable<ArticleViewModel> articleVM, AspNetUser user)
        {
            return articleVM.Select(a =>
            {
                a = _AddCurrentUserInfo(a, user);
                return a;
            });
        }

        public static ArticleViewModel AddCurrentUserInfo(this ArticleViewModel articleVM, AspNetUser user)
        {
            return _AddCurrentUserInfo(articleVM, user);
        }
    }
}