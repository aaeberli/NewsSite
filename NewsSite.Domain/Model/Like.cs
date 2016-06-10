namespace NewsSite.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Like")]
    public partial class Like
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int NewsId { get; set; }

        public virtual News News { get; set; }

        public virtual User User { get; set; }
    }
}
