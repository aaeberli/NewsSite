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

        /// <summary>
        /// Removes dependent elements before removing the parent
        /// </summary>
        /// <param name="entity">The Article to remove</param>
        /// <returns>REmoved article</returns>
        public override Article Remove(Article entity)
        {
            entity.Likes.Select(l => _dataAccessAdapter.Remove(l));
            return base.Remove(entity);
        }
    }
}

