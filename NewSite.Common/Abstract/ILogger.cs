using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Defines a structure for a Logger
    /// </summary>
    public interface ISolutionLogger
    {
        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="methodName">Name of the method which caused the error</param>
        void LogError(Exception ex, string methodName = null);

        /// <summary>
        /// Logs an information
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="methodName">Name of the method which caused the error</param>
        void LogInfo(string message, string methodName = null);
    }
}
