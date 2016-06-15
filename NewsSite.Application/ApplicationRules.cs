using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using NewsSite.Common;

namespace NewsSite.Application
{
    
    internal class ApplicationRules : IRules
    {
        private IDictionary<ReasonEnum, Expression<Func<object, bool>>> verifications;
        public IDictionary<ReasonEnum, Expression<Func<object, bool>>> Verifications
        {
            get
            {
                if (verifications == null)
                {
                    verifications = new Dictionary<ReasonEnum, Expression<Func<object, bool>>>();

                    Expression<Func<object, bool>> maxLikeF = likeCount => (int)likeCount <= 10;
                    verifications.Add(ReasonEnum.MaxLikes, maxLikeF);
                }
                return verifications;
            }
        }

        private ApplicationRules()
        {

        }
    }
}
