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
    using NewsSite.Domain.Model;

    [TestClass]
    public class DataAccessAdapterUT
    {
        private UnityContainer container;
        //private ITestUtils<int> utils;
        private ITestUtils<string> utils;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();
            
            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>()
                .RegisterType<IConnStringWrapper, IdentityNewsSiteDBWrapper>()
                .RegisterType<ITestUtils<string>, IdentityUtils>();

            utils = container.Resolve<ITestUtils<string>>();
        }

        [TestMethod]
        public void Test_clean_db()
        {
            utils.CleanTables();
        }

        [TestMethod]
        public void Test_setup_db()
        {
            utils.CreateUsers();
        }

        [TestMethod]
        public void Test_create_single_news_db()
        {
            // Arrange
            utils.CleanTables();
            // Act
            int newsId = utils.CreateSingleNews(utils.CreateUsers().Item1);
            // Assert
            Assert.IsTrue(newsId > 0);
        }

        [TestMethod]
        public void Test_create_like_db()
        {
            // Arrange
            utils.CleanTables();
            string userId = utils.CreateUsers().Item1;
            int likeId = utils.AddLike(userId, utils.CreateSingleNews(userId));
            // Act
            // Assert
            Assert.IsTrue(likeId > 0);
        }

        [TestMethod]
        public void Test_dataAdapter_create_news()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            string test_title = "test_title";
            string test_body = "test_body";
            News news = adapter.Create<News>();
            news.Title = test_title;
            news.Body = test_body;
            news.CreatedDate = DateTime.Now;
            news.AuthorId = userIds.Item1;

            // Act
            adapter.Add<News>(news);
            adapter.SaveChanges();
            List<News> newsList = adapter.GetEntities<News>().ToList();
            News createdNews = newsList[0];

            // Assert
            Assert.IsTrue(newsList.Count == 1);
            Assert.IsNotNull(createdNews.AspNetUser);
            Assert.IsTrue(createdNews.AuthorId == userIds.Item1);
            Assert.IsTrue(createdNews.Likes.Count() == 0);
        }

        [TestMethod]
        public void Test_dataAdapter_update_news()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int newsId = utils.CreateSingleNews(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);
            string new_title_piece = "modified";

            // Act
            news.Title += new_title_piece;
            news.Body += new_title_piece;
            news.UpdatedDate = DateTime.Now;
            adapter.SaveChanges();
            News modifiedNews = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);

            // Assert
            Assert.IsNotNull(modifiedNews);
            Assert.AreEqual(news.Title, modifiedNews.Title);
            Assert.AreEqual(news.Body, modifiedNews.Body);
            Assert.AreEqual(news.UpdatedDate, modifiedNews.UpdatedDate);
            Assert.IsTrue(modifiedNews.Likes.Count() == 0);
        }

        [TestMethod]
        public void Test_dataAdapter_delete_news()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int newsId = utils.CreateSingleNews(userIds.Item2);
            utils.AddLike(userIds.Item2, newsId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);

            // Act
            news.Likes.Select(l => adapter.Remove(l));
            //for (int i = 0; i < news.Likes.Count(); i++)
            //{
            //    adapter.Remove(news.Likes.ElementAt(i));
            //}
            News deletedNews = adapter.Remove(news);
            adapter.SaveChanges();
            IEnumerable<News> newses = adapter.GetEntities<News>();

            // Assert
            Assert.IsNotNull(deletedNews);
            Assert.AreEqual(0, newses.Count());
        }

        [TestMethod]
        public void Test_dataAdapter_add_like()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int newsId = utils.CreateSingleNews(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);
            Like like = adapter.Create<Like>();
            like.CreatedDate = DateTime.Now;
            like.NewsId = newsId;
            like.UserId = userIds.Item1;

            // Act
            news.Likes.Add(like);
            adapter.SaveChanges();
            News modifiedNews = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);

            // Assert
            Assert.IsNotNull(modifiedNews);
            Assert.IsTrue(modifiedNews.Likes.Count() == 1);
            Assert.IsNotNull(like.News);
            Assert.IsNotNull(like.AspNetUser);
        }


        [TestMethod]
        public void Test_dataAdapter_remove_like()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int newsId = utils.CreateSingleNews(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, newsId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);
            Like like = news.Likes.Single(l => l.Id == likeId);

            // Act
            adapter.Remove(like);
            adapter.SaveChanges();
            News modifiedNews = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);

            // Assert
            Assert.IsNotNull(modifiedNews);
            Assert.IsTrue(modifiedNews.Likes.Count() == 0);
        }
    }
}
