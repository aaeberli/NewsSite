using System.Collections.Generic;

namespace NewsSite.Common.Abstract
{
    public interface IMapperAdapter
    {
        TDestination Map<TSource, TDestination>(TSource source);
        IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source);
    }
}
