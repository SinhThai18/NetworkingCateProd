using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using serverProduct.Models;
using static System.Collections.Specialized.BitVector32;

namespace ServerApp
{
    public class ClientHandler
    {
        private TcpClient _client;
        private int _clientId;

        DatabaseHandler DatabaseHandler = new DatabaseHandler();
        public ClientHandler(TcpClient client, int clientId)
        {
            _client = client;
            _clientId = clientId;
        }

        public void HandleClient()
        {
            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[1024];
            int count;

            try
            {
                // Gửi tất cả category cho client
                //string categories = DatabaseHandler.GetCategories();
                //byte[] categoryResponse = Encoding.UTF8.GetBytes(categories);
                //stream.Write(categoryResponse, 0, categoryResponse.Length);
                //Console.WriteLine($"[CLIENT {_clientId}] Sent categories to client.");

                // Xử lý yêu cầu từ client
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string request = Encoding.UTF8.GetString(buffer, 0, count);
                    Console.WriteLine($"[CLIENT {_clientId}] Received: {request}");

                    

                    if (request == "ALL")
                    {
                        string allProducts = DatabaseHandler.GetAllProducts();
                        byte[] allProductsResponse = Encoding.UTF8.GetBytes(allProducts);
                        stream.Write(allProductsResponse, 0, allProductsResponse.Length);
                        Console.WriteLine($"[CLIENT {_clientId}] Sent all products to client.");
                    }
                    else if (request == "GET_CATEGORIES")
                    {
                        // Lấy tất cả các danh mục từ Database và gửi lại client
                        string categories = DatabaseHandler.GetCategories(); // Hàm này trả về chuỗi JSON danh sách danh mục
                        byte[] categoryResponse = Encoding.UTF8.GetBytes(categories);
                        stream.Write(categoryResponse, 0, categoryResponse.Length);

                        Console.WriteLine($"[CLIENT {_clientId}] Sent categories to client.");
                    }
                    else if (request.StartsWith("{"))
                    {
                        try
                        {
                            // Giải mã yêu cầu JSON
                            var jsonDoc = JsonDocument.Parse(request);
                            string action = jsonDoc.RootElement.GetProperty("Action").GetString();

                            if (action == "ADD_PRODUCT")
                            {
                                var productData = jsonDoc.RootElement.GetProperty("Product");
                                var product = new Product
                                {
                                    Name = productData.GetProperty("Name").GetString(),
                                    Price = productData.GetProperty("Price").GetDecimal(),
                                    CatId = productData.GetProperty("CatId").GetInt32()
                                };

                                // Thêm sản phẩm vào database
                                string response = DatabaseHandler.AddProduct(product.Name, product.Price, product.CatId);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                stream.Write(responseBytes, 0, responseBytes.Length);

                                Console.WriteLine($"[CLIENT {_clientId}] Added product: {product.Name}, {product.Price}, Category ID: {product.CatId}");
                            }
                           
                            else if (action == "EDIT_PRODUCT")
                            {
                                var productData = jsonDoc.RootElement.GetProperty("Product");
                                int id = productData.GetProperty("Id").GetInt32();
                                string name = productData.GetProperty("Name").GetString();
                                decimal price = productData.GetProperty("Price").GetDecimal();
                                int catId = productData.GetProperty("CatId").GetInt32();

                                // Cập nhật sản phẩm trong database
                                string response = DatabaseHandler.EditProduct(id, name, price, catId);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                //Thread.Sleep(10000);
                                stream.Write(responseBytes, 0, responseBytes.Length);

                                Console.WriteLine($"[CLIENT {_clientId}] Edited product: {name}, {price}, Category ID: {catId}");
                            }
                            else if (action == "DELETE_PRODUCT")
                            {
                                int id = jsonDoc.RootElement.GetProperty("ProductId").GetInt32();
                               
                                // Xóa sản phẩm trong database
                                string response = DatabaseHandler.DeleteProduct(id);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                stream.Write(responseBytes, 0, responseBytes.Length);

                                Console.WriteLine($"[CLIENT {_clientId}] Deleted product with ID: {id}");
                            }
                            else if (action == "ADD_CATEGORY")
                            {
                                var categoryData = jsonDoc.RootElement.GetProperty("Data");
                                string name = categoryData.GetProperty("Name").GetString();

                                string response = DatabaseHandler.AddCategory(name);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                stream.Write(responseBytes, 0, responseBytes.Length);

                                Console.WriteLine($"[CLIENT {_clientId}] Added category: {name}");
                            }
                            else if (action == "EDIT_CATEGORY")
                            {
                                var categoryData = jsonDoc.RootElement.GetProperty("Data");
                                int id = categoryData.GetProperty("Id").GetInt32();
                                string name = categoryData.GetProperty("Name").GetString();

                                string response = DatabaseHandler.EditCategory(id, name);
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                stream.Write(responseBytes, 0, responseBytes.Length);

                                Console.WriteLine($"[CLIENT {_clientId}] Edited category ID: {id}, Name: {name}");
                            }
                            else
                            {
                                byte[] errorResponse = Encoding.UTF8.GetBytes("Invalid action.");
                                stream.Write(errorResponse, 0, errorResponse.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing JSON: {ex.Message}");
                            byte[] errorResponse = Encoding.UTF8.GetBytes("Invalid request format.");
                            stream.Write(errorResponse, 0, errorResponse.Length);
                        }
                    }
                    else
                    {
                        int categoryId = int.Parse(request);
                        string products = DatabaseHandler.GetProductsByCategory(categoryId);
                        byte[] productResponse = Encoding.UTF8.GetBytes(products);
                        stream.Write(productResponse, 0, productResponse.Length);
                        Console.WriteLine($"[CLIENT {_clientId}] Sent products for category {categoryId}.");
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"[CLIENT {_clientId}] Error: {e.Message}");
            }

            _client.Close();
            Console.WriteLine($"[CLIENT {_clientId}] Disconnected.");
        }


    }
}