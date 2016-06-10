namespace NewsSite.Domain.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class NewsSiteDbDataAccess : DbContext
    {
        public NewsSiteDbDataAccess()
            : base("name=NewsSiteDb")
        {
        }

        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<News>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<News>()
                .Property(e => e.Body)
                .IsUnicode(false);

            modelBuilder.Entity<News>()
                .HasMany(e => e.Likes)
                .WithRequired(e => e.News)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Likes)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.News)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<UserType>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserType)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);
        }
    }
}
