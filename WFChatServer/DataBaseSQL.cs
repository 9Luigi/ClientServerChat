using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using WFChatServer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WFChatServer
{
    class DataBaseSQL
    {
        static internal string AuthorizeDB(string username, string password)
        {
            try
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\dbRegisteredUsers.mdf;Integrated Security=True";
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
                    connection.Close();
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "Error"; 
            }
        }
    }
}
