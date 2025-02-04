using System;
using System.Linq;
using System.Text.Json;
using serverProduct.Contexts;
using serverProduct.Models;

namespace ServerApp
{
    public class DatabaseHandler
    {
        public string GetCategories()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var categories = context.Categories
                        .Select(c => new { c.Id, c.Name })
                        .ToList();

                    return JsonSerializer.Serialize(categories);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
                return "[]";
            }
        }

        public string GetAllProducts()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var products = context.Products
                        .Select(p => new { p.Id,p.Name, p.Price,p.CatId })
                        .ToList();

                    return JsonSerializer.Serialize(products);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return "[]";
            }
        }

        public string GetProductsByCategory(int categoryId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var products = context.Products
                        .Where(p => p.CatId == categoryId)
                        .Select(p => new { p.Id, p.Name, p.Price, p.CatId })
                        .ToList();

                    return JsonSerializer.Serialize(products);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return "[]";
            }
        }

        public string AddProduct(string name, decimal price, int categoryId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var product = new Product
                    {
                        Name = name,
                        Price = price,
                        CatId = categoryId
                    };
                    context.Products.Add(product);
                    context.SaveChanges();
                    return JsonSerializer.Serialize(new { Success = true, Message = "Product added successfully!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                return JsonSerializer.Serialize(new { Success = false, Message = "Failed to add product." });
            }
        }

        public static string EditProduct(int id, string name, decimal price, int catId)
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Id == id);
                if (product != null)
                {
                    product.Name = name;
                    product.Price = price;
                    product.CatId = catId;
                    db.SaveChanges();
                    return "SUCCESS: Product updated.";
                }
                return "FAIL: Product not found.";
            }
        }


        public static string DeleteProduct(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null)
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                        return "SUCCESS";
                    }
                    return "FAIL: Product not found.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return "FAIL: Error occurred while deleting product.";
            }
        }

        public string AddCategory(string name)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var category = new Category { Name = name };
                    context.Categories.Add(category);
                    context.SaveChanges();
                    return JsonSerializer.Serialize(new { Success = true, Message = "Category added successfully!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
                return JsonSerializer.Serialize(new { Success = false, Message = "Failed to add category." });
            }
        }

        public string EditCategory(int id, string name)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var category = context.Categories.FirstOrDefault(c => c.Id == id);
                    if (category != null)
                    {
                        category.Name = name;
                        context.SaveChanges();
                        return JsonSerializer.Serialize(new { Success = true, Message = "Category updated successfully!" });
                    }
                    return JsonSerializer.Serialize(new { Success = false, Message = "Category not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing category: {ex.Message}");
                return JsonSerializer.Serialize(new { Success = false, Message = "Failed to edit category." });
            }
        }

    }
}