using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Application.Abstract
{
    public interface INewsService : IDisposable
    {
        bool AutoSave { get; set; }

        IEnumerable<Article> GetArticlesList(AspNetUser user);
        Article GetSingleArticle(AspNetUser user, Article news);
        Like AddLike(AspNetUser user, Article news);
        Article AddArticle(AspNetUser user, Article news);
        Like RemoveLike(AspNetUser user, Article news, Like like);
        Article RemoveArticle(AspNetUser user, Article news);
        Article EditArticle(AspNetUser user, Article news);
        IEnumerable<ArticlesStats> GetTopTenArticles();
    }
}
