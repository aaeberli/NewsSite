using System;
using NewsSite.Common.Abstract;
using System.Diagnostics;

namespace NewsSite.Test
{
    internal class FakeLogger : ISolutionLogger
    {

        public void LogError(Exception ex, string methodName = null)
        {
            Debug.Write(ex.Message);
        }

        public void LogInfo(string message, string methodName = null)
        {
            Debug.Write(message);
        }
    }
}