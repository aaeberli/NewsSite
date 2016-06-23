using NewsSite.Application.Abstract;
using NewsSite.Application.Properties;
using NewsSite.Common;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Application
{

    /// <summary>
    /// Application Service
    /// Implements INewsService and IApplicationServiceWithRules (see respective documentation)
    /// </summary>
    public class NewsService : INewsService<ApplicationRule>, IApplicationServiceWithRules<ApplicationRule>
    {
        IRepository<Article> _articleRepo;
        IRepository<AspNetUser> _userRepo;
        IRepository<Like> _likeRepo;
        ISolutionLogger _logger;

        public NewsService(IRepository<Article> articleRepo, IRepository<AspNetUser> userRepo, IRepository<Like> likeRepo, ISolutionLogger logger)
        {
            if (articleRepo == null) throw new NullReferenceException("IRepository<Article> must be initialized");
            if (userRepo == null) throw new NullReferenceException("IRepository<AspNetUser> must be initialized");
            if (likeRepo == null) throw new NullReferenceException("IRepository<Like> must be initialized");
            if (logger == null) throw new NullReferenceException("ILogger must be initialized");
            _articleRepo = articleRepo;
            _userRepo = userRepo;
            _likeRepo = likeRepo;
            _logger = logger;

            AutoSave = true;
        }

        public bool AutoSave { get; set; }

        private IList<ApplicationRule> _ApplicationRules;
        public IList<ApplicationRule> ApplicationRules
        {
            get
            {
                if (_ApplicationRules == null) _ApplicationRules = new List<ApplicationRule>();
                return _ApplicationRules;
            }
            private set
            {
                _ApplicationRules = value;
            }
        }

        public void AddRule(ApplicationRule applicationRule)
        {
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

        /// <summary>
        /// Decorates each method to perform common tasks
        /// </summary>
        /// <typeparam name="T">Result type of the decorated function</typeparam>
        /// <param name="action">A lambda expression representing the business logic</param>
        /// <param name="save">Specifies if SaveChanges must be performed each time</param>
        /// <returns>The object returned by the decorated function</returns>

        private T Decorator<T>(Func<T> action, bool save = true)
        {
            try
            {
                ResetRules();
                T result = action.Invoke();
                bool status = GetRulesStatus();
                if (save && AutoSave && status) SaveChanges();
                if (!status)
                {
                    StackTrace stackTrace = new StackTrace();
                    string methodName = stackTrace.GetFrame(1).GetMethod().Name;
                    foreach (var rule in ApplicationRules.Where(r => !r.Result))
                    {
                        _logger.LogInfo(rule.Reason.ToString(), methodName);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                StackTrace stackTrace = new StackTrace();
                string methodName = stackTrace.GetFrame(1).GetMethod().Name;
                _logger.LogError(ex, methodName);
                return default(T);
            }
        }

        public Like AddLike(AspNetUser user, Article article)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, _articleRepo.SingleOrDefault(art => art.Id == article.Id) != null, ReasonEnum.NoArticle);
                int likesPerUser = _likeRepo.Read(l => l.UserId == user.Id).Count();
                ApplicationRule c = new ApplicationRule(this, likesPerUser < Settings.Default.MaxLikes, ReasonEnum.MaxLikes);
                ApplicationRule d = new ApplicationRule(this, _likeRepo.SingleOrDefault(l => l.UserId == user.Id && l.ArticleId == article.Id) == null, ReasonEnum.AlreadyLiked);

                if (a & b & c & d)
                {
                    Like like = _likeRepo.Create();
                    like.UserId = user.Id;
                    like.ArticleId = article.Id;
                    like.CreatedDate = DateTime.Now;
                    _likeRepo.Add(like);
                    return like;
                }
                else return null;
            });
        }

        public Like RemoveLike(AspNetUser user, Article article, Like like)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, _articleRepo.SingleOrDefault(art => art.Id == article.Id) != null, ReasonEnum.NoArticle);
                Like foundLike = _likeRepo.SingleOrDefault(l => l.Id == like.Id);
                ApplicationRule c = new ApplicationRule(this, foundLike != null, ReasonEnum.NoLike);
                ApplicationRule d = new ApplicationRule(this, foundLike?.UserId == user.Id, ReasonEnum.WrongUser);
                ApplicationRule e = new ApplicationRule(this, foundLike?.ArticleId == article.Id, ReasonEnum.WrongArticle);

                if (a & b & c & d & e)
                {
                    Like removedLike = _likeRepo.Remove(foundLike);
                    return like;
                }
                else return null;
            });
        }

        public IEnumerable<Article> GetArticlesList(AspNetUser user)
        {
            return Decorator(() =>
            {
                // Verify user exists
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);

                if (a)
                {
                    return _articleRepo.Read();
                }
                else return null;
            }, false);
        }

        public Article GetSingleArticle(AspNetUser user, Article article)
        {
            return Decorator(() =>
            {
                ApplicationRule a = new ApplicationRule(this, _userRepo.SingleOrDefault(u => u.Id == user.Id) != null, ReasonEnum.NoUSer);
                if (_userRepo.SingleOrDefault(u => u.Id == user.Id) != null)
                {
                    return _articleRepo.SingleOrDefault(n => n.Id == article.Id);
                }
                else return null;
            }, false);
        }

        public Article AddArticle(AspNetUser user, Article article)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundUser.AspNetRoles.SingleOrDefault(r => r.Name == RoleType.Publisher.ToString()) != null, ReasonEnum.NoPublisher);
                ApplicationRule c = new ApplicationRule(this, !article.Title.IsNullOrEmptyOrWhiteSpace(), ReasonEnum.EmptyTitle);
                ApplicationRule d = new ApplicationRule(this, !article.Body.IsNullOrEmptyOrWhiteSpace(), ReasonEnum.EmptyBody);
                ApplicationRule e = new ApplicationRule(this, article.Title?.Length <= 50, ReasonEnum.TitleTooLong);

                if (a & b & c & d & e)
                {
                    Article createdArticle = _articleRepo.Create();
                    createdArticle.Title = article.Title;
                    createdArticle.Body = article.Body;
                    createdArticle.AuthorId = foundUser.Id;
                    createdArticle.CreatedDate = DateTime.Now;
                    _articleRepo.Add(createdArticle);
                    return createdArticle;
                }
                else return null;
            });
        }

        public Article RemoveArticle(AspNetUser user, Article article)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                Article foundArticle = _articleRepo.SingleOrDefault(art => art.Id == article.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundArticle != null, ReasonEnum.NoArticle);
                ApplicationRule c = new ApplicationRule(this, foundArticle.AspNetUser.Id == user.Id, ReasonEnum.WrongUser);

                if (a & b & c)
                {
                    Article removedArticle = _articleRepo.Remove(foundArticle);
                    return removedArticle;
                }
                else return null;
            });
        }

        public Article EditArticle(AspNetUser user, Article article)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                Article foundArticle = _articleRepo.SingleOrDefault(art => art.Id == article.Id);
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundArticle != null, ReasonEnum.NoArticle);
                ApplicationRule c = new ApplicationRule(this, foundArticle.AspNetUser.Id == user.Id, ReasonEnum.WrongUser);

                if (a & b & c)
                {
                    foundArticle.Title = article.Title.IsNullOrEmptyOrWhiteSpace() ? foundArticle.Title : article.Title;
                    foundArticle.Body = article.Body.IsNullOrEmptyOrWhiteSpace() ? foundArticle.Body : article.Body;
                    foundArticle.UpdatedDate = DateTime.Now;
                    return foundArticle;
                }
                else return null;
            });
        }

        public IEnumerable<ArticlesStats> GetTopTenArticles(AspNetUser user)
        {
            return Decorator(() =>
            {
                AspNetUser foundUser = _userRepo.SingleOrDefault(u => u.Id == user.Id);
                IEnumerable<Article> foundArticles = _articleRepo.Read();
                IEnumerable<Like> foundLikes = _likeRepo.Read();
                ApplicationRule a = new ApplicationRule(this, foundUser != null, ReasonEnum.NoUSer);
                ApplicationRule b = new ApplicationRule(this, foundUser.AspNetRoles.SingleOrDefault(r => r.Name == RoleType.Publisher.ToString()) != null, ReasonEnum.NoPublisher);

                if (a & b)
                {
                    var artView = foundArticles.Select(art => new ArticlesStats() { Id = art.Id, Title = art.Title, Likes = art.Likes.Count() });
                    return artView.OrderByDescending(art => art.Likes).Take(Settings.Default.MaxLikes);
                }
                else return null;
            }, false);
        }

        public void Dispose()
        {
            if (AutoSave) SaveChanges();
        }

        private void SaveChanges()
        {
            _articleRepo.SaveChanges();
            _likeRepo.SaveChanges();
            _userRepo.SaveChanges();
        }
    }
}
