using GridViewApplication.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewApplication.Repository
{
    public static class Repository1
    {
        private static readonly string connectionString = Connection.ConnectionStringMsSQL;

        public static DataTable BindGridViewDataset()
        {
            DataTable dt = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM data order by id asc", con);
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }
        

        public static void InsertData(string name, string email, string phone, string age)
        {
            string commandText = "INSERT INTO data VALUES(@Name, @Email, @Phone, @Age)";
            SqlParameter[] parameters = {
                new SqlParameter("@Name", name),
                new SqlParameter("@Email", email),
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Age", age)
            };

            Command.ExecuteNonQuery(commandText, parameters);
        }
        public static void UpdateData(int userId, string name, string email, string phone, string age)
        {
            string commandText = "UPDATE data SET Name = @Name, Email = @Email, Phone = @Phone, Age = @Age WHERE Id = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@Name", name),
                new SqlParameter("@Email", email),
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Age", age),
                new SqlParameter("@UserId", userId)
            };

            Command.ExecuteNonQuery(commandText, parameters);
        }

        public static void DeleteData(int userId)
        {
            string commandText = "DELETE FROM data WHERE Id = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            Command.ExecuteNonQuery(commandText, parameters);
        }

        public static bool IsEmailExist(string email, int userId)
        {
            string query = "SELECT COUNT(*) FROM data WHERE email = @Email AND Id != @UserId";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@UserId", userId);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}