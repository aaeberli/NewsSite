using NewsSite.Application.Abstract;
using NewsSite.Application.Properties;
using NewsSite.Common;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Application
{
    // TODO: use postsharp for error handling
    public class NewsService : INewsService, IApplicationServiceWithRules<ApplicationRule>
    {
        IRepository<News> _newsRepo;
        IRepository<AspNetUser> _userRepo;
        IRepository<Like> _likeRepo;

        public NewsService(IRepository<News> newsRepo, IRepository<AspNetUser> userRepo, IRepository<Like> likeRepo)
        {
            if (newsRepo == null) throw new NullReferenceException("IRepository<News> must be initialized");
            if (userRepo == null) throw new NullReferenceException("IRepository<AspNetUser> must be initialized");
            if (likeRepo == null) throw new NullReferenceException("IRepository<Like> must be initialized");
            _newsRepo = newsRepo;
            _userRepo = userRepo;
            _likeRepo = likeRepo;
        }


        public IList<ApplicationRule> ApplicationRules { get; private set; }

        private T Decorator<T>(Func<T> action)
        {
            ResetRules();
            return action.Invoke();
        }

        public Like AddLike(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, _newsRepo.SingleOrDefault(n => n.Id == news.Id) != null, ReasonEnum.NoNews);
                int likesPerUser = _likeRepo.Read(l => l.UserId == user.Id).Count();
                ApplicationRule c = new ApplicationRule(this, likesPerUser < Settings.Default.MaxLikes, ReasonEnum.MaxLikes);

                if (a & b & c)
                {
                    Like like = _likeRepo.Create();
                    like.UserId = user.Id;
                    like.NewsId = news.Id;
                    like.CreatedDate = DateTime.Now;
                    _likeRepo.Add(like);
                    _likeRepo.SaveChanges();
                    return like;
                }
                else return null;
            });
        }

        public IEnumerable<News> GetNewsList(AspNetUser user)
        {
            return Decorator(() =>
            {
                // Verify user exists
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);

                if (a)
                {
                    return _newsRepo.Read();
                }
                else return null;
            });
        }

        public News GetSingleNews(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                if (_userRepo.SingleOrDefault(u => u.Id == user.Id) != null)
                {
                    return _newsRepo.SingleOrDefault(n => n.Id == news.Id);
                }
                else return null;
            });
        }

        public void AddRule(ApplicationRule applicationRule)
        {
            if (ApplicationRules == null) ApplicationRules = new List<ApplicationRule>();
            ApplicationRules.Add(applicationRule);
        }

        public void ResetRules()
        {
            ApplicationRules = null;
        }

        public News AddNews(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundUser.AspNetRoles.SingleOrDefault(r => r.Name == RoleType.Publisher.ToString()) != null, ReasonEnum.NoPublisher);
                ApplicationRule c = new ApplicationRule(this, !news.Title.IsNullOrEmptyOrWhiteSpace(), ReasonEnum.EmptyTitle);
                ApplicationRule d = new ApplicationRule(this, !news.Body.IsNullOrEmptyOrWhiteSpace(), ReasonEnum.EmptyTitle);

                if (a & b & c & d)
                {
                    News createdNews = _newsRepo.Create();
                    createdNews.Title = news.Title;
                    createdNews.Body = news.Body;
                    createdNews.AuthorId = foundUser.Id;
                    createdNews.CreatedDate = DateTime.Now;
                    _newsRepo.Add(createdNews);
                    _likeRepo.SaveChanges();
                    return createdNews;
                }
                else return null;
            });
        }
    }
}
