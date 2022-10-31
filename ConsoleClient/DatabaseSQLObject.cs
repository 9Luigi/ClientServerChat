using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Server
{
    class DataBaseSQL
    {
        static protected internal void SearchDB()
        {
            string datasource = @"C:\Users\fastr\source\repos\MyClientServer\MyClientServer\DBUsers.mdf";
            string database = "users";
            string connString = @"Data Source=" + datasource + ";Database="
                        + database + ";Trusted_Connection=True";
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                Console.WriteLine("Openning Connection ...");

                //open connection
                conn.Open();

                Console.WriteLine("Connection successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            Console.Read();
        }
    }
    }
}
