using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OtpManager.Test
{
    using NewsSite.Common.Abstract;
    using NewsSite.DataAccess;
    using NewsSite.DataAccess.Repositories;
    using NewsSite.Domain.Model;
    using System.Data;
    using static Utils;

    [TestClass]
    public class UserRepositoryUT
    {
        private UnityContainer container;
        private IRepository<User> repo;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            container = new UnityContainer();

            container
                .RegisterType<IDataAccessAdapter, DataAccessAdapter>()
                .RegisterType<IRepository<User>, UserRepository>();

            repo = container.Resolve<IRepository<User>>();
        }


        [TestMethod]
        public void Test_add_user()
        {

        }
    }
}
