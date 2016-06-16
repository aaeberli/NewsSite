using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    public interface IApplicationServiceWithRules<TRule> where TRule : class
    {
        IList<TRule> ApplicationRules { get; }

        void AddRule(TRule applicationRule);

        void ResetRules();

        bool GetRulesStatus();
    }
}
