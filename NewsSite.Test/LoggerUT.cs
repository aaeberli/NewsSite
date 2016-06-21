using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsSite.Application.Infrastructure;

namespace NewsSite.Test
{
    [TestClass]
    public class LoggerUT
    {
        [TestMethod]
        public void Test_ApplicationLogger_error()
        {
            // Arrange

            // Act
            ApplicationLogger logger = new ApplicationLogger();
            logger.LogError(new Exception("test_error_log"));

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_ApplicationLogger_info()
        {
            // Arrange

            // Act
            ApplicationLogger logger = new ApplicationLogger();
            logger.LogInfo("test_info_log");

            // Assert
            Assert.IsTrue(true);
        }
    }
}
