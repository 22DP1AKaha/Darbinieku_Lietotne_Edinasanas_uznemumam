using Microsoft.EntityFrameworkCore;
using EIIOS.Models;

namespace EIIOS.Data
{
    public class EIIOSDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public EIIOSDbContext(DbContextOptions<EIIOSDbContext> options) : base(options)
        {
        }

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
        public DbSet<ImageModel> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
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

            ConfigureRelationships(modelBuilder);
            ConfigureConstraints(modelBuilder);
            SeedInitialData(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Image relationship - only for products
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Image)
                .WithMany(i => i.Products)
                .HasForeignKey(p => p.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

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

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllergenModel>().HasData(
                new AllergenModel { Id = 1, Code = "GLU", Name = "Gluten" },
                new AllergenModel { Id = 2, Code = "NUT", Name = "Nuts" },
                new AllergenModel { Id = 3, Code = "DUI", Name = "Dairy" },
                new AllergenModel { Id = 4, Code = "EGA", Name = "Eggs" },
                new AllergenModel { Id = 5, Code = "SOY", Name = "Soy" },
                new AllergenModel { Id = 6, Code = "FIS", Name = "Fish" },
                new AllergenModel { Id = 7, Code = "SHE", Name = "Shellfish" },
                new AllergenModel { Id = 8, Code = "CEL", Name = "Celery" },
                new AllergenModel { Id = 9, Code = "MUS", Name = "Mustard" },
                new AllergenModel { Id = 10, Code = "SES", Name = "Sesame Seeds" },
                new AllergenModel { Id = 11, Code = "LUP", Name = "Lupin" },
                new AllergenModel { Id = 12, Code = "SUL", Name = "Sulphites" }
            );

            modelBuilder.Entity<UserModel>().HasData(
                new UserModel
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@eiios.com",
                    PasswordHash = "1234",
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = UserRole.Administrator,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                });

            modelBuilder.Entity<CategoryModel>().HasData(
                new CategoryModel { Id = 1, Name = "Starters", Description = "Appetizers and small plates", DisplayOrder = 1, IsActive = true },
                new CategoryModel { Id = 2, Name = "Main Courses", Description = "Hearty and filling meals", DisplayOrder = 2, IsActive = true },
                new CategoryModel { Id = 3, Name = "Desserts", Description = "Sweet treats to finish your meal", DisplayOrder = 3, IsActive = true },
                new CategoryModel { Id = 4, Name = "Beverages", Description = "Drinks and refreshments", DisplayOrder = 4, IsActive = true }
            );

            modelBuilder.Entity<InventoryItemModel>().HasData(
                new InventoryItemModel { Id = 1, Name = "Chicken Breast", Unit = "kg", CurrentQuantity = 50, MinimumQuantity = 5, UnitCost = 6.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 2, Name = "Flour", Unit = "kg", CurrentQuantity = 100, MinimumQuantity = 10, UnitCost = 1.20m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 3, Name = "Milk", Unit = "l", CurrentQuantity = 30, MinimumQuantity = 5, UnitCost = 0.90m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 4, Name = "Eggs", Unit = "pieces", CurrentQuantity = 200, MinimumQuantity = 20, UnitCost = 0.15m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 5, Name = "Lamb Meat", Unit = "kg", CurrentQuantity = 40, MinimumQuantity = 5, UnitCost = 9.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 6, Name = "Beef Mince", Unit = "kg", CurrentQuantity = 30, MinimumQuantity = 5, UnitCost = 8.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 7, Name = "Pita Bread", Unit = "pieces", CurrentQuantity = 300, MinimumQuantity = 50, UnitCost = 0.40m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 8, Name = "Tortilla Wraps", Unit = "pieces", CurrentQuantity = 200, MinimumQuantity = 30, UnitCost = 0.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 9, Name = "Lettuce", Unit = "kg", CurrentQuantity = 15, MinimumQuantity = 2, UnitCost = 1.80m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 10, Name = "Tomatoes", Unit = "kg", CurrentQuantity = 20, MinimumQuantity = 3, UnitCost = 2.20m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 11, Name = "Onions", Unit = "kg", CurrentQuantity = 25, MinimumQuantity = 3, UnitCost = 1.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 12, Name = "Cabbage", Unit = "kg", CurrentQuantity = 10, MinimumQuantity = 2, UnitCost = 1.70m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 13, Name = "Cucumbers", Unit = "kg", CurrentQuantity = 12, MinimumQuantity = 2, UnitCost = 2.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 14, Name = "Yogurt", Unit = "kg", CurrentQuantity = 10, MinimumQuantity = 2, UnitCost = 3.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 15, Name = "Garlic", Unit = "kg", CurrentQuantity = 5, MinimumQuantity = 1, UnitCost = 4.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 16, Name = "Chili Sauce", Unit = "l", CurrentQuantity = 8, MinimumQuantity = 2, UnitCost = 5.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 17, Name = "Tahini Sauce", Unit = "l", CurrentQuantity = 6, MinimumQuantity = 1, UnitCost = 6.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 18, Name = "French Fries", Unit = "kg", CurrentQuantity = 25, MinimumQuantity = 5, UnitCost = 2.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 19, Name = "Olive Oil", Unit = "l", CurrentQuantity = 15, MinimumQuantity = 3, UnitCost = 7.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new InventoryItemModel { Id = 20, Name = "Spice Mix (Kebab Seasoning)", Unit = "kg", CurrentQuantity = 5, MinimumQuantity = 1, UnitCost = 12.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<ProductModel>().HasData(
                new ProductModel
                {
                    Id = 1,
                    Name = "Chicken Kebab Wrap",
                    Description = "Grilled chicken pieces served in a tortilla wrap with lettuce, tomatoes, onions, and garlic sauce.",
                    BasePrice = 7.50m,
                    PreparationTime = 10,
                    IsAvailable = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedById = 1
                },
                new ProductModel
                {
                    Id = 2,
                    Name = "Lamb Doner Kebab",
                    Description = "Thin slices of seasoned lamb served in pita bread with fresh vegetables and chili sauce.",
                    BasePrice = 8.50m,
                    PreparationTime = 12,
                    IsAvailable = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedById = 1
                },
                new ProductModel
                {
                    Id = 3,
                    Name = "Beef Kofta Kebab",
                    Description = "Spiced beef mince skewers served with fries and tahini sauce.",
                    BasePrice = 9.00m,
                    PreparationTime = 15,
                    IsAvailable = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedById = 1
                },
                new ProductModel
                {
                    Id = 4,
                    Name = "Falafel Wrap",
                    Description = "Crispy falafel balls with lettuce, tomatoes, cucumbers, and tahini sauce in a tortilla wrap.",
                    BasePrice = 6.50m,
                    PreparationTime = 8,
                    IsAvailable = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedById = 1
                },
                new ProductModel
                {
                    Id = 5,
                    Name = "Chicken & Fries Box",
                    Description = "Grilled chicken pieces served over a bed of crispy fries with garlic sauce.",
                    BasePrice = 7.00m,
                    PreparationTime = 10,
                    IsAvailable = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedById = 1
                }
            );

            modelBuilder.Entity<ProductCategoryModel>().HasData(
                new ProductCategoryModel { ProductId = 1, CategoryId = 2 },
                new ProductCategoryModel { ProductId = 2, CategoryId = 2 },
                new ProductCategoryModel { ProductId = 3, CategoryId = 2 },
                new ProductCategoryModel { ProductId = 4, CategoryId = 2 },
                new ProductCategoryModel { ProductId = 5, CategoryId = 2 }
            );

            modelBuilder.Entity<ProductAllergenModel>().HasData(
                new ProductAllergenModel { ProductId = 1, AllergenId = 1 },
                new ProductAllergenModel { ProductId = 1, AllergenId = 3 },
                new ProductAllergenModel { ProductId = 1, AllergenId = 4 },
                new ProductAllergenModel { ProductId = 2, AllergenId = 1 },
                new ProductAllergenModel { ProductId = 2, AllergenId = 3 },
                new ProductAllergenModel { ProductId = 2, AllergenId = 4 },
                new ProductAllergenModel { ProductId = 3, AllergenId = 1 },
                new ProductAllergenModel { ProductId = 3, AllergenId = 10 },
                new ProductAllergenModel { ProductId = 4, AllergenId = 1 },
                new ProductAllergenModel { ProductId = 4, AllergenId = 10 },
                new ProductAllergenModel { ProductId = 5, AllergenId = 3 },
                new ProductAllergenModel { ProductId = 5, AllergenId = 4 }
            );

            modelBuilder.Entity<ProductIngredientModel>().HasData(
                new ProductIngredientModel { Id = 1, ProductId = 1, InventoryItemId = 1, QuantityNeeded = 0.20m, Unit = "kg" },
                new ProductIngredientModel { Id = 2, ProductId = 1, InventoryItemId = 8, QuantityNeeded = 1, Unit = "pieces" },
                new ProductIngredientModel { Id = 3, ProductId = 1, InventoryItemId = 9, QuantityNeeded = 0.05m, Unit = "kg" },
                new ProductIngredientModel { Id = 4, ProductId = 1, InventoryItemId = 10, QuantityNeeded = 0.05m, Unit = "kg" },
                new ProductIngredientModel { Id = 5, ProductId = 1, InventoryItemId = 11, QuantityNeeded = 0.02m, Unit = "kg" },
                new ProductIngredientModel { Id = 6, ProductId = 1, InventoryItemId = 15, QuantityNeeded = 0.01m, Unit = "kg" },
                new ProductIngredientModel { Id = 7, ProductId = 2, InventoryItemId = 5, QuantityNeeded = 0.25m, Unit = "kg" },
                new ProductIngredientModel { Id = 8, ProductId = 2, InventoryItemId = 7, QuantityNeeded = 1, Unit = "pieces" },
                new ProductIngredientModel { Id = 9, ProductId = 2, InventoryItemId = 9, QuantityNeeded = 0.05m, Unit = "kg" },
                new ProductIngredientModel { Id = 10, ProductId = 2, InventoryItemId = 10, QuantityNeeded = 0.05m, Unit = "kg" },
                new ProductIngredientModel { Id = 11, ProductId = 2, InventoryItemId = 11, QuantityNeeded = 0.02m, Unit = "kg" },
                new ProductIngredientModel { Id = 12, ProductId = 2, InventoryItemId = 16, QuantityNeeded = 0.02m, Unit = "l" },
                new ProductIngredientModel { Id = 13, ProductId = 3, InventoryItemId = 6, QuantityNeeded = 0.25m, Unit = "kg" },
                new ProductIngredientModel { Id = 14, ProductId = 3, InventoryItemId = 20, QuantityNeeded = 0.01m, Unit = "kg" },
                new ProductIngredientModel { Id = 15, ProductId = 3, InventoryItemId = 18, QuantityNeeded = 0.15m, Unit = "kg" },
                new ProductIngredientModel { Id = 16, ProductId = 3, InventoryItemId = 17, QuantityNeeded = 0.02m, Unit = "l" },
                new ProductIngredientModel { Id = 17, ProductId = 4, InventoryItemId = 8, QuantityNeeded = 1, Unit = "pieces" },
                new ProductIngredientModel { Id = 18, ProductId = 4, InventoryItemId = 9, QuantityNeeded = 0.05m, Unit = "kg" },
                new ProductIngredientModel { Id = 19, ProductId = 4, InventoryItemId = 10, QuantityNeeded = 0.05m, Unit = "kg" },
                new ProductIngredientModel { Id = 20, ProductId = 4, InventoryItemId = 17, QuantityNeeded = 0.02m, Unit = "l" },

    new ProductIngredientModel { Id = 22, ProductId = 5, InventoryItemId = 1, QuantityNeeded = 0.20m, Unit = "kg" },
    new ProductIngredientModel { Id = 23, ProductId = 5, InventoryItemId = 18, QuantityNeeded = 0.20m, Unit = "kg" },
    new ProductIngredientModel { Id = 24, ProductId = 5, InventoryItemId = 15, QuantityNeeded = 0.01m, Unit = "kg" } 
);
        }

    }
}