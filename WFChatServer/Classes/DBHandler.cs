using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WFChatServer
{
    class DBHandler
    {
        internal delegate void ErrorDelegate(string message);
        internal event ErrorDelegate ErrorEvent;
        internal string AuthorizeDB(string username, string password)
        {
            try
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
                            return "OK";
                        }
                        else
                        {
                            return "Incorrect password";
                        }
                    }
                    else
                    {
                        return "User is not found";
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorEvent.Invoke(ex.Message);
                return "Error";
            }
        }
    }
}
