using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsSite.WebApplication.Infrastrucutre
{
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