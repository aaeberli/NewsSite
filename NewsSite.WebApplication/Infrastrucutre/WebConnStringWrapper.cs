using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsSite.WebApplication.Infrastrucutre
{
    /// <summary>
    /// Local implementation of IConnStringWrapper
    /// </summary>
    public class WebConnStringWrapper : IConnStringWrapper
    {
        public string ConnectionName
        {
            get
            {
                return "DefaultConnection";
            }
        }
    }
}