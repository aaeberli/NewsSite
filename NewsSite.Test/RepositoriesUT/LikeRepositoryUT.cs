using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using NewsSite.DataAccess;
using NewsSite.DataAccess.Repositories;

namespace NewsSite.Test
{
    using Abstract;
    using ConnStringWrappers;
    using System.Linq;
    using static DbUtils;

    [TestClass]
    public class LikeRepositoryUT
    {
        private UnityContainer container;
        private IRepository<Like> repo;
        private ITestUtils<string> utils;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IConnStringWrapper, NewsSiteDBWrapper>()
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>(new PerThreadLifetimeManager())
                .RegisterType<IRepository<Like>, LikeRepository>()
                .RegisterType<ITestUtils<string>, DbUtils>();

            utils = container.Resolve<ITestUtils<string>>();

            repo = container.Resolve<IRepository<Like>>();
        }

        [TestMethod]
        public void Test_add_like_to_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Like like = repo.Create();
            like.CreatedDate = DateTime.Now;
            like.UserId = userIds.Item1;
            like.ArticleId = articleId;

            // Act
            repo.Add(like);
            repo.SaveChanges();
            var articles = adapter.GetEntities<Article>();

            // Assert
            Assert.AreEqual(1, articles.Single(n => n.Id == articleId).Likes.Count());
            Assert.AreEqual(userIds.Item1, articles.Single(n => n.Id == articleId).Likes.ElementAt(0).AspNetUser.Id);

        }


        [TestMethod]
        public void Test_remove_like_from_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, articleId);
            Like like = repo.SingleOrDefault(l => l.Id == likeId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();

            // Act
            repo.Remove(like);
            repo.SaveChanges();
            var articles = adapter.GetEntities<Article>().Single(n => n.Id == articleId);

            // Assert
            Assert.AreEqual(0, articles.Likes.Count());

        }
    }
}
