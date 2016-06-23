using NewsSite.Common.Abstract;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Application.Infrastructure
{
    /// <summary>
    /// Application Logger using NLog
    /// </summary>
    public class ApplicationLogger : ISolutionLogger
    {
        private static Logger __logger;

        protected virtual Logger _logger
        {
            get
            {
                if (__logger == null)
                {
                    var config = new LoggingConfiguration();

                    var fileTarget = new FileTarget();
                    config.AddTarget("file", fileTarget);

                    fileTarget.Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} ${message}";
                    fileTarget.FileName = "${basedir}\\log.txt";

                    var rule1 = new LoggingRule("*", LogLevel.Error, LogLevel.Error, fileTarget);
                    config.LoggingRules.Add(rule1);
                    var rule2 = new LoggingRule("*", LogLevel.Info, LogLevel.Info, fileTarget);
                    config.LoggingRules.Add(rule2);

                    LogManager.Configuration = config;

                    __logger = LogManager.GetLogger("ApplicationLogger");
                }
                return __logger;
            }
        }

        public void LogError(Exception ex, string methodName = null)
        {
            methodName = methodName == null ? string.Empty : $"{ methodName}: ";
            _logger.Error($"{methodName}{ex.Message}");
        }

        public void LogInfo(string message, string methodName = null)
        {
            methodName = methodName == null ? string.Empty : $"{ methodName}: ";
            _logger.Info($"{methodName}{message}");
        }

    }
}
