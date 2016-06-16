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

        public bool AutoSave { get; set; }

        public IList<ApplicationRule> ApplicationRules { get; private set; }

        public void AddRule(ApplicationRule applicationRule)
        {
            if (ApplicationRules == null) ApplicationRules = new List<ApplicationRule>();
            ApplicationRules.Add(applicationRule);
        }

        public void ResetRules()
        {
            ApplicationRules = null;
        }

        public bool GetRulesStatus()
        {
            return ApplicationRules.Count(r => !r.Result) == 0;
        }


        private T Decorator<T>(Func<T> action, bool save = true)
        {
            ResetRules();
            T result = action.Invoke();
            if (save && AutoSave && GetRulesStatus()) SaveChanges();
            return result;
        }

        public Like AddLike(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, _newsRepo.SingleOrDefault(n => n.Id == news.Id) != null, ReasonEnum.NoNews);
                int likesPerUser = _likeRepo.Read(l => l.UserId == user.Id).Count();
                ApplicationRule c = new ApplicationRule(this, likesPerUser < Settings.Default.MaxLikes, ReasonEnum.MaxLikes);
                ApplicationRule d = new ApplicationRule(this, _likeRepo.SingleOrDefault(l => l.UserId == user.Id && l.NewsId == news.Id) == null, ReasonEnum.AlreadyLiked);

                if (a & b & c & d)
                {
                    Like like = _likeRepo.Create();
                    like.UserId = user.Id;
                    like.NewsId = news.Id;
                    like.CreatedDate = DateTime.Now;
                    _likeRepo.Add(like);
                    return like;
                }
                else return null;
            });
        }

        public Like RemoveLike(AspNetUser user, News news, Like like)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, _newsRepo.SingleOrDefault(n => n.Id == news.Id) != null, ReasonEnum.NoNews);
                Like foundLike = _likeRepo.SingleOrDefault(l => l.Id == like.Id);
                ApplicationRule c = new ApplicationRule(this, foundLike != null, ReasonEnum.NoLike);
                ApplicationRule d = new ApplicationRule(this, foundLike?.UserId == user.Id, ReasonEnum.WrongUser);
                ApplicationRule e = new ApplicationRule(this, foundLike?.NewsId == news.Id, ReasonEnum.WrongNews);

                if (a & b & c & d & e)
                {
                    Like removedLike = _likeRepo.Remove(foundLike);
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
            }, false);
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
            }, false);
        }

        public News AddNews(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundUser.AspNetRoles.SingleOrDefault(r => r.Name == RoleType.Publisher.ToString()) != null, ReasonEnum.NoPublisher);
                ApplicationRule c = new ApplicationRule(this, !news.Title.IsNullOrEmptyOrWhiteSpace(), ReasonEnum.EmptyTitle);
                ApplicationRule d = new ApplicationRule(this, !news.Body.IsNullOrEmptyOrWhiteSpace(), ReasonEnum.EmptyBody);

                if (a & b & c & d)
                {
                    News createdNews = _newsRepo.Create();
                    createdNews.Title = news.Title;
                    createdNews.Body = news.Body;
                    createdNews.AuthorId = foundUser.Id;
                    createdNews.CreatedDate = DateTime.Now;
                    _newsRepo.Add(createdNews);
                    return createdNews;
                }
                else return null;
            });
        }

        public News RemoveNews(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                News foundNews = _newsRepo.SingleOrDefault(n => n.Id == news.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundNews != null, ReasonEnum.NoNews);
                ApplicationRule c = new ApplicationRule(this, foundNews.AspNetUser.Id == user.Id, ReasonEnum.WrongUser);

                if (a & b & c)
                {
                    News removedNews = _newsRepo.Remove(foundNews);
                    return removedNews;
                }
                else return null;
            });
        }

        public News EditNews(AspNetUser user, News news)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                News foundNews = _newsRepo.SingleOrDefault(n => n.Id == news.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundNews != null, ReasonEnum.NoNews);
                ApplicationRule c = new ApplicationRule(this, foundNews.AspNetUser.Id == user.Id, ReasonEnum.WrongUser);

                if (a & b & c)
                {
                    foundNews.Title = news.Title.IsNullOrEmptyOrWhiteSpace() ? foundNews.Title : news.Title;
                    foundNews.Body = news.Body.IsNullOrEmptyOrWhiteSpace() ? foundNews.Body : news.Body;
                    foundNews.UpdatedDate = DateTime.Now;
                    return foundNews;
                }
                else return null;
            });
        }

        public IEnumerable<NewsStats> GetTopTenNews()
        {
            return Decorator(() =>
            {
                IEnumerable<News> foundNewses = _newsRepo.Read();
                IEnumerable<Like> foundLikes = _likeRepo.Read();
                var newsView = foundNewses.Select(n => new NewsStats() { Id = n.Id, Title = n.Title, Likes = n.Likes.Count() });

                return newsView.OrderByDescending(n => n.Likes).Take(10);
            }, false);
        }

        public void Dispose()
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            // it's enough for one repo to act on dbContext and save changes
            _newsRepo.SaveChanges();
        }
    }
}
