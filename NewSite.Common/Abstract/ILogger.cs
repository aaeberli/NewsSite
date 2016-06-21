using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    public interface ISolutionLogger
    {
        void LogError(Exception ex, string methodName = null);

        void LogInfo(string message, string methodName = null);
    }
}
