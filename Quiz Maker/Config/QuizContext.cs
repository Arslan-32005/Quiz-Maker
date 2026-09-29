using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quiz_Maker.Models;

namespace Quiz_Maker.Config
{
    public class QuizContext: IdentityDbContext<ApplicationUser>
    {
        public QuizContext(DbContextOptions<QuizContext> options) : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // uniqueindex on sharecode
            builder.Entity<Quiz>()
                .HasIndex(q => q.ShareCode)
                .IsUnique();
            // quiz to question relationship one to many
            builder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            // question to option relationship one to many
            builder.Entity<Option>()
                .HasOne(opt => opt.Question)
                .WithMany(o => o.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            // quiz to attempt relationship one to many
            builder.Entity<Attempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            // quiz to user relationship many to one
            builder.Entity<Quiz>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Quizzes)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // attempt to user relationship many to one
            builder.Entity<Attempt>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Attempts)
                .HasForeignKey(a => a.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            // composite index on option questionid and order
            builder.Entity<Option>()
                .HasIndex(o => new { o.QuestionId, o.Order })
                .IsUnique();
            // composite index on question quizid and order
            builder.Entity<Question>()
                .HasIndex(q => new { q.QuizId, q.Order })
                .IsUnique();
        }
    }
    
}
