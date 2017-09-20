using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loans.Models
{
    class LoansDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
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

            builder.Entity<Loan>().HasKey(loan => new
            {
                loan.BorrowerId,
                loan.DebtorId
            });

            builder.Entity<Loan>()
                .HasOne(loan => loan.Debtor)
                .WithMany(deptor => deptor.Borrowers)
                .HasForeignKey(loan => loan.DebtorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Loan>()
                .HasOne(loan => loan.Borrower)
                .WithMany(borrower => borrower.Debtors)
                .HasForeignKey(loan => loan.BorrowerId)
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
