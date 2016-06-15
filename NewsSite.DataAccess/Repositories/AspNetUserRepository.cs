using NewsSite.Common.Abstract;
using NewsSite.DataAccess.Abstract;
using NewsSite.Domain.Model;
using System.Linq;

namespace NewsSite.DataAccess.Repositories
{

    public class AspNetUserRepository : RepositoryBase<AspNetUser>, IRepository<AspNetUser>
    {
        public AspNetUserRepository(IDataAccessAdapter dataAccessAdapter)
            : base(dataAccessAdapter)
        {

        }

        public override AspNetUser Remove(AspNetUser entity)
        {
            entity.Likes.Select(l => _dataAccessAdapter.Remove(l));
            return base.Remove(entity);
        }
    }
}

