using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Defines a data adapter (typically over dbContext or DataContext)
    /// </summary>
    public interface IDataAccessAdapter : IDisposable
    {
        /// <summary>
        /// Gets a list of Entities
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <returns>List of Entities</returns>
        IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Create a new Entity for insertion
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <returns>The created Entity</returns>
        TEntity Create<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Adds an Entity
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entity">The Entity</param>
        /// <returns>The added Entity</returns>
        TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Adds a range of Entities
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entities">Range of Entities to be added</param>
        /// <returns>Added Entities</returns>
        IEnumerable<TEntity> Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// REmoves an Entity
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entity">Entity to be removed</param>
        /// <returns>The removed Entity</returns>
        TEntity Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Removes a range of Entities
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entities">Entities to be removed</param>
        /// <returns>REmoved entities</returns>
        IEnumerable<TEntity> Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Persists chenges against the DB
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Undoes changes if any errors occur
        /// </summary>
        void UndoChanges();
    }
}
