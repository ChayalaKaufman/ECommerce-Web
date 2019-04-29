using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ECommerce.Data
{
    public class ClientDb
    {
        private string _connectionString;

        public ClientDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Product> GetProductsForCategory(int id)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            if (id == 0)
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Products";
            }
            else
            {
                cmd.CommandText = "SELECT * FROM Products WHERE CategoryId = @id";
                cmd.Parameters.AddWithValue("@id", id);
            }
            conn.Open();
            var products = new List<Product>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    Price = (decimal)reader["Price"],
                    Image = (string)reader["Image"]
                });
            }
            return products;
        }

        public IEnumerable<Category> GetCategories()
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Categories";
            conn.Open();
            List<Category> categories = new List<Category>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Category
                {
                    Name = (string)reader["Name"],
                    Id = (int)reader["Id"]
                });
            }
            return categories;
        }

        public Product GetProduct(int id)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Products WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            return new Product
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = (string)reader["Description"],
                CategoryId = (int)reader["CategoryId"],
                Image = (string)reader["Image"],
                Price = (decimal)reader["Price"]
            };
        }

        public void AddCustomer(Customer c, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            c.PasswordHash = hash;

            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Customers (FirstName,LastName,Email,PasswordHash) 
                                VALUES (@firstName,@lastName,@email,@passwordHash) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@email", c.Email);
            cmd.Parameters.AddWithValue("@passwordHash", c.PasswordHash);
            conn.Open();
            c.Id = (int)(decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
        }

        public Customer Login(string email, string password)
        {
            var customer = GetByEmail(email);
            if (customer == null)
            {
                return null; //incorrect email
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, customer.PasswordHash);

            if (!isValid)
            {
                return null;
            }

            return customer;
        }

        public Customer GetByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT TOP 1 * FROM Customers c
                                    left join addresses a on a.customerId = c.Id
                                    WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                Customer c = new Customer
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    PasswordHash = (string)reader["PasswordHash"],
                    Email = (string)reader["Email"]
                };
                if (reader["Street"] != DBNull.Value)
                {
                    c.Address = new Address
                    {
                        StreetAddress = (string)reader["Street"],
                        City = (string)reader["City"],
                        State = (string)reader["State"],
                        Zip = (string)reader["Zip"]
                    };
                }
                return c;
            }
        }

        public int CreateCart()
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO ShoppingCarts VALUES (@date) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public void AddToCart(int productId, int cartId, int quantity)
        {
            bool exists = CartItemExists(productId, cartId);

            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            if (exists)
            {
                cmd.CommandText = "UPDATE CartProducts SET Quantity = Quantity + 1 WHERE cartId = @cartId AND ProductId = @productId";
            }
            else
            {
                cmd.CommandText = "INSERT INTO CartProducts VALUES (@productId,@cartId,@quantity)";
                cmd.Parameters.AddWithValue("@quantity", quantity);
            }
            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@cartId", cartId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public bool CartItemExists(int productId, int cartId)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Quantity FROM CartProducts WHERE cartId = @cartId AND ProductId = @productId";
            cmd.Parameters.AddWithValue("@cartId", cartId);
            cmd.Parameters.AddWithValue("@productId", productId);
            conn.Open();
            if (cmd.ExecuteScalar() != null)
            {
                return true;
            }
            return false;
        }

        public List<CartProduct> GetCartProducts(int cartId)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from CartProducts cp
                                join Products p on cp.ProductId = p.Id 
                                where cp.CartId = @cartId";
            cmd.Parameters.AddWithValue("@cartId", cartId);
            conn.Open();
            var reader = cmd.ExecuteReader();
            List<CartProduct> cartProducts = new List<CartProduct>();
            while (reader.Read())
            {
                CartProduct cp = new CartProduct
                {
                    Product = new Product
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                        Image = (string)reader["Image"],
                        CategoryId = (int)reader["CategoryId"],
                        Price = (decimal)reader["Price"]
                    },

                    Quantity = (int)reader["Quantity"]
                };

                cartProducts.Add(cp);
            }
            return cartProducts;
        }

        public void RemoveFromCart(int cartId, int productId)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE CartProducts WHERE CartId = @cartId AND ProductId = @productId";
            cmd.Parameters.AddWithValue("@cartId", cartId);
            cmd.Parameters.AddWithValue("@productId", productId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void EditCart(int cartId, int productId, int quantity)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE CartProducts SET Quantity = @quantity WHERE CartId = @cartId AND ProductId = @productId";
            cmd.Parameters.AddWithValue("@cartId", cartId);
            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@quantity", quantity);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }
    }
}
