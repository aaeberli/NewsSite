using NewsSite.Common.Abstract;
using NewsSite.DataAccess.Abstract;
using NewsSite.Domain.Model;
using System.Linq;

namespace NewsSite.DataAccess.Repositories
{

    public class ArticleRepository : RepositoryBase<Article>, IRepository<Article>
    {
        public ArticleRepository(IDataAccessAdapter dataAccessAdapter)
            : base(dataAccessAdapter)
        {

        }

        public override Article Remove(Article entity)
        {
            entity.Likes.Select(l => _dataAccessAdapter.Remove(l));
            return base.Remove(entity);
        }
    }
}

