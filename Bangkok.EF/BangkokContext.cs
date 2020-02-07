using Microsoft.EntityFrameworkCore;
using Bangkok.Models;

namespace Bangkok.EF
{
    public class BangkokContext : DbContext
    {
        public BangkokContext(DbContextOptions<BangkokContext> options) : base(options){ }

        public virtual DbSet<EnumStatus> EnumStatus { get; set; }
        public virtual DbSet<TransactionData> TransactionData { get; set; }
        public virtual DbSet<LogData> LogData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.EnableSensitiveDataLogging(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnumStatus>(entity =>             
            {
                entity.ToTable("enum.Status");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnType("tinyint")
                    .ValueGeneratedNever();
                entity.Property(e => e.StatusInitial)
                    .HasColumnType("nvarchar(1)");
            });
            modelBuilder.Entity<TransactionData>(entity => 
            {
                entity.ToTable("Transaction.Data");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID);
                entity.Property(e => e.Amount)
                    .HasColumnType("decimal");
                entity.Property(e => e.CurrencyCode)
                    .HasColumnType("nvarchar(10)");
                entity.Property(e => e.TransactionDT);
                entity.Property(e => e.Status)
                    .HasColumnType("tinyint");

                entity.HasOne(e => e.EnumStatus)
                    .WithOne()
                    .HasForeignKey<TransactionData>(e => e.Status)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transaction.Data_enum.Status");
            });
            modelBuilder.Entity<LogData>(entity => 
            {
                entity.ToTable("Log.Data");
                entity.Property(e => e.ID);
                entity.Property(e => e.ErrorType)
                    .HasColumnType("varchar(255)");
                entity.Property(e => e.Description)
                    .HasColumnType("varchar(max)");
            });
        }
    }
}
