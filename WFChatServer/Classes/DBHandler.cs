using System;
using System.Data;
using ChatShared.Protocol;
using Microsoft.Data.SqlClient;

namespace WFChatServer
{
    /// <summary>Checks users against the Users table of the LocalDB database dbRegisteredUsers.mdf.</summary>
    class DBHandler : IUserAuthenticator
    {
        public LoginError? Authenticate(string username, string password)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\dbRegisteredUsers.mdf;Integrated Security=True;Pooling=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //values are passed as parameters, never concatenated into the query text (SQL injection)
                SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(Username) FROM Users WHERE Username = @username", connection);
                sqlCommand.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                int number = (Int32)sqlCommand.ExecuteScalar();

                if (number > 0)
                {
                    sqlCommand = new SqlCommand("SELECT COUNT(Username) FROM Users WHERE Username = @username AND Password = @password", connection);
                    sqlCommand.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    sqlCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                    number = (Int32)sqlCommand.ExecuteScalar();
                    if (number > 0)
                    {
                        return null;
                    }
                    else
                    {
                        return LoginError.IncorrectPassword;
                    }
                }
                else
                {
                    return LoginError.UserNotFound;
                }
            }
        }
    }
}
