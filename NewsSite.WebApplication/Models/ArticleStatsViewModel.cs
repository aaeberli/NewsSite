using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.WebApplication.Models
{
    // Models returned by AccountController actions.
    public class ArticlesStatsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Likes { get; set; }
    }
}
