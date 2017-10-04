using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loans.Models
{
    public class LoansDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<UsersGroup> UsersGroups { get; set; }
        public DbSet<Requisite> Requisites { get; set; }
        public DbSet<Loan> Loans { get; set; }

        public LoansDbContext(DbContextOptions<LoansDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LoanSummary>()
                .HasKey(summary => new { summary.FromId, summary.ToId });

            builder.Entity<LoanSummary>()
                .HasOne(summary => summary.From)
                .WithMany(borrower => borrower.To)
                .HasForeignKey(summary => summary.FromId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoanSummary>()
                .HasOne(summary => summary.To)
                .WithMany(borrower => borrower.From)
                .HasForeignKey(summary => summary.ToId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsersGroupEnrollment>().HasKey(enrollment => new
            {
                enrollment.UserId,
                enrollment.GroupId
            });
        }
    }
}
