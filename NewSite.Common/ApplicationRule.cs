using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common
{
    /// <summary>
    /// Class representing a Business Rule
    /// </summary>
    public class ApplicationRule
    {
        /// <summary>
        /// Encapsulated satisfaction of the rule
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// Encapsulates an eventual failure reason
        /// </summary>
        public ReasonEnum Reason { get; private set; }

        #region Set of overridden operators and casting
        public static bool operator &(ApplicationRule a, bool b)
        {
            return a.Result && b;
        }
        public static bool operator &(bool a, ApplicationRule b)
        {
            return a && b.Result;
        }
        public static bool operator &(ApplicationRule a, ApplicationRule b)
        {
            return a.Result && b.Result;
        }

        public static bool operator |(ApplicationRule a, bool b)
        {
            return a.Result || b;
        }
        public static bool operator |(bool a, ApplicationRule b)
        {
            return a || b.Result;
        }
        public static bool operator |(ApplicationRule a, ApplicationRule b)
        {
            return a.Result || b.Result;
        }

        public static bool operator !(ApplicationRule a)
        {
            return !a.Result;
        }

        public static implicit operator bool(ApplicationRule a)
        {
            return a.Result;
        } 
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">A Service to which the rule is added when created</param>
        /// <param name="result">Result of the rule</param>
        /// <param name="reason">Description of eventual failure reason</param>
        public ApplicationRule(IApplicationServiceWithRules<ApplicationRule> service, bool result, ReasonEnum reason)
        {
            Result = result;
            Reason = reason;
            service.AddRule(this);
        }
    }
}
