using NewsSite.Common.Abstract;
using NewsSite.DataAccess.Abstract;
using NewsSite.Domain.Model;
using System.Linq;

namespace NewsSite.DataAccess.Repositories
{

    public class NewsRepository : RepositoryBase<News>, IRepository<News>
    {
        public NewsRepository(IDataAccessAdapter dataAccessAdapter)
            : base(dataAccessAdapter)
        {

        }

        public override News Remove(News entity)
        {
            entity.Likes.Select(l => _dataAccessAdapter.Remove(l));
            return base.Remove(entity);
        }
    }
}

