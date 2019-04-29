using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ECommerce.Data
{
    public class Db
    {
        private string _connectionString;

        public Db(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddCategory(string catName)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Categories VALUES (@name)";
            cmd.Parameters.AddWithValue("@name", catName);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
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

        public void AddProduct(Product p)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Products VALUES (@name,@description,@categoryId,@price,@image)";
            cmd.Parameters.AddWithValue("@name", p.Name);
            cmd.Parameters.AddWithValue("@description", p.Description);
            cmd.Parameters.AddWithValue("@categoryId", p.CategoryId);
            cmd.Parameters.AddWithValue("@price", p.Price);
            cmd.Parameters.AddWithValue("@image", p.Image);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        
    }
}
