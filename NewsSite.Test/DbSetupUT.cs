using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsSite.Test
{
    using Abstract;
    using ConnStringWrappers;
    using NewsSite.Common.Abstract;
    using NewsSite.DataAccess;
    using NewsSite.Domain.Model;

    [TestClass]
    public class DbSetupUT
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
                .RegisterType<IConnStringWrapper, NewsSiteDBWrapper>()
                .RegisterType<ITestUtils<string>, Utils>();

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
            utils.CleanTables();
            utils.CreateUsers();
        }

        [TestMethod]
        public void Test_create_single_article_db()
        {
            // Arrange
            utils.CleanTables();
            // Act
            int articleId = utils.CreateSingleArticle(utils.CreateUsers().Item1);
            // Assert
            Assert.IsTrue(articleId > 0);
        }

        [TestMethod]
        public void Test_create_single_like_db()
        {
            // Arrange
            utils.CleanTables();
            // Act
            var users = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(users.Item1);
            int likeId = utils.AddLike(users.Item2, articleId);
            // Assert
            Assert.IsTrue(likeId > 0);
        }


        [TestMethod]
        public void Test_dataAdapter_create_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            string test_title = "test_title";
            string test_body = "test_body";
            Article article = adapter.Create<Article>();
            article.Title = test_title;
            article.Body = test_body;
            article.CreatedDate = DateTime.Now;
            article.AuthorId = userIds.Item1;

            // Act
            adapter.Add<Article>(article);
            adapter.SaveChanges();
            List<Article> articleList = adapter.GetEntities<Article>().ToList();
            Article createdArticle = articleList[0];

            // Assert
            Assert.IsTrue(articleList.Count == 1);
            Assert.IsNotNull(createdArticle.AspNetUser);
            Assert.IsTrue(createdArticle.AuthorId == userIds.Item1);
            Assert.IsTrue(createdArticle.Likes.Count() == 0);
        }

        [TestMethod]
        public void Test_dataAdapter_update_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Article article = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);
            string new_title_piece = "modified";

            // Act
            article.Title += new_title_piece;
            article.Body += new_title_piece;
            article.UpdatedDate = DateTime.Now;
            adapter.SaveChanges();
            Article modifiedArticle = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);

            // Assert
            Assert.IsNotNull(modifiedArticle);
            Assert.AreEqual(article.Title, modifiedArticle.Title);
            Assert.AreEqual(article.Body, modifiedArticle.Body);
            Assert.AreEqual(article.UpdatedDate, modifiedArticle.UpdatedDate);
            Assert.IsTrue(modifiedArticle.Likes.Count() == 0);
        }

        [TestMethod]
        public void Test_dataAdapter_delete_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            utils.AddLike(userIds.Item2, articleId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Article article = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);

            // Act
            article.Likes.Select(l => adapter.Remove(l));
            //for (int i = 0; i < article.Likes.Count(); i++)
            //{
            //    adapter.Remove(article.Likes.ElementAt(i));
            //}
            Article deletedArticle = adapter.Remove(article);
            adapter.SaveChanges();
            IEnumerable<Article> articlees = adapter.GetEntities<Article>();

            // Assert
            Assert.IsNotNull(deletedArticle);
            Assert.AreEqual(0, articlees.Count());
        }

        [TestMethod]
        public void Test_dataAdapter_add_like()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Article article = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);
            Like like = adapter.Create<Like>();
            like.CreatedDate = DateTime.Now;
            like.ArticleId = articleId;
            like.UserId = userIds.Item1;

            // Act
            article.Likes.Add(like);
            adapter.SaveChanges();
            Article modifiedArticle = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);

            // Assert
            Assert.IsNotNull(modifiedArticle);
            Assert.IsTrue(modifiedArticle.Likes.Count() == 1);
            Assert.IsNotNull(like.Article);
            Assert.IsNotNull(like.AspNetUser);
        }


        [TestMethod]
        public void Test_dataAdapter_remove_like()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, articleId);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Article article = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);
            Like like = article.Likes.Single(l => l.Id == likeId);

            // Act
            adapter.Remove(like);
            adapter.SaveChanges();
            Article modifiedArticle = adapter.GetEntities<Article>().SingleOrDefault(n => n.Id == articleId);

            // Assert
            Assert.IsNotNull(modifiedArticle);
            Assert.IsTrue(modifiedArticle.Likes.Count() == 0);
        }
    }
}
