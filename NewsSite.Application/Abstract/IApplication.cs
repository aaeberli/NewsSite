using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Application.Abstract
{
    public interface INewsService<TRule> : IApplicationServiceWithRules<TRule>, IDisposable where TRule : class
    {
        bool AutoSave { get; set; }

        IEnumerable<Article> GetArticlesList(AspNetUser user);
        Article GetSingleArticle(AspNetUser user, Article article);
        Like AddLike(AspNetUser user, Article article);
        Article AddArticle(AspNetUser user, Article article);
        Like RemoveLike(AspNetUser user, Article article, Like like);
        Article RemoveArticle(AspNetUser user, Article article);
        Article EditArticle(AspNetUser user, Article article);
        IEnumerable<ArticlesStats> GetTopTenArticles();
    }
}
