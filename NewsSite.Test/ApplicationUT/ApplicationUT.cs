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
            newsservice.AutoSave = true;
            utils = container.Resolve<ITestUtils<string>>();
        }

        [TestMethod]
        public void Test_Application_shows_news_list()
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
        public void Test_Application_get_single_news()
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
        public void Test_Application_add_like()
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
        public void Test_Application_remove_like()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, news1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            News news = new News() { Id = news1 };
            Like toRemove = new Like() { Id = likeId };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            Like like = newsservice.RemoveLike(user, news, toRemove);
            var assertingNews = repo.SingleOrDefault(n => n.Id == news1);

            // Assert
            Assert.IsNotNull(like);
            Assert.IsNotNull(assertingNews);
            Assert.AreEqual<int>(0, assertingNews.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_remove_like_wrong_user()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, news1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            News news = new News() { Id = news1 };
            Like toRemove = new Like() { Id = likeId };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            Like like = newsservice.RemoveLike(user, news, toRemove);
            var assertingNews = repo.SingleOrDefault(n => n.Id == news1);

            // Assert
            Assert.IsNull(like);
            Assert.IsNotNull(assertingNews);
            Assert.AreEqual<int>(1, assertingNews.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_remove_like_wrong_news()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, news1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            News news = new News() { Id = news2 };
            Like toRemove = new Like() { Id = likeId };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            Like like = newsservice.RemoveLike(user, news, toRemove);
            var assertingNews = repo.SingleOrDefault(n => n.Id == news1);

            // Assert
            Assert.IsNull(like);
            Assert.IsNotNull(assertingNews);
            Assert.AreEqual<int>(1, assertingNews.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_add_like_violating_max()
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
        public void Test_Application_add_like_existing_like_and_user_and_news()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, news1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            News news = new News() { Id = news1 };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            Like like = newsservice.AddLike(user, news);
            var assertingNews = repo.SingleOrDefault(n => n.Id == news1);

            // Assert
            Assert.IsNull(like);
            Assert.IsNotNull(assertingNews);
            Assert.AreEqual<int>(1, assertingNews.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_add_news()
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
            Assert.AreNotEqual<DateTime?>(null, createdNews.CreatedDate);
        }

        [TestMethod]
        public void Test_Application_add_news_no_title()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string body = "test body";
            News news = new News()
            {
                Body = body,
            };

            // Act
            News createdNews = newsservice.AddNews(user, news);

            // Assert
            Assert.IsNull(createdNews);
        }


        [TestMethod]
        public void Test_Application_add_news_no_body()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string title = "test title";
            News news = new News()
            {
                Title = title,
            };

            // Act
            News createdNews = newsservice.AddNews(user, news);

            // Assert
            Assert.IsNull(createdNews);
        }

        [TestMethod]
        public void Test_Application_add_news_no_publisher()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            string title = "test title";
            string body = "test body";
            News news = new News()
            {
                Body = body,
                Title = title,
            };

            // Act
            News createdNews = newsservice.AddNews(user, news);

            // Assert
            Assert.IsNull(createdNews);
        }


        [TestMethod]
        public void Test_Application_remove_news()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            News news = new News() { Id = news2 };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            News removedNews = newsservice.RemoveNews(user, news);
            News assertingNews = repo.SingleOrDefault(n => n.Id == news2);

            // Assert
            Assert.IsNotNull(removedNews);
            Assert.IsNull(assertingNews);
        }


        [TestMethod]
        public void Test_Application_remove_news_bad_user()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            News news = new News() { Id = news2 };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            News removedNews = newsservice.RemoveNews(user, news);
            News assertingNews = repo.SingleOrDefault(n => n.Id == news2);

            // Assert
            Assert.IsNull(removedNews);
            Assert.IsNotNull(assertingNews);
        }

        [TestMethod]
        public void Test_Application_edit_news()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            News news = new News()
            {
                Id = news2,
                Title = edited_title,
                Body = edited_body,
            };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            News editedNews = newsservice.EditNews(user, news);
            News assertingNews = repo.SingleOrDefault(n => n.Id == news2);

            // Assert
            Assert.IsNotNull(editedNews);
            Assert.AreEqual<string>(edited_title, assertingNews.Title);
            Assert.AreEqual<string>(edited_body, assertingNews.Body);
            Assert.AreEqual<string>(edited_title, editedNews.Title);
            Assert.AreEqual<string>(edited_body, editedNews.Body);
            Assert.AreNotEqual<DateTime?>(null, editedNews.UpdatedDate);
        }

        [TestMethod]
        public void Test_Application_edit_news_bad_user()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int news1 = utils.CreateSingleNews(userIds.Item2);
            int news2 = utils.CreateSingleNews(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            News news = new News()
            {
                Id = news2,
                Title = edited_title,
                Body = edited_body,
            };
            var repo = container.Resolve<IRepository<News>>();

            // Act
            News editedNews = newsservice.EditNews(user, news);
            News assertingNews = repo.SingleOrDefault(n => n.Id == news2);

            // Assert
            Assert.IsNull(editedNews);
            Assert.AreNotEqual<string>(edited_title, assertingNews.Title);
            Assert.AreNotEqual<string>(edited_body, assertingNews.Body);
            Assert.AreEqual<DateTime?>(null, assertingNews.UpdatedDate);
        }

        [TestMethod]
        public void Test_Application_get_topten_news()
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
            {
                utils.AddLike(userIds.Item1, newsIds[i]);
                utils.AddLike(userIds.Item2, newsIds[i]);
            }
            for (int i = 0; i < 5; i++)
                utils.AddLike(userIds.Item1, newsIds[i + 5]);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            News news = new News() { Id = newsIds[10] };
            var repo = container.Resolve<IRepository<Like>>();

            // Act
            IEnumerable<NewsStats> newses = newsservice.GetTopTenNews();
            IEnumerable<NewsStats> first5newses = newses.Take(5);
            // Assert
            Assert.IsTrue(newses.Count() <= 10);
            for (int i = 0; i < 5; i++)
                Assert.IsNotNull(first5newses.SingleOrDefault(n => (int)n.Id == newsIds[i]));
        }

    }

}
