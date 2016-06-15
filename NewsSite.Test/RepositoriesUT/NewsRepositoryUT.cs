using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsSite.Test
{
    using ConnStringWrappers;
    using NewsSite.Common.Abstract;
    using NewsSite.DataAccess;
    using NewsSite.DataAccess.Repositories;
    using NewsSite.Domain.Model;
    using System.Data;
    using static Utils;

    [TestClass]
    public class NewsRepositoryUT
    {
        private UnityContainer container;
        private IRepository<News> repo;
        private IRepository<AspNetUser> userRepo;
        private ITestUtils<string> utils;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IConnStringWrapper, IdentityNewsSiteDBWrapper>()
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>(new PerThreadLifetimeManager())
                .RegisterType<IRepository<News>, NewsRepository>()
                .RegisterType<IRepository<AspNetUser>, AspNetUserRepository>()
                .RegisterType<ITestUtils<string>, IdentityUtils>();

            utils = container.Resolve<ITestUtils<string>>();

            repo = container.Resolve<IRepository<News>>();
            userRepo = container.Resolve<IRepository<AspNetUser>>();
        }

        [TestMethod]
        public void Test_add_news()
        {
            // Arrange
           utils. CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            AspNetUser user = adapter.GetEntities<AspNetUser>().Single(u => u.Id == userIds.Item2);
            News news = new News()
            {
                Title = Utils.NewsTitle,
                Body = Utils.NewsBody,
                AspNetUser = user,
                CreatedDate = DateTime.Now,
            };

            // Act
            News addedNews = repo.Add(news);
            repo.SaveChanges();
            var newses = adapter.GetEntities<News>();

            // Assert
            Assert.IsTrue(newses.Count() == 1);
            Assert.IsNotNull(addedNews.AspNetUser);
            Assert.IsTrue(addedNews.AspNetUser.Id == userIds.Item2);
        }



        [TestMethod]
        public void Test_remove_news()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int newsId = utils.CreateSingleNews(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().Single(n => n.Id == newsId);

            // Act
            News removedNews = repo.Remove(news);
            repo.SaveChanges();
            var newses = adapter.GetEntities<News>();

            // Assert
            Assert.IsTrue(newses.Count() == 0);

        }

        [TestMethod]
        public void Test_update_news()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int newsId = utils.CreateSingleNews(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().Single(n => n.Id == newsId);
            string new_title_piece = "modified";

            // Act
            news.Title += new_title_piece;
            repo.SaveChanges();
            var newses = adapter.GetEntities<News>();

            // Assert
            Assert.AreEqual(news.Title, newses.Single(n => n.Id == newsId).Title);

        }


    }
}
