using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OtpManager.Test
{
    using NewsSite.Common.Abstract;
    using NewsSite.DataAccess;
    using NewsSite.Domain.Model;
    using static Utils;

    [TestClass]
    public class DataAccessAdapterUT
    {
        private UnityContainer container;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>();
        }

        [TestMethod]
        public void Test_clean_db()
        {
            CleanTables();
        }

        [TestMethod]
        public void Test_setup_db()
        {
            CreateUsers();
        }

        [TestMethod]
        public void Test_create_single_news_db()
        {
            // Arrange
            CleanTables();
            // Act
            int newsId = CreateSingleNews(CreateUsers().Item1);
            // Assert
            Assert.IsTrue(newsId > 0);
        }

        [TestMethod]
        public void Test_create_like_db()
        {
            // Arrange
            CleanTables();
            int userId = CreateUsers().Item1;
            int likeId = AddLike(userId, CreateSingleNews(userId));
            // Act
            // Assert
            Assert.IsTrue(likeId > 0);
        }

        [TestMethod]
        public void Test_dataAdapter_create_news()
        {
            // Arrange
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            string test_title = "test_title";
            string test_body = "test_body";
            User author = adapter.GetEntities<User>().SingleOrDefault(u => u.Id == userIds.Item1);
            News news = new News
            {
                Title = test_title,
                Body = test_body,
                CreatedDate = DateTime.Now,
               // User = author,
                AuthorId = userIds.Item1,
            };

            // Act
            adapter.Add<News>(news);
            adapter.SaveChanges();
            List<News> newsList = adapter.GetEntities<News>().ToList();
            News createdNews = newsList[0];

            // Assert
            Assert.IsTrue(newsList.Count == 1);
            Assert.IsNotNull(createdNews.User);
            Assert.IsTrue(createdNews.User.UserName == Utils.TestPublisher);
            Assert.IsTrue(createdNews.AuthorId == userIds.Item1);
            Assert.IsTrue(createdNews.Likes.Count() == 0);
        }

        [TestMethod]
        public void Test_dataAdapter_update_news()
        {
            // Arrange
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            int newsId = CreateSingleNews(userIds.Item2);
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
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            int newsId = CreateSingleNews(userIds.Item2);
            AddLike(userIds.Item2, newsId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);

            // Act
            for (int i = 0; i < news.Likes.Count(); i++)
            {
                adapter.Remove(news.Likes.ElementAt(i));
            }
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
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            int newsId = CreateSingleNews(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            News news = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);
            User liker = adapter.GetEntities<User>().SingleOrDefault(u => u.Id == userIds.Item1);
            Like like = new Like() { News = news, User = liker, CreatedDate = DateTime.Now };

            // Act
            news.Likes.Add(like);
            adapter.SaveChanges();
            News modifiedNews = adapter.GetEntities<News>().SingleOrDefault(n => n.Id == newsId);

            // Assert
            Assert.IsNotNull(modifiedNews);
            Assert.IsTrue(modifiedNews.Likes.Count() == 1);
        }


        [TestMethod]
        public void Test_dataAdapter_remove_like()
        {
            // Arrange
            CleanTables();
            Tuple<int, int> userIds = CreateUsers();
            int newsId = CreateSingleNews(userIds.Item2);
            int likeId = AddLike(userIds.Item1, newsId);
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
