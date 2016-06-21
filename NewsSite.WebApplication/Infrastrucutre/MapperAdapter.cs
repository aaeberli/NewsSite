﻿using AutoMapper;
using NewsSite.Common.Abstract;
using NewsSite.Domain.Model;
using NewsSite.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsSite.WebApplication.Infrastrucutre
{
    public class MapperAdapter : IMapperAdapter
    {
        public MapperAdapter()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ArticleViewModel, Article>();
                cfg.CreateMap<Article, ArticleViewModel>()
                    .ForMember(dest => dest.Likers, opt => opt.MapFrom(source => source.Likes.Select(l => l.AspNetUser.Id)))
                    .ForMember(dest => dest.CurrentUserLike, opt => opt.Ignore())
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(source => source.AspNetUser.UserName));
            });
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source.ToList());
        }
    }
}