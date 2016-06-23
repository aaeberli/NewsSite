using System.Collections.Generic;

namespace NewsSite.Common.Abstract
{
    /// <summary>
    /// Defines the structure of an object-toobject mapper
    /// </summary>
    public interface IMapperAdapter
    {
        /// <summary>
        /// Maps an object
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>Mapped object</returns>
        TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class;

        /// <summary>
        /// Maps a collection of objects
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source collection</param>
        /// <returns>Mapped colection</returns>
        IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source)
            where TSource : class
            where TDestination : class;
    }
}
