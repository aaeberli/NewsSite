using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Defines a Service containing a set of rules to be verified
    /// </summary>
    /// <typeparam name="TRule">Type of the Business Rule</typeparam>
    public interface IApplicationServiceWithRules<TRule> where TRule : class
    {
        /// <summary>
        /// Set of rules populated dynamically by the service at each operation
        /// </summary>
        IList<TRule> ApplicationRules { get; }

        /// <summary>
        /// Adds a rule
        /// </summary>
        /// <param name="applicationRule">The rule to be added</param>
        void AddRule(TRule applicationRule);

        /// <summary>
        /// Resets rules (typically before every action is performed)
        /// </summary>
        void ResetRules();

        /// <summary>
        /// Gets the status of the rules (false if at least one rule is not satisfied)
        /// </summary>
        /// <returns></returns>
        bool GetRulesStatus();
    }
}
