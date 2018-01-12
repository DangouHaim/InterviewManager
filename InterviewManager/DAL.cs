using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Security.Cryptography;

namespace InterviewManager
{
    class DAL : IDisposable
    {
        private const string cs = "Data Source=DANGOU-PC;Initial Catalog=InterviewManager;Integrated Security=True";
        SqlConnection con = null;

        public DAL()
        {
            con = new SqlConnection(cs);
            con.Open();
        }

        public SqlCommand Query(string q)
        {
            return new SqlCommand(q, con);
        }

        public bool CheckForExistance(string table, string field, string value)
        {
            bool res = false;
            using (var rdr = Query("SELECT * FROM [" + table + "] WHERE [" + field + "] = '" + value + "'").ExecuteReader())
            {
                res = rdr.HasRows;
            }
            return res;
        }

        public static string GetMd5Hash(string text)
        {

            string source = text;
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
            }
            return sBuilder.ToString();
        }

        public void Dispose()
        {
            con.Dispose();
        }
    }
}
