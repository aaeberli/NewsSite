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
    using NewsSite.DataAccess.Repositories;
    using NewsSite.Domain.Model;
    using System.Data;
    using static Utils;

    [TestClass]
    public class ArticleRepositoryUT
    {
        private UnityContainer container;
        private IRepository<Article> repo;
        private IRepository<AspNetUser> userRepo;
        private ITestUtils<string> utils;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IConnStringWrapper, NewsSiteDBWrapper>()
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>(new PerThreadLifetimeManager())
                .RegisterType<IRepository<Article>, ArticleRepository>()
                .RegisterType<IRepository<AspNetUser>, AspNetUserRepository>()
                .RegisterType<ITestUtils<string>, Utils>();

            utils = container.Resolve<ITestUtils<string>>();

            repo = container.Resolve<IRepository<Article>>();
            userRepo = container.Resolve<IRepository<AspNetUser>>();
        }

        [TestMethod]
        public void Test_add_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            AspNetUser user = adapter.GetEntities<AspNetUser>().Single(u => u.Id == userIds.Item2);
            Article article = new Article()
            {
                Title = Utils.ArticleTitle,
                Body = Utils.ArticleBody,
                AspNetUser = user,
                CreatedDate = DateTime.Now,
            };

            // Act
            Article addedArticle = repo.Add(article);
            repo.SaveChanges();
            var articles = adapter.GetEntities<Article>();

            // Assert
            Assert.IsTrue(articles.Count() == 1);
            Assert.IsNotNull(addedArticle.AspNetUser);
            Assert.IsTrue(addedArticle.AspNetUser.Id == userIds.Item2);
        }



        [TestMethod]
        public void Test_remove_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articelId = utils.CreateSingleArticle(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Article article = adapter.GetEntities<Article>().Single(n => n.Id == articelId);

            // Act
            Article removedArticle = repo.Remove(article);
            repo.SaveChanges();
            var articles = adapter.GetEntities<Article>();

            // Assert
            Assert.IsTrue(articles.Count() == 0);

        }

        [TestMethod]
        public void Test_update_article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int articleId = utils.CreateSingleArticle(userIds.Item2);
            IDataAccessAdapter adapter = container.Resolve<IDataAccessAdapter>();
            Article article = adapter.GetEntities<Article>().Single(n => n.Id == articleId);
            string new_title_piece = "modified";

            // Act
            article.Title += new_title_piece;
            repo.SaveChanges();
            var articles = adapter.GetEntities<Article>();

            // Assert
            Assert.AreEqual(article.Title, articles.Single(n => n.Id == articleId).Title);

        }


    }
}
