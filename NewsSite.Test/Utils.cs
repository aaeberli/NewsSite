using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Test
{
    internal class Utils
    {
        public static string TestPublisher { get { return "test_publisher"; } }
        public static string TestEmployee { get { return "test_employee"; } }
        public static string TestPublisherPass { get { return "test_employee_pass"; } }
        public static string TestEmployeePass { get { return "test_employee_pass"; } }

        public static string NewsTitle { get { return "test_news_title"; } }
        public static string NewsBody { get { return "test_news_body"; } }

        public static void ExecuteNonQuery(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string connString, string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable QueryTable(string cmdText)
        {
            string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;
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

        /// <summary>
        /// Cleans DB for tests
        /// </summary>
        public static void CleanTables()
        {
            string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;

            string cmdText_clear_like = "DELETE FROM [Like]";
            string cmdText_clear_news = "DELETE FROM [News]";
            string cmdText_clear_user = "DELETE FROM [User]";
            string cmdText_clear_userType = "DELETE FROM [UserType]";
            ExecuteNonQuery(connString, cmdText_clear_like);
            ExecuteNonQuery(connString, cmdText_clear_news);
            ExecuteNonQuery(connString, cmdText_clear_user);
            ExecuteNonQuery(connString, cmdText_clear_userType);
        }

        /// <summary>
        /// Create two default userd, one per type
        /// </summary>
        public static Tuple<int, int> CreateUsers()
        {
            string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;

            string cmdText_restore_userType1 = "INSERT INTO [UserType] ([Type],[Description]) VALUES (1,'Publisher'); SELECT @@IDENTITY;";
            string cmdText_restore_userType2 = "INSERT INTO [UserType] ([Type],[Description]) VALUES (2,'Employee'); SELECT @@IDENTITY;";
            object publisherId = ExecuteScalar(connString, cmdText_restore_userType1);
            object employeeId = ExecuteScalar(connString, cmdText_restore_userType2);

            string cmdText_restore_test_publisher_user = $"INSERT INTO [User] ([UserName],[Password],[CreatedDate],[TypeId]) VALUES ('{TestPublisher}','{TestPublisherPass}',GETDATE(),{publisherId}); SELECT @@IDENTITY;";
            string cmdText_restore_test_employee_user = $"INSERT INTO [User] ([UserName],[Password],[CreatedDate],[TypeId]) VALUES ('{TestEmployee}','{TestEmployeePass}',GETDATE(),{employeeId}); SELECT @@IDENTITY;";
            return new Tuple<int, int>(
                    Convert.ToInt32(ExecuteScalar(connString, cmdText_restore_test_publisher_user)),
                    Convert.ToInt32(ExecuteScalar(connString, cmdText_restore_test_employee_user))
                );
        }

        public static int CreateSingleNews(int authorId)
        {
            string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;

            string cmdText_create_news = $"INSERT INTO [News] ([Title],[Body],[CreatedDate],[AuthorId]) VALUES ('{NewsTitle}','{NewsBody}',GETDATE(),{authorId}); SELECT @@IDENTITY;";
            return Convert.ToInt32(ExecuteScalar(connString, cmdText_create_news));
        }

        public static int AddLike(int userId, int newsId)
        {
            string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;

            string cmdText_create_like = $"INSERT INTO [Like] ([UserId],[NewsId],[CreatedDate]) VALUES ({userId},{newsId},GETDATE()); SELECT @@IDENTITY;";
            int likeId = Convert.ToInt32(ExecuteScalar(connString, cmdText_create_like));
            return likeId;

        }


        ///// <summary>
        ///// Creates a test user directly in DB
        ///// </summary>
        //public static object CreateUser(string userId)
        //{
        //    string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;

        //    string cmdText_create_user = $"INSERT INTO [User] values ('{userId}'); SELECT @@IDENTITY;";
        //    return ExecuteScalar(connString, cmdText_create_user);
        //}
        //public static object CreateUser()
        //{
        //    return CreateUser("test_user");
        //}

        ///// <summary>
        ///// Creates an otp for the specified used directly in DB
        ///// </summary>
        ///// <returns></returns>
        //public static void CreateOtp(object id, string password, DateTime? startDate = null)
        //{
        //    string connString = ConfigurationManager.ConnectionStrings["NewsSiteDb"].ConnectionString;

        //    string _startDate = startDate != null ? $"'{((DateTime)startDate).ToString("yyyy-MM-dd hh:mm:ss.fff tt")}'" : "GETDATE()";
        //    string cmdText_create_user = $"INSERT INTO [Otp] ([UserId],[Password],[StartDate]) values ({id},'{password}',{_startDate});";
        //    ExecuteNonQuery(connString, cmdText_create_user);
        //}
        //public static void CreateOtp(object id)
        //{
        //    CreateOtp(id, "test_pass", null);
        //}
    }
}
