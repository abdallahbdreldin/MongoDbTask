using MongoDB.Bson;
using MongoDB.Driver;
using TodayWebAPi.DAL.Data.Entities;
namespace TodayWebAPi.DAL.Data.Context
{
    public class StoreContext
    {
        private readonly IMongoDatabase _database;

        public StoreContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }

        //public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        //public IMongoCollection<ProductBrand> Brands => _database.GetCollection<ProductBrand>("Brands");
        //public IMongoCollection<ProductType> Types => _database.GetCollection<ProductType>("Types");
        //public IMongoCollection<CustomerBasket> CustomerBaskets => _database.GetCollection<CustomerBasket>("CustomerBaskets");
        //public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        //public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("OrderItems");
        //public IMongoCollection<DeliveryMethod> DeliveryMethods => _database.GetCollection<DeliveryMethod>("DeliveryMethods");

        public async Task InitializeAsync()
        {
            Console.WriteLine($"Initializing database...");
            try
            {
                var client = new MongoClient(_database.Client.Settings);
                var server = await client.ListDatabaseNamesAsync();
                Console.WriteLine("Successfully connected to MongoDB Atlas");

                await SeedDataAsync();
                Console.WriteLine("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }

        public async Task SeedDataAsync()
        {
            try
            {
                Console.WriteLine("Starting seeding...");

                var productCount = await GetCollection<Product>().CountDocumentsAsync(_ => true);
                Console.WriteLine($"Products count: {productCount}");
                if (productCount > 0)
                {
                    Console.WriteLine("Skipping seeding due to existing Products.");
                    return;
                }

                await SeedDeliveryMethodsAsync();
                var brandIds = await SeedBrandsAsync();
                var typeIds = await SeedTypesAsync();
                await SeedProductsAsync(brandIds, typeIds);

                Console.WriteLine("Seeding completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding failed: {ex.Message}");
                throw;
            }
        }

        private async Task SeedDeliveryMethodsAsync()
        {
            Console.WriteLine("Seeding DeliveryMethods...");
            var deliveryMethods = new List<DeliveryMethod>
        {
            new DeliveryMethod
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ShortName = "Standard",
                DeliveryTime = "5-7 Days",
                Description = "Standard shipping method",
                Price = 20m
            },
            new DeliveryMethod
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ShortName = "Express",
                DeliveryTime = "2-3 Days",
                Description = "Faster shipping",
                Price = 50m
            },
            new DeliveryMethod
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ShortName = "Overnight",
                DeliveryTime = "1 Day",
                Description = "Next-day delivery",
                Price = 100m
            }
        };

            await GetCollection<DeliveryMethod>().InsertManyAsync(deliveryMethods);
        }

        private async Task<Dictionary<string, string>> SeedBrandsAsync()
        {
            Console.WriteLine("Seeding ProductBrands...");
            var brandList = new List<ProductBrand>
        {
                new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Apple" },
                new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Samsung" },
                new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Sony" },
                new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Dell" },
                new() { Id = ObjectId.GenerateNewId().ToString(), Name = "HP" },
                new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Bose" }
        };

            await GetCollection<ProductBrand>().InsertManyAsync(brandList);

            return brandList.ToDictionary(b => b.Name, b => new string(b.Id));
        }

        private async Task<Dictionary<string, string>> SeedTypesAsync()
        {
            Console.WriteLine("Seeding ProductTypes...");
            var typeList = new List<ProductType>
        {
            new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Smartphone" },
            new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Tablet" },
            new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Laptop" },
            new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Smartwatch" },
            new() { Id = ObjectId.GenerateNewId().ToString(), Name = "Headphones" }
        };

            await GetCollection<ProductType>().InsertManyAsync(typeList);

            return typeList.ToDictionary(t => t.Name, t => new string(t.Id));
        }

        private async Task SeedProductsAsync(Dictionary<string, string> brandIds, Dictionary<string, string> typeIds)
        {
            Console.WriteLine("Seeding Products...");
            var products = new List<Product>
        {
            new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "iPhone 15",
            Description = "Latest Apple smartphone with A16 chip",
            Price = 999.99m,
            PictureUrl = "images/iphone15.jpg",
            InStock = 50,
            BrandId = brandIds["Apple"],
            TypeId = typeIds["Smartphone"],
        },
            new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Samsung Galaxy S24",
            Description = "Flagship Samsung smartphone with high-end features",
            Price = 899.99m,
            PictureUrl = "images/galaxys24.jpg",
            InStock = 30,
            BrandId = brandIds["Samsung"],
            TypeId = typeIds["Smartphone"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Sony WH-1000XM5",
            Description = "High-quality noise-canceling headphones from Sony",
            Price = 349.99m,
            PictureUrl = "images/sony-headphones.jpg",
            InStock = 20,
            BrandId = brandIds["Sony"],
            TypeId = typeIds["Headphones"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Dell XPS 15",
            Description = "Powerful laptop with stunning display",
            Price = 1599.99m,
            PictureUrl = "images/dell-xps15.jpg",
            InStock = 15,
            BrandId = brandIds["Dell"],
            TypeId = typeIds["Laptop"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "HP Spectre x360",
            Description = "Convertible laptop with sleek design",
            Price = 1399.99m,
            PictureUrl = "images/hp-spectre.jpg",
            InStock = 25,
            BrandId = brandIds["HP"],
            TypeId = typeIds["Laptop"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Bose QuietComfort 45",
            Description = "Premium noise-canceling headphones with immersive sound",
            Price = 329.99m,
            PictureUrl = "images/bose-qc45.jpg",
            InStock = 10,
            BrandId = brandIds["Bose"],
            TypeId = typeIds["Headphones"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "iPad Pro 12.9",
            Description = "Apple's most powerful tablet with M2 chip",
            Price = 1099.99m,
            PictureUrl = "images/ipad-pro.jpg",
            InStock = 12,
            BrandId = brandIds["Apple"],
            TypeId = typeIds["Tablet"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Samsung Galaxy Tab S9",
            Description = "Premium Android tablet with AMOLED display",
            Price = 799.99m,
            PictureUrl = "images/galaxy-tab-s9.jpg",
            InStock = 18,
            BrandId = brandIds["Samsung"],
            TypeId = typeIds["Tablet"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Apple Watch Series 9",
            Description = "Latest Apple smartwatch with fitness tracking",
            Price = 499.99m,
            PictureUrl = "images/apple-watch9.jpg",
            InStock = 35,
            BrandId = brandIds["Apple"],
            TypeId = typeIds["Smartwatch"],
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Samsung Galaxy Watch 6",
            Description = "Advanced smartwatch with health monitoring features",
            Price = 449.99m,
            PictureUrl = "images/galaxy-watch6.jpg",
            InStock = 40,
            BrandId = brandIds["Samsung"],
            TypeId = typeIds["Smartwatch"],
        }
    };

            await GetCollection<Product>().InsertManyAsync(products);
            Console.WriteLine($"Seeded {products.Count} Products.");
        }


        //public async Task SeedDataAsync()
        //{
        //    try
        //    {
        //        Console.WriteLine("Starting seeding...");

        //        var productCount = await Products.CountDocumentsAsync(_ => true);
        //        Console.WriteLine($"Products count: {productCount}");
        //        if (productCount > 0)
        //        {
        //            Console.WriteLine("Skipping seeding due to existing Products.");
        //            return;
        //        }

        //        Console.WriteLine("Seeding DeliveryMethods...");
        //        var deliveryMethods = new List<DeliveryMethod>
        //        {
        //            new DeliveryMethod
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                ShortName = "Standard",
        //                DeliveryTime = "5-7 Days",
        //                Description = "Standard shipping method",
        //                Price = 20m
        //            },
        //            new DeliveryMethod
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                ShortName = "Express",
        //                DeliveryTime = "2-3 Days",
        //                Description = "Faster shipping",
        //                Price = 50m
        //            },
        //            new DeliveryMethod
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                ShortName = "Overnight",
        //                DeliveryTime = "1 Day",
        //                Description = "Next-day delivery",
        //                Price = 100m
        //            }
        //        };
        //        await DeliveryMethods.InsertManyAsync(deliveryMethods);

        //        var products = new List<Product>
        //        {
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "iPhone 15",
        //                Description = "Latest Apple smartphone with A16 chip",
        //                Price = 999.99m,
        //                PictureUrl = "images/iphone15.jpg",
        //                InStock = 50,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Samsung Galaxy S24",
        //                Description = "Flagship Samsung smartphone with high-end features",
        //                Price = 899.99m,
        //                PictureUrl = "images/galaxys24.jpg",
        //                InStock = 30,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Sony WH-1000XM5",
        //                Description = "High-quality noise-canceling headphones from Sony",
        //                Price = 349.99m,
        //                PictureUrl = "images/sony-headphones.jpg",
        //                InStock = 20,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Dell XPS 15",
        //                Description = "Powerful laptop with stunning display",
        //                Price = 1599.99m,
        //                PictureUrl = "images/dell-xps15.jpg",
        //                InStock = 15,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "HP Spectre x360",
        //                Description = "Convertible laptop with sleek design",
        //                Price = 1399.99m,
        //                PictureUrl = "images/hp-spectre.jpg",
        //                InStock = 25,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Bose QuietComfort 45",
        //                Description = "Premium noise-canceling headphones with immersive sound",
        //                Price = 329.99m,
        //                PictureUrl = "images/bose-qc45.jpg",
        //                InStock = 10,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "iPad Pro 12.9",
        //                Description = "Apple's most powerful tablet with M2 chip",
        //                Price = 1099.99m,
        //                PictureUrl = "images/ipad-pro.jpg",
        //                InStock = 12,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Samsung Galaxy Tab S9",
        //                Description = "Premium Android tablet with AMOLED display",
        //                Price = 799.99m,
        //                PictureUrl = "images/galaxy-tab-s9.jpg",
        //                InStock = 18,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Apple Watch Series 9",
        //                Description = "Latest Apple smartwatch with fitness tracking",
        //                Price = 499.99m,
        //                PictureUrl = "images/apple-watch9.jpg",
        //                InStock = 35,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            },
        //            new Product
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //                Name = "Samsung Galaxy Watch 6",
        //                Description = "Advanced smartwatch with health monitoring features",
        //                Price = 449.99m,
        //                PictureUrl = "images/galaxy-watch6.jpg",
        //                InStock = 40,
        //                BrandId = ObjectId.GenerateNewId(),
        //                TypeId = ObjectId.GenerateNewId(),
        //            }
        //        };
        //        await Products.InsertManyAsync(products);
        //        Console.WriteLine($"Seeded {products.Count} Products.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Seeding failed: {ex.Message}");
        //        throw;
        //    }
        //}
    }
}