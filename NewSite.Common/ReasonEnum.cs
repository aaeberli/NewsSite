﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common
{
    /// <summary>
    /// Enumerates possible failures in checking Business Rules
    /// </summary>
    public enum ReasonEnum
    {
        Generic,
        Error,
        MaxLikes,
        NoUSer,
        NoArticle,
        NoPublisher,
        EmptyTitle,
        NoLike,
        WrongUser,
        WrongArticle,
        AlreadyLiked,
        EmptyBody,
        TitleTooLong,
    }
}
