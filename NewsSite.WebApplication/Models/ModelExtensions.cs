using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsSite.WebApplication.Models
{
    /// <summary>
    /// Static class extending ViewModels
    /// </summary>
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

        /// <summary>
        /// Extends a collection of ArticleViewModel merging information from current user, with a fluent syntax
        /// </summary>
        /// <param name="articleVM">Collection of ViewModels</param>
        /// <param name="user">Current user</param>
        /// <returns>The enriched collection fo ViewModel</returns>
        public static IEnumerable<ArticleViewModel> AddCurrentUserInfo(this IEnumerable<ArticleViewModel> articleVM, AspNetUser user)
        {
            return articleVM.Select(a =>
            {
                a = _AddCurrentUserInfo(a, user);
                return a;
            });
        }

        /// <summary>
        /// Extends ArticleViewModel merging information from current user, with a fluent syntax
        /// </summary>
        /// <param name="articleVM">The ViewModel</param>
        /// <param name="user">Current user</param>
        /// <returns>The enriched ViewModel</returns>
        public static ArticleViewModel AddCurrentUserInfo(this ArticleViewModel articleVM, AspNetUser user)
        {
            return _AddCurrentUserInfo(articleVM, user);
        }
    }
}