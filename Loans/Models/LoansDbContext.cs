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
                .HasKey(summary => new { summary.CreditorId, summary.DebtorId });

            builder.Entity<LoanSummary>()
                .HasOne(summary => summary.Creditor)
                .WithMany(borrower => borrower.Credits)
                .HasForeignKey(summary => summary.CreditorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoanSummary>()
                .HasOne(summary => summary.Debtor)
                .WithMany(borrower => borrower.Debts)
                .HasForeignKey(summary => summary.DebtorId)
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
