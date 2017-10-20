using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loans.Models
{
    public class LoansContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Community> Communities { get; set; }
        public DbSet<CommunityEnrollment> CommunitiesEnrollments { get; set; }

        public DbSet<Requisite> Requisites { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<LoanSummary> LoanSummaries { get; set; }

        public LoansContext(DbContextOptions<LoansContext> options)
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

            builder.Entity<CommunityEnrollment>().HasKey(enrollment => new
            {
                enrollment.UserId,
                GroupId = enrollment.CommunityId
            });
        }
    }
}
