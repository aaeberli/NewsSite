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

        IEnumerable<News> GetNewsList(AspNetUser user);
        News GetSingleNews(AspNetUser user, News news);
        Like AddLike(AspNetUser user, News news);
        News AddNews(AspNetUser user, News news);
        Like RemoveLike(AspNetUser user, News news, Like like);
        News RemoveNews(AspNetUser user, News news);
        News EditNews(AspNetUser user, News news);
        IEnumerable<NewsStats> GetTopTenNews();
    }
}
