using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using NewsSite.DataAccess;
using NewsSite.DataAccess.Repositories;

namespace NewsSite.Test
{
    using System.Linq;
    using static Utils;

    [TestClass]
    public class LikeRepositoryUT
    {
        private UnityContainer container;
        private IRepository<Like> repo;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>(new PerThreadLifetimeManager())
                .RegisterType<IRepository<Like>, LikeRepository>();

            repo = container.Resolve<IRepository<Like>>();
        }

        [TestMethod]
        public void Test_add_like_to_news()
        {
            // Arrange
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            int newsId = CreateSingleNews(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Like like = repo.Create();
            like.CreatedDate = DateTime.Now;
            like.UserId = userIds.Item1;
            like.NewsId = newsId;

            // Act
            repo.Add(like);
            repo.SaveChanges();
            var newses = adapter.GetEntities<News>();

            // Assert
            Assert.AreEqual(1, newses.Single(n => n.Id == newsId).Likes.Count());
            Assert.AreEqual(userIds.Item1, newses.Single(n => n.Id == newsId).Likes.ElementAt(0).User.Id);

        }


        [TestMethod]
        public void Test_remove_like_from_news()
        {
            // Arrange
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            int newsId = CreateSingleNews(userIds.Item2);
            int likeId = AddLike(userIds.Item1, newsId);
            Like like = repo.SingleOrDefault(l => l.Id == likeId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();

            // Act
            repo.Remove(like);
            repo.SaveChanges();
            var newses = adapter.GetEntities<News>().Single(n => n.Id == newsId);

            // Assert
            Assert.AreEqual(0, newses.Likes.Count());

        }
    }
}
