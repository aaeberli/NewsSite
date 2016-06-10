using NewsSite.Common.Abstract;
using NewsSite.DataAccess.Abstract;
using NewsSite.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.DataAccess.Repositories
{

    public class UserRepository : RepositoryBase<User>, IRepository<User>
    {
        public UserRepository(IDataAccessAdapter dataAccessAdapter)
            : base(dataAccessAdapter)
        {

        }

    }
}

