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
            for (int i = 0; i < entity.Likes.Count; i++)
            {
                _dataAccessAdapter.Remove(entity.Likes.ElementAt(i));
            }
            return base.Remove(entity);
        }
    }
}

