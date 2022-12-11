using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WFChatServer
{
    class dbHandler
    {
        internal delegate void errorDelegate(string message);
        internal event errorDelegate errorEvent;
        internal string AuthorizeDB(string username, string password)
        {
            try
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\dbRegisteredUsers.mdf;Integrated Security=True;Pooling=True";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string request = "SELECT COUNT(Username) FROM Users WHERE Username = '"+ username + "'";
                SqlCommand sqlCommand = new SqlCommand(request,connection);
                int number = (Int32)sqlCommand.ExecuteScalar();
                
                if (number>0)
                {
                    request = "SELECT COUNT(Username) FROM Users WHERE Username = '" + username + "' AND Password  =  '" + password + "'";
                    sqlCommand = new SqlCommand(request, connection);
                    number = (Int32)sqlCommand.ExecuteScalar();
                    if (number > 0)
                    {
                        connection.Close();
                        return "OK";
                        
                    }
                    else
                    {
                        connection.Close();
                        return "Incorrect password"; 
                    }
                }
                else
                {
                    connection.Close();
                    return "User is not found";
                }
            }
            catch(Exception ex)
            {
                errorEvent.Invoke(ex.Message);
                return "Error"; 
            }
        }
    }
}
