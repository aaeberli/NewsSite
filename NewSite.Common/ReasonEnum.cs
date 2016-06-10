using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common
{
    public enum ReasonEnum
    {
        Generic,
        Error,
        ElementValidation,
        ElementDuplication,
        ElementPresence,
        UserIdLength,
        ElementExpired,
        WrongPassword,
        WrongUser,
    }
}
