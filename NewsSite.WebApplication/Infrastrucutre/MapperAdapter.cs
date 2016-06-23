using AutoMapper;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using NewsSite.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsSite.WebApplication.Infrastrucutre
{
    /// <summary>
    /// Mapper Adapter encapsulating Automapper
    /// </summary>
    public class MapperAdapter : IMapperAdapter
    {
        /// <summary>
        /// MApping configuration
        /// </summary>
        public MapperAdapter()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ArticleViewModel, Article>();
                cfg.CreateMap<Article, ArticleViewModel>()
                    .ForMember(dest => dest.Likers, opt => opt.MapFrom(source => source.Likes.ToDictionary(l => l.AspNetUser.Id, l => l.Id)))
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.AspNetUser.UserName))
                    .ForMember(dest => dest.CurrentUserLike, opt => opt.Ignore())
                    .ForMember(dest => dest.CurrentUserLikeId, opt => opt.Ignore())
                    .ForMember(dest => dest.CurrentUserId, opt => opt.Ignore());
            });
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return null;
            return Mapper.Map<TSource, TDestination>(source);
        }

        public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return null;
            return Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source.ToList());
        }
    }
}