using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SubsidySystem.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Citizen> Citizens { get; set; }
        public virtual DbSet<CompensationCalculation> CompensationCalculations { get; set; }
        public virtual DbSet<FamilyMember> FamilyMembers { get; set; }
        public virtual DbSet<Income> Incomes { get; set; }
        public virtual DbSet<IncomeType> IncomeTypes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentRegistry> PaymentRegistries { get; set; }
        public virtual DbSet<Standard> Standards { get; set; }
        public virtual DbSet<SubsidyCalculation> SubsidyCalculations { get; set; }
        public virtual DbSet<UtilityCharge> UtilityCharges { get; set; }
        public virtual DbSet<UtilityServiceType> UtilityServiceTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in entries)
            {
                var properties = entry.Properties.ToList();

                foreach (var property in properties)
                {
                    var currentValue = property.CurrentValue;

                    if (currentValue is DateTime dt)
                    {
                        if (dt.Kind != DateTimeKind.Utc)
                        {
                            property.CurrentValue = new DateTime(dt.Ticks, DateTimeKind.Utc);
                        }
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}