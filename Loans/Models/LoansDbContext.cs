using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loans.Models
{
    public class LoansDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<UsersGroup> UsersGroups { get; set; }
        public DbSet<Requisite> Requisites { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanSummary> LoanSummaries { get; set; }

        public LoansDbContext(DbContextOptions<LoansDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LoanSummary>()
                .HasKey(summary => new { FromId = summary.CreditorId, ToId = summary.BorrowerId });

            builder.Entity<LoanSummary>()
                .HasOne(summary => summary.Creditor)
                .WithMany(borrower => borrower.Borrowers)
                .HasForeignKey(summary => summary.CreditorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoanSummary>()
                .HasOne(summary => summary.Borrower)
                .WithMany(borrower => borrower.Creditors)
                .HasForeignKey(summary => summary.BorrowerId)
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
