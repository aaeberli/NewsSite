using NewsSite.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Test.Abstract
{
    internal abstract class BaseUtils
    {
        public static string TestPublisher { get { return "test_publisher"; } }
        public static string TestEmployee { get { return "test_employee"; } }
        public static string TestPublisherPass { get { return "test_employee_pass"; } }
        public static string TestEmployeePass { get { return "test_employee_pass"; } }

        public static string ArticleTitle { get { return "test_article_title"; } }
        public static string ArticleBody { get { return "test_article_body"; } }

        protected string _connStringName;

        public BaseUtils(IConnStringWrapper wrapper)
        {
            _connStringName = wrapper.ConnectionName;
        }

        public void ExecuteNonQuery(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public DataTable QueryTable(string cmdText)
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
        }
    }

}
