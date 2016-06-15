using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    public interface IRules
    {
        IDictionary<ReasonEnum, Expression<Func<object, bool>>> Verifications { get; }
    }
}
