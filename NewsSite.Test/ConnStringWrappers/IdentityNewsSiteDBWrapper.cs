﻿using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Test.ConnStringWrappers
{
    internal class IdentityNewsSiteDBWrapper : IConnStringWrapper
    {
        public string ConnectionName
        {
            get
            {
                return "IdentityNewsSiteDB";
            }
        }
    }
}
