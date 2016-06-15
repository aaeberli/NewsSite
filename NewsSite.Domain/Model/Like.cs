namespace NewsSite.Domain.Model
{
    using Common.Abstract;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Like")]
    public partial class Like: BaseEntity
    {
        public int Id { get; set; }

        public int NewsId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual News News { get; set; }
    }
}
