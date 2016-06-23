using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Defines the structure of the repositories
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity</typeparam>
    public interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        /// <summary>
        /// Create a new Entity for insertion
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <returns>The created Entity</returns>
        TEntity Create();

        /// <summary>
        /// Adds an Entity
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entity">The Entity</param>
        /// <returns>The added Entity</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Adds a range of Entities
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entities">Range of Entities to be added</param>
        /// <returns>Added Entities</returns>
        IEnumerable<TEntity> Add(IEnumerable<TEntity> entities);

        /// <summary>
        /// Gets a list of Entities
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <returns>List of Entities</returns>
        ICollection<TEntity> Read();

        /// <summary>
        /// Gets a filtered list of Entities
        /// </summary>
        /// <param name="filter">Filter represented by a lambda expression</param>
        /// <returns>List of filtered Entities</returns>
        IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// REmoves an Entity
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entity">Entity to be removed</param>
        /// <returns>The removed Entity</returns>
        TEntity Remove(TEntity entity);

        /// <summary>
        /// Removes a range of Entities
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity</typeparam>
        /// <param name="entities">Entities to be removed</param>
        /// <returns>REmoved entities</returns>
        IEnumerable<TEntity> Remove(IEnumerable<TEntity> entities);

        /// <summary>
        /// Persists chenges against the DB
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Performs the equivalent of the Linq SingleOrDefault operation
        /// </summary>
        /// <param name="filter">Filter represented by a lambda expression</param>
        /// <returns>Found object or null</returns>
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter);
        void UndoChanges();
    }

}
