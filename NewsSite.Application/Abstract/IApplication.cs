using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Application.Abstract
{
    /// <summary>
    /// Defines Business Logic Structure
    /// </summary>
    /// <typeparam name="TRule"></typeparam>
    public interface INewsService<TRule> : IApplicationServiceWithRules<TRule>, IDisposable where TRule : class
    {
        /// <summary>
        /// Sets auto SaveChanges() behaviour for every operation
        /// </summary>
        bool AutoSave { get; set; }

        /// <summary>
        /// Gets the full list of articles
        /// </summary>
        /// <param name="user">Current user (is not used as filter)</param>
        /// <returns>Full list of articles</returns>
        IEnumerable<Article> GetArticlesList(AspNetUser user);

        /// <summary>
        /// Gets a single article from by Id
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="article">An Article object (only Id must be set)</param>
        /// <returns>Single article</returns>
        Article GetSingleArticle(AspNetUser user, Article article);

        /// <summary>
        /// Adds a Like to an article
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="article">An Article object (only Id must be set)</param>
        /// <returns>The added Like</returns>
        Like AddLike(AspNetUser user, Article article);

        /// <summary>
        /// Adds an article
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="article">The article to add</param>
        /// <returns>The added article</returns>
        Article AddArticle(AspNetUser user, Article article);

        /// <summary>
        /// Removes a like from an article
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="article">An Article object (only Id must be set)</param>
        /// <param name="like">A Like object (only Id must be set)</param>
        /// <returns>The removed Like</returns>
        Like RemoveLike(AspNetUser user, Article article, Like like);

        /// <summary>
        /// Removes an article
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="article">An Article object (only Id must be set)</param>
        /// <returns>The removed article</returns>
        Article RemoveArticle(AspNetUser user, Article article);

        /// <summary>
        /// Updates an article
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="article">The article to modify</param>
        /// <returns>The updated article</returns>
        Article EditArticle(AspNetUser user, Article article);


        /// <summary>
        /// Gets top ten articles by number of likes
        /// </summary>
        /// <param name="user">Current user</param>
        /// <returns>an ad-hoc object with aggregated information</returns>
        IEnumerable<ArticlesStats> GetTopTenArticles(AspNetUser user);
    }
}
