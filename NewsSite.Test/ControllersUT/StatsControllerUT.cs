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
    public class StatsControllerUT
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
                .RegisterType<Controller, MockStatsController>();

            utils = container.Resolve<ITestUtils<string>>();
        }

        internal StatsController ControllerSetup(string userId, string roleName)
        {
            FakeUserProvider fakeUserProvider = new FakeUserProvider()
            {
                FakeRoles = new List<string>() { roleName },
                FakeUserId = userId
            };
            container.RegisterInstance<IUserProvider>(fakeUserProvider);
            MockStatsController controller = container.Resolve<Controller>() as MockStatsController;
            controller.BeginExecute();
            return controller;
        }

        [TestMethod]
        public void Test_StatsController_GetStats()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item2);
            StatsController controller = ControllerSetup(userIds.Item2, RoleType.Employee.ToString());

            // Act
            IEnumerable<ArticlesStatsViewModel> articles = controller.GetStats().ViewData.Model as IEnumerable<ArticlesStatsViewModel>;

            // Assert
            Assert.IsNotNull(articles);
            Assert.AreEqual<int>(2, articles.Count());
        }

        [TestMethod]
        public void Test_StatsController_GetStats_No_Publisher()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item2);
            StatsController controller = ControllerSetup(userIds.Item1, RoleType.Employee.ToString());

            // Act
            string viewName = controller.GetStats().ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }

        [TestMethod]
        public void Test_StatsController_GetStats_Wrong_User()
        {
            // Arrange
            utils.CleanTables();
            Tuple<string, string> userIds = utils.CreateUsers();
            utils.CreateSingleArticle(userIds.Item2);
            utils.CreateSingleArticle(userIds.Item2);
            string wrong_user = "wrong_user";
            StatsController controller = ControllerSetup(wrong_user, RoleType.Employee.ToString());

            // Act
            string viewName = controller.GetStats().ViewName;

            // Assert
            Assert.AreEqual<string>("Error", viewName);
        }
    }
}