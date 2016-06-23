using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsSite.Application;
using NewsSite.Application.Abstract;
using NewsSite.Application.Infrastructure;
using NewsSite.Common;
using NewsSite.Common.Abstract;
using NewsSite.DataAccess;
using NewsSite.DataAccess.Repositories;
using NewsSite.Domain.Model;
using NewsSite.Test;
using NewsSite.Test.Abstract;
using NewsSite.Test.ConnStringWrappers;
using NewsSite.Test.Fakes;
using NewsSite.Test.Mock;
using NewsSite.WebApplication.Controllers;
using NewsSite.WebApplication.Infrastrucutre;
using NewsSite.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NewsSite.Test.ControllersUT
{
    [TestClass]
    public class ArticleControllerUT
    {
        UnityContainer container;
        ITestUtils<string> utils;

        [TestInitialize]
        public void Init()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<ISolutionLogger, ApplicationLogger>()
                .RegisterType<ISolutionLogger, FakeLogger>()
                .RegisterType<IConnStringWrapper, NewsSiteDBWrapper>()
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>()
                .RegisterType<IRepository<AspNetUser>, AspNetUserRepository>()
                .RegisterType<IRepository<Article>, ArticleRepository>()
                .RegisterType<IRepository<Like>, LikeRepository>()
                .RegisterType<INewsService<ApplicationRule>, NewsService>()
                .RegisterType<ITestUtils<string>, DbUtils>()
                .RegisterType<IMapperAdapter, MapperAdapter>()
                .RegisterType<Controller, MockArticleController>();

            utils = container.Resolve<ITestUtils<string>>();
        }

        internal ArticleController ControllerSetup(string userId, string roleName)
        {
            FakeUserProvider fakeUserProvider = new FakeUserProvider()
            {
                FakeRoles = new List<string>() { roleName },
                FakeUserId = userId
            };
            container.RegisterInstance<IUserProvider>(fakeUserProvider);
            MockArticleController controller = container.Resolve<Controller>() as MockArticleController;
            controller.BeginExecute();
            return controller;
        }

        [TestMethod]
        public void Test_ArticleController_ArticleList()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            IEnumerable<ArticleViewModel> articles = controller.ArticleList().ViewData.Model as IEnumerable<ArticleViewModel>;

            // Assert
            Assert.IsNotNull(articles);
            Assert.AreEqual<int>(2, articles.Count());
        }


        [TestMethod]
        public void Test_ArticleController_ArticleList_WrongUSer()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "qwlrknlq3";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());

            // Act
            IEnumerable<ArticleViewModel> articles = controller.ArticleList().ViewData.Model as IEnumerable<ArticleViewModel>;

            // Assert
            Assert.IsNull(articles);
        }

        [TestMethod]
        public void Test_ArticleController_PersonalArticleList()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item1);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            IEnumerable<ArticleViewModel> articles = controller.PersonalArticleList().ViewData.Model as IEnumerable<ArticleViewModel>;

            // Assert
            Assert.IsNotNull(articles);
            Assert.AreEqual<int>(1, articles.Count());
        }

        [TestMethod]
        public void Test_ArticleController_PersonalArticleList_WrongUser()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item1);
            string wrong_user = "qwlrknlq3";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());

            // Act
            IEnumerable<ArticleViewModel> articles = controller.PersonalArticleList().ViewData.Model as IEnumerable<ArticleViewModel>;

            // Assert
            Assert.IsNull(articles);
        }

        [TestMethod]
        public void Test_ArticleController_EditArticle_GET()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());

            // Act
            ArticleViewModel article = controller.EditArticle(article1).ViewData.Model as ArticleViewModel;

            // Assert
            Assert.IsNotNull(article);
            Assert.AreEqual<int>(article1, article.Id);
            Assert.AreEqual<string>(userIds.Item2, article.AuthorId);
        }

        [TestMethod]
        public void Test_ArticleController_EditArticle_GET_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "qwlrknlq3";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());

            // Act
            ArticleViewModel articles = controller.EditArticle(article1).ViewData.Model as ArticleViewModel;

            // Assert
            Assert.IsNull(articles);
        }

        [TestMethod]
        public void Test_ArticleController_EditArticle_POST()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            ArticleViewModel vm = new ArticleViewModel() { Id = article1, Title = edited_title, Body = edited_body };

            // Act
            string url = (controller.EditArticle(vm) as RedirectResult).Url;

            // Assert
            Assert.IsFalse(url.IsNullOrEmptyOrWhiteSpace());
        }

        [TestMethod]
        public void Test_ArticleController_EditArticle_POST_NoBody()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());
            controller.ModelState.AddModelError("Body", "");
            string edited_title = "edited_title";
            string edited_body = null;
            ArticleViewModel vm = new ArticleViewModel() { Id = article1, Title = edited_title, Body = edited_body };

            // Act
            string viewName = (controller.EditArticle(vm) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual<string>("CreateArticle", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_EditArticle_POST_No_Publisher()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            ArticleViewModel vm = new ArticleViewModel() { Id = article1, Title = edited_title, Body = edited_body };

            // Act
            string viewName = (controller.EditArticle(vm) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_EditArticle_POST_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "wrong_user";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            ArticleViewModel vm = new ArticleViewModel() { Id = article1, Title = edited_title, Body = edited_body };

            // Act
            string viewName = (controller.EditArticle(vm) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_CreateArticle_GET()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());

            // Act
            string viewName = controller.CreateArticle().ViewName;

            // Assert
            Assert.AreEqual<string>("CreateArticle", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_CreateArticle_POST()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());
            string edited_title = "edited_title";
            string edited_body = "edited_body";
            ArticleViewModel vm = new ArticleViewModel() { Title = edited_title, Body = edited_body };

            // Act
            string url = (controller.CreateArticle(vm) as RedirectResult).Url;

            // Assert
            Assert.AreEqual<string>("ArticleList", url);
        }

        [TestMethod]
        public void Test_ArticleController_CreateArticle_POST_No_Title()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());
            controller.ModelState.AddModelError("Title", "");
            string edited_title = "";
            string edited_body = "created_body";
            ArticleViewModel vm = new ArticleViewModel() { Title = edited_title, Body = edited_body };

            // Act
            string viewName = (controller.CreateArticle(vm) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual<string>("CreateArticle", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_CreateArticleTest_POST_No_Publisher()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());
            string edited_title = "created_title";
            string edited_body = "created_body";
            ArticleViewModel vm = new ArticleViewModel() { Title = edited_title, Body = edited_body };

            // Act
            string viewName = (controller.CreateArticle(vm) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_CreateArticleTest_POST_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "wrong_user";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());
            string edited_title = "created_title";
            string edited_body = "created_body";
            ArticleViewModel vm = new ArticleViewModel() { Title = edited_title, Body = edited_body };

            // Act
            string viewName = (controller.CreateArticle(vm) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_ViewArticle()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            ArticleViewModel vm = controller.ViewArticle(article1).ViewData.Model as ArticleViewModel;

            // Assert
            Assert.IsNotNull(vm);
            Assert.AreEqual<int>(article1, vm.Id);
            Assert.AreEqual<string>(userIds.Item2, vm.AuthorId);
        }

        [TestMethod]
        public void Test_ArticleController_ViewArticle_Wrong_Id()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            string viewName = controller.ViewArticle(article1 + 10).ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_ViewArticle_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "wrong_user";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());

            // Act
            string viewName = controller.ViewArticle(article1).ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_LikeArticle()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());

            // Act
            string url = (controller.LikeArticle(article1) as RedirectResult).Url;
            string[] actionName = url.Split('?');
            string[] qs = actionName[1].Split('=');

            // Assert
            Assert.AreEqual("ViewArticle", actionName[0]);
            Assert.AreEqual("articleId", qs[0]);
            Assert.IsTrue(int.Parse(qs[1]) >= 0);
        }

        [TestMethod]
        public void Test_ArticleController_LikeArticle_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "wrong_user";
            ArticleController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());

            // Act
            string viewName = (controller.LikeArticle(article1) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_LikeArticle_Wrong_Article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());
            int wrong_article = article2 + 10;

            // Act
            string viewName = (controller.LikeArticle(wrong_article) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_UnlikeArticle()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, article1);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            string url = (controller.UnlikeArticle(article1, likeId) as RedirectResult).Url;
            string[] actionName = url.Split('?');
            string[] qs = actionName[1].Split('=');

            // Assert
            Assert.AreEqual("ViewArticle", actionName[0]);
            Assert.AreEqual("articleId", qs[0]);
            Assert.IsTrue(int.Parse(qs[1]) >= 0);
        }

        [TestMethod]
        public void Test_ArticleController_UnlikeArticle_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, article1);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());

            // Act
            string viewName = (controller.UnlikeArticle(article1, likeId) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_UnlikeArticle_Wrong_Article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, article1);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            string viewName = (controller.UnlikeArticle(article2, likeId) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_RemoveArticle()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, article1);
            ArticleController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());

            // Act
            string url = (controller.RemoveArticle(article1) as RedirectResult).Url;

            // Assert
            Assert.AreEqual("ArticleList", url);
        }

        [TestMethod]
        public void Test_ArticleController_RemoveArticle_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, article1);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            string viewName = (controller.RemoveArticle(article1) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual("Error", viewName);
        }

        [TestMethod]
        public void Test_ArticleController_RemoveArticle_Wrong_Article()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            int article1 = utils.CreateSingleArticle(userIds.Item2);
            int article2 = utils.CreateSingleArticle(userIds.Item2);
            int likeId = utils.AddLike(userIds.Item1, article1);
            ArticleController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            string viewName = (controller.RemoveArticle(article1 + 10) as ViewResult).ViewName;

            // Assert
            Assert.AreEqual("Error", viewName);
        }
    }
}