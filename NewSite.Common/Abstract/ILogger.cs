﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    public interface ILogger
    {
        void LogError(Exception ex);

        void LogInfo(string message);
    }
}