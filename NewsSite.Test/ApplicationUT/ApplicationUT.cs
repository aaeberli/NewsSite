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
using NewsSite.Test.Abstract;
using NewsSite.Application.Infrastructure;
using NewsSite.Common;

namespace NewsSite.Test.ApplicationUT
{
    [TestClass]
    public class ApplicationUT
    {
        private ITestUtils<string> utils;
        private UnityContainer container;
        private INewsService<ApplicationRule> newsservice;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<ISolutionLogger, ApplicationLogger>()
                .RegisterType<IConnStringWrapper, NewsSiteDBWrapper>()
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>(new PerThreadLifetimeManager())
                .RegisterType<IRepository<AspNetUser>, AspNetUserRepository>()
                .RegisterType<IRepository<Article>, ArticleRepository>()
                .RegisterType<IRepository<Like>, LikeRepository>()
                .RegisterType<ITestUtils<string>, Utils>()
                .RegisterType<INewsService<ApplicationRule>, NewsService>();

            newsservice = container.Resolve<INewsService<ApplicationRule>>();
            newsservice.AutoSave = true;
            utils = container.Resolve<ITestUtils<string>>();
        }

        [TestMethod]
        public void Test_Application_shows_article_list()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            utils.AddLike(userIds.Item1, article2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            // Act
            IEnumerable<Article> article = newsservice.GetArticlesList(user);
            // Assert
            Assert.IsNotNull(article);
            Assert.AreEqual<int>(2, article.Count());
            Assert.AreEqual<int>(1, article.Single(n => n.Id == article2).Likes.Count());
        }

        [TestMethod]
        public void Test_Application_get_single_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            utils.AddLike(userIds.Item1, article2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            Article article = new Article() { Id = article2 };

            // Act
            Article retrievedArticle = newsservice.GetSingleArticle(user, article);

            // Assert
            Assert.IsNotNull(retrievedArticle);
            Assert.AreEqual<int>(article.Id, retrievedArticle.Id);
            Assert.AreEqual<int>(1, retrievedArticle.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_add_like()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            Article article = new Article() { Id = article2 };

            // Act
            Like like = newsservice.AddLike(user, article);

            // Assert
            Assert.IsNotNull(like);
            Assert.AreEqual<string>(userIds.Item1, like.AspNetUser.Id);
            Assert.AreEqual<int>(article2, like.Article.Id);
        }

        [TestMethod]
        public void Test_Application_remove_like()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, article1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            Article article = new Article() { Id = article1 };
            Like toRemove = new Like() { Id = likeId };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Like like = newsservice.RemoveLike(user, article, toRemove);
            var assertingArticle = repo.SingleOrDefault(n => n.Id == article1);

            // Assert
            Assert.IsNotNull(like);
            Assert.IsNotNull(assertingArticle);
            Assert.AreEqual<int>(0, assertingArticle.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_remove_like_wrong_user()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, article1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            Article article = new Article() { Id = article1 };
            Like toRemove = new Like() { Id = likeId };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Like like = newsservice.RemoveLike(user, article, toRemove);
            var assertingArticle = repo.SingleOrDefault(n => n.Id == article1);

            // Assert
            Assert.IsNull(like);
            Assert.IsNotNull(assertingArticle);
            Assert.AreEqual<int>(1, assertingArticle.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_remove_like_wrong_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, article1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            Article article = new Article() { Id = article2 };
            Like toRemove = new Like() { Id = likeId };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Like like = newsservice.RemoveLike(user, article, toRemove);
            var assertingArticle = repo.SingleOrDefault(n => n.Id == article1);

            // Assert
            Assert.IsNull(like);
            Assert.IsNotNull(assertingArticle);
            Assert.AreEqual<int>(1, assertingArticle.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_add_like_violating_max()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            List<int> articleIds = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                articleIds.Add(utils.CreateSingleArticle(userIds.Item2));

            }
            for (int i = 0; i < 5; i++)
                utils.AddLike(userIds.Item1, articleIds[i]);
            for (int i = 0; i < 5; i++)
                utils.AddLike(userIds.Item1, articleIds[i + 5]);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            Article article = new Article() { Id = articleIds[10] };
            var repo = container.Resolve<IRepository<Like>>();
            int likesBefore = repo.Read().Count();

            // Act
            Like like = newsservice.AddLike(user, article);
            int likesAfter = repo.Read().Count();
            // Assert
            Assert.IsNull(like);
            Assert.AreEqual<int>(likesBefore, likesAfter);
        }

        [TestMethod]
        public void Test_Application_add_like_existing_like_and_user_and_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item2, article1);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            Article article = new Article() { Id = article1 };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Like like = newsservice.AddLike(user, article);
            var assertingArticle = repo.SingleOrDefault(n => n.Id == article1);

            // Assert
            Assert.IsNull(like);
            Assert.IsNotNull(assertingArticle);
            Assert.AreEqual<int>(1, assertingArticle.Likes.Count());
        }

        [TestMethod]
        public void Test_Application_add_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string body = "test body";
            string title = "test title";
            Article article = new Article()
            {
                Body = body,
                Title = title,
            };

            // Act
            Article createdArticle = newsservice.AddArticle(user, article);

            // Assert
            Assert.IsNotNull(createdArticle);
            Assert.AreEqual<string>(userIds.Item2, createdArticle.AspNetUser.Id);
            Assert.AreEqual<string>(title, createdArticle.Title);
            Assert.AreEqual<string>(body, createdArticle.Body);
            Assert.AreNotEqual<DateTime?>(null, createdArticle.CreatedDate);
        }

        [TestMethod]
        public void Test_Application_add_article_no_title()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string body = "test body";
            Article article = new Article()
            {
                Body = body,
            };

            // Act
            Article createdArticle = newsservice.AddArticle(user, article);

            // Assert
            Assert.IsNull(createdArticle);
        }

        [TestMethod]
        public void Test_Application_add_article_title_longer_than_50()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string body = "test body";
            string title = "abcdefghij abcdefghij abcdefghij abcdefghij abcdefghij";
            Article article = new Article()
            {
                Title = title,
                Body = body,
            };

            // Act
            Article createdArticle = newsservice.AddArticle(user, article);

            // Assert
            Assert.IsNull(createdArticle);
        }


        [TestMethod]
        public void Test_Application_add_article_no_body()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string title = "test title";
            Article article = new Article()
            {
                Title = title,
            };

            // Act
            Article createdArticle = newsservice.AddArticle(user, article);

            // Assert
            Assert.IsNull(createdArticle);
        }

        [TestMethod]
        public void Test_Application_add_article_no_publisher()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            string title = "test title";
            string body = "test body";
            Article article = new Article()
            {
                Body = body,
                Title = title,
            };

            // Act
            Article createdArticle = newsservice.AddArticle(user, article);

            // Assert
            Assert.IsNull(createdArticle);
        }


        [TestMethod]
        public void Test_Application_remove_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            Article article = new Article() { Id = article2 };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Article removedArticle = newsservice.RemoveArticle(user, article);
            Article assertingArticle = repo.SingleOrDefault(n => n.Id == article2);

            // Assert
            Assert.IsNotNull(removedArticle);
            Assert.IsNull(assertingArticle);
        }


        [TestMethod]
        public void Test_Application_remove_article_bad_user()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            Article article = new Article() { Id = article2 };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Article removedArticle = newsservice.RemoveArticle(user, article);
            Article assertingArticle = repo.SingleOrDefault(n => n.Id == article2);

            // Assert
            Assert.IsNull(removedArticle);
            Assert.IsNotNull(assertingArticle);
        }

        [TestMethod]
        public void Test_Application_edit_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item2 };
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            Article article = new Article()
            {
                Id = article2,
                Title = edited_title,
                Body = edited_body,
            };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Article editedArticle = newsservice.EditArticle(user, article);
            Article assertingArticle = repo.SingleOrDefault(n => n.Id == article2);

            // Assert
            Assert.IsNotNull(editedArticle);
            Assert.AreEqual<string>(edited_title, assertingArticle.Title);
            Assert.AreEqual<string>(edited_body, assertingArticle.Body);
            Assert.AreEqual<string>(edited_title, editedArticle.Title);
            Assert.AreEqual<string>(edited_body, editedArticle.Body);
            Assert.AreNotEqual<DateTime?>(null, editedArticle.UpdatedDate);
        }

        [TestMethod]
        public void Test_Application_edit_article_bad_user()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            Article article = new Article()
            {
                Id = article2,
                Title = edited_title,
                Body = edited_body,
            };
            var repo = container.Resolve<IRepository<Article>>();

            // Act
            Article editedArticle = newsservice.EditArticle(user, article);
            Article assertingArticle = repo.SingleOrDefault(n => n.Id == article2);

            // Assert
            Assert.IsNull(editedArticle);
            Assert.AreNotEqual<string>(edited_title, assertingArticle.Title);
            Assert.AreNotEqual<string>(edited_body, assertingArticle.Body);
            Assert.AreEqual<DateTime?>(null, assertingArticle.UpdatedDate);
        }

        [TestMethod]
        public void Test_Application_get_topten_article()
        {
            // Arrange
            utils.CleanTables();
            var userIds = utils.CreateUsers();
            List<int> articleIds = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                articleIds.Add(utils.CreateSingleArticle(userIds.Item2));

            }
            for (int i = 0; i < 5; i++)
            {
                utils.AddLike(userIds.Item1, articleIds[i]);
                utils.AddLike(userIds.Item2, articleIds[i]);
            }
            for (int i = 0; i < 5; i++)
                utils.AddLike(userIds.Item1, articleIds[i + 5]);
            AspNetUser user = new AspNetUser() { Id = userIds.Item1 };
            Article article = new Article() { Id = articleIds[10] };
            var repo = container.Resolve<IRepository<Like>>();

            // Act
            IEnumerable<ArticlesStats> articlees = newsservice.GetTopTenArticles();
            IEnumerable<ArticlesStats> first5articlees = articlees.Take(5);
            // Assert
            Assert.IsTrue(articlees.Count() <= 10);
            for (int i = 0; i < 5; i++)
                Assert.IsNotNull(first5articlees.SingleOrDefault(n => (int)n.Id == articleIds[i]));
        }

    }

}
