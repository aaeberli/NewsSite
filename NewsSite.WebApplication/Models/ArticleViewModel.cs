using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.WebApplication.Models
{

    public class ArticleViewModel
    {
        [Required(AllowEmptyStrings =false)]
        [StringLength(50, ErrorMessage = "The {0} must be maximum {1} characters long.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Body")]
        public string Body { get; set; }

        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(128)]
        public string AuthorId { get; set; }

        public string Author { get; set; }

        public Dictionary<string, int> Likers { get; set; }

        public bool CurrentUserLike { get; set; }

        public int CurrentUserLikeId { get; set; }

        public string CurrentUserId { get; set; }

    }
}