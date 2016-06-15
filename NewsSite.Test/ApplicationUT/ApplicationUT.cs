using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using NewsSite.Common.Abstract;
using NewsSite.Test.ConnStringWrappers;
using NewsSite.DataAccess;
using NewsSite.DataAccess.Repositories;
using NewsSite.Domain.Model;
using NewsSite.Application;
using System.Collections.Generic;
using NewsSite.Application.Abstract;

namespace NewsSite.Test.ApplicationUT
{
    [TestClass]
    public class ApplicationUT
    {
        private ITestUtils<string> utils;
        private UnityContainer container;
        private INewsService newsservice;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IConnStringWrapper, IdentityNewsSiteDBWrapper>()
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>(new PerThreadLifetimeManager())
                .RegisterType<IRepository<AspNetUser>, AspNetUserRepository>()
                .RegisterType<IRepository<News>, NewsRepository>()
                .RegisterType<IRepository<Like>, LikeRepository>()
                .RegisterType<ITestUtils<string>, IdentityUtils>()
                .RegisterType<INewsService, NewsService>();

            newsservice = container.Resolve<INewsService>();
            utils = container.Resolve<ITestUtils<string>>();
        }

        [TestMethod]
        public void Test_shows_news_list()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            utils.AddLike(userIds.Item1, news2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            // Act
            IEnumerable<News> news = newsservice.GetNewsList(user);
            // Assert
            Assert.IsNotNull(news);
            Assert.AreEqual<int>(2, news.Count());
            Assert.AreEqual<int>(1, news.Single(n => n.Id == news2).Likes.Count());
        }

        [TestMethod]
        public void Test_get_single_news()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            utils.AddLike(userIds.Item1, news2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            News news = new News() { Id = news2 };

            // Act
            News retrievedNews = newsservice.GetSingleNews(user, news);

            // Assert
            Assert.IsNotNull(retrievedNews);
            Assert.AreEqual<int>(news.Id, retrievedNews.Id);
            Assert.AreEqual<int>(1, retrievedNews.Likes.Count());
        }

        [TestMethod]
        public void Test_add_like()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            News news = new News() { Id = news2 };

            // Act
            Like like = newsservice.AddLike(user, news);

            // Assert
            Assert.IsNotNull(like);
            Assert.AreEqual<string>(userIds.Item1, like.AspNetUser.Id);
            Assert.AreEqual<int>(news2, like.News.Id);
        }

        [TestMethod]
        public void Test_remove_like()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_add_like_violating_max()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            List<int> newsIds = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                newsIds.Add(utils.CreateSingleNews(userIds.Item2));

            }
            for (int i = 0; i < 5; i++)
                utils.AddLike(userIds.Item1, newsIds[i]);
            for (int i = 0; i < 5; i++)
                utils.AddLike(userIds.Item1, newsIds[i + 5]);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            News news = new News() { Id = newsIds[10] };
            var repo = container.Resolve<IRepository<Like>>();
            int likesBefore = repo.Read().Count();

            // Act
            Like like = newsservice.AddLike(user, news);
            int likesAfter = repo.Read().Count();
            // Assert
            Assert.IsNull(like);
            Assert.AreEqual<int>(likesBefore, likesAfter);
        }

        [TestMethod]
        public void Test_add_like_existing_like_and_user_and_news()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_add_news()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string body = "test body";
            string title = "test title";
            News news = new News()
            {
                Body = body,
                Title = title,
            };

            // Act
            News createdNews = newsservice.AddNews(user, news);

            // Assert
            Assert.IsNotNull(createdNews);
            Assert.AreEqual<string>(userIds.Item2, createdNews.AspNetUser.Id);
            Assert.AreEqual<string>(title, createdNews.Title);
            Assert.AreEqual<string>(body, createdNews.Body);
            Assert.AreNotEqual<DateTime>(default(DateTime), createdNews.CreatedDate);
        }

        [TestMethod]
        public void Test_add_news_no_title()
        {
            throw new NotImplementedException();
        }


        [TestMethod]
        public void Test_add_news_no_body()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_add_news_no_publisher()
        {
            throw new NotImplementedException();
        }


        [TestMethod]
        public void Test_edit_news()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_edit_news_no_title()
        {
            throw new NotImplementedException();
        }
        [TestMethod]
        public void Test_edit_news_no_body()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_edit_news_no_publisher()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_get_news_stats()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_get_news_stats_no_publisher()
        {
            throw new NotImplementedException();
        }
    }

}
