using System;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace WinFormClient
{
    class DataBaseSQL
    {
        static internal string AuthorizeDB()
        {
            TextBox tbUsername = Application.OpenForms["MainForm"].Controls["tbUsername"] as TextBox;
            TextBox tbPassword = Application.OpenForms["MainForm"].Controls["tbPassword"] as TextBox;
            try
            {
                string connectionString = @"Data Source=192.168.1.1,1433;Initial Catalog=DBUsers;User ID = Admin;Password = Admin;";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string request = "SELECT COUNT(Username) FROM Users WHERE Username = '"+ tbUsername.Text + "'";
                SqlCommand sqlCommand = new SqlCommand(request,connection);
                int number = (Int32)sqlCommand.ExecuteScalar();
                
                if (number>0)
                {
                    request = "SELECT COUNT(Username) FROM Users WHERE Username = '" + tbUsername.Text + "' AND Password  =  '" + tbPassword.Text + "'";
                    sqlCommand = new SqlCommand(request, connection);
                    number = (Int32)sqlCommand.ExecuteScalar();
                    connection.Close();
                    if (number > 0)
                    {
                        return "OK";
                        
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password");
                        return "Error";
                       
                    }
                }
                else
                {
                    MessageBox.Show("User is not found");
                    return "Error";
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
