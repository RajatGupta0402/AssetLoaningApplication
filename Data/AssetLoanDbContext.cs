using AssetLoaningApplication.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace AssetLoaningApplication.Data
{
    public class AssetLoanDbContext : DbContext
    {

        public AssetLoanDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<TransactionDetails> TransactionDetails { get; set; }

        public DbSet<AssetDetails> AssetDetails { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        // AssetLoanDbContext class
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionDetails>()
                .HasOne(td => td.SupervisorDetails)
                .WithMany()
                .HasForeignKey(td => td.supervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactionDetails>()
                .HasOne(td => td.StudentDetails)
                .WithMany()
                .HasForeignKey(td => td.studentId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


    }
}
