using Microsoft.EntityFrameworkCore;
using EIIOS.Models;

namespace EIIOS.Data
{
    public class EIIOSDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        // Constructor for dependency injection
        public EIIOSDbContext(DbContextOptions<EIIOSDbContext> options) : base(options)
        {
        }

        // Parameterless constructor for migrations
        public EIIOSDbContext() : base()
        {
        }

        // DbSets for all your models
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductCategoryModel> ProductCategories { get; set; }
        public DbSet<AllergenModel> Allergens { get; set; }
        public DbSet<ProductAllergenModel> ProductAllergens { get; set; }
        public DbSet<InventoryItemModel> InventoryItems { get; set; }
        public DbSet<StockEntryModel> StockEntries { get; set; }
        public DbSet<ProductIngredientModel> ProductIngredients { get; set; }
        public DbSet<DailyMenuModel> DailyMenus { get; set; }
        public DbSet<DailyMenuItemModel> DailyMenuItems { get; set; }
        public DbSet<DiscountModel> Discounts { get; set; }
        public DbSet<DiscountProductModel> DiscountProducts { get; set; }
        public DbSet<TimeEntryModel> TimeEntries { get; set; }

        // Configuration for migrations (when no DI available)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Replace with your actual connection string
                optionsBuilder.UseMySql(
                    "Server=localhost;Port=3306;Database=EIIOS;Uid=root;Pwd=your_password;",
                    new MySqlServerVersion(new Version(9, 3, 0))
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite keys for junction tables
            modelBuilder.Entity<ProductCategoryModel>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductAllergenModel>()
                .HasKey(pa => new { pa.ProductId, pa.AllergenId });

            modelBuilder.Entity<DiscountProductModel>()
                .HasKey(dp => new { dp.DiscountId, dp.ProductId });

            // Configure unique indexes
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AllergenModel>()
                .HasIndex(a => a.Code)
                .IsUnique();

            modelBuilder.Entity<DailyMenuModel>()
                .HasIndex(dm => dm.MenuDate)
                .IsUnique();

            // Configure enum conversions
            modelBuilder.Entity<UserModel>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<DiscountModel>()
                .Property(d => d.DiscountType)
                .HasConversion<string>();

            modelBuilder.Entity<TimeEntryModel>()
                .Property(te => te.Status)
                .HasConversion<string>();

            // Configure relationships and constraints
            ConfigureRelationships(modelBuilder);
            ConfigureConstraints(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Product relationships
            modelBuilder.Entity<ProductCategoryModel>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategoryModel>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductAllergenModel>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAllergens)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductAllergenModel>()
                .HasOne(pa => pa.Allergen)
                .WithMany(a => a.ProductAllergens)
                .HasForeignKey(pa => pa.AllergenId)
                .OnDelete(DeleteBehavior.Restrict);

            // User creation relationships
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.CreatedBy)
                .WithMany(u => u.CreatedProducts)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DailyMenuModel>()
                .HasOne(dm => dm.CreatedBy)
                .WithMany(u => u.CreatedDailyMenus)
                .HasForeignKey(dm => dm.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StockEntryModel>()
                .HasOne(se => se.CreatedBy)
                .WithMany(u => u.CreatedStockEntries)
                .HasForeignKey(se => se.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DiscountModel>()
                .HasOne(d => d.CreatedBy)
                .WithMany(u => u.CreatedDiscounts)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            // Time entry relationships
            modelBuilder.Entity<TimeEntryModel>()
                .HasOne(te => te.Employee)
                .WithMany(u => u.TimeEntries)
                .HasForeignKey(te => te.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimeEntryModel>()
                .HasOne(te => te.ApprovedBy)
                .WithMany(u => u.ApprovedTimeEntries)
                .HasForeignKey(te => te.ApprovedById)
                .OnDelete(DeleteBehavior.SetNull);

            // Stock entry relationships
            modelBuilder.Entity<StockEntryModel>()
                .HasOne(se => se.InventoryItem)
                .WithMany(ii => ii.StockEntries)
                .HasForeignKey(se => se.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product ingredient relationships
            modelBuilder.Entity<ProductIngredientModel>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductIngredientModel>()
                .HasOne(pi => pi.InventoryItem)
                .WithMany(ii => ii.ProductIngredients)
                .HasForeignKey(pi => pi.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Daily menu relationships
            modelBuilder.Entity<DailyMenuItemModel>()
                .HasOne(dmi => dmi.DailyMenu)
                .WithMany(dm => dm.DailyMenuItems)
                .HasForeignKey(dmi => dmi.DailyMenuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DailyMenuItemModel>()
                .HasOne(dmi => dmi.Product)
                .WithMany(p => p.DailyMenuItems)
                .HasForeignKey(dmi => dmi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Discount relationships
            modelBuilder.Entity<DiscountProductModel>()
                .HasOne(dp => dp.Discount)
                .WithMany(d => d.DiscountProducts)
                .HasForeignKey(dp => dp.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountProductModel>()
                .HasOne(dp => dp.Product)
                .WithMany(p => p.DiscountProducts)
                .HasForeignKey(dp => dp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureConstraints(ModelBuilder modelBuilder)
        {
            // Decimal precision settings
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.BasePrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DailyMenuItemModel>()
                .Property(dmi => dmi.SpecialPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DiscountModel>()
                .Property(d => d.DiscountValue)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InventoryItemModel>()
                .Property(ii => ii.CurrentQuantity)
                .HasPrecision(10, 3);

            modelBuilder.Entity<InventoryItemModel>()
                .Property(ii => ii.MinimumQuantity)
                .HasPrecision(10, 3);

            modelBuilder.Entity<InventoryItemModel>()
                .Property(ii => ii.UnitCost)
                .HasPrecision(10, 2);

            modelBuilder.Entity<StockEntryModel>()
                .Property(se => se.Quantity)
                .HasPrecision(10, 3);

            modelBuilder.Entity<ProductIngredientModel>()
                .Property(pi => pi.QuantityNeeded)
                .HasPrecision(10, 3);

            modelBuilder.Entity<TimeEntryModel>()
                .Property(te => te.TotalHours)
                .HasPrecision(5, 2);
        }

        // Automatic timestamp updates
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    if (entityEntry.Entity is UserModel user)
                        user.UpdatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is ProductModel product)
                        product.UpdatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is InventoryItemModel inventory)
                        inventory.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}