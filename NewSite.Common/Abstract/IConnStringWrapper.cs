using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Wraps a connection string's name (used in dbContext instantiation
    /// </summary>
    public interface IConnStringWrapper
    {
        string ConnectionName { get; }
    }
}
