using NewsSite.Common.Abstract;
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
    internal class IdentityUtils : BaseUtils, ITestUtils<string>
    {

        public IdentityUtils(IConnStringWrapper wrapper)
            : base(wrapper)
        {
            _connStringName = wrapper.ConnectionName;
        }

        public int AddLike(int userId, int newsId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cleans DB for tests
        /// </summary>
        public void CleanTables()
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;

            string cmdText_clear_like = "DELETE FROM [Like]";
            string cmdText_clear_news = "DELETE FROM [News]";
            string cmdText_clear_user_roles = "DELETE FROM [AspNetUserRoles]";
            string cmdText_clear_users = "DELETE FROM [AspNetUsers]";
            string cmdText_clear_roles = "DELETE FROM [AspNetRoles]";
            ExecuteNonQuery(connString, cmdText_clear_like);
            ExecuteNonQuery(connString, cmdText_clear_news);
            ExecuteNonQuery(connString, cmdText_clear_user_roles);
            ExecuteNonQuery(connString, cmdText_clear_users);
            ExecuteNonQuery(connString, cmdText_clear_roles);
        }

                /// <summary>
        /// Create two default userd, one per type
        /// </summary>

        public Tuple<string, string> CreateUsers()
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;

            string cmdText_restore_employee_role = "INSERT INTO [AspNetRoles] ([Id],[Name]) VALUES ('b938a800-076f-42aa-aeb8-54fad97a3624','Employee')";
            string cmdText_restore_publisher_role = "INSERT INTO [AspNetRoles] ([Id],[Name]) VALUES ('96a509fa-7233-4312-a0d4-c15a7896b365','Publisher')";
            ExecuteNonQuery(connString, cmdText_restore_employee_role);
            ExecuteNonQuery(connString, cmdText_restore_publisher_role);

            string cmdText_restore_test_employee_user = $"INSERT INTO [AspNetUsers] VALUES ('da4c1e3d-8d68-48c2-bbfd-baa2601acaf8','London','test_employee@newssite.co.uk',0,'AOk3zp4VEGgoLchkdIHN2II3Bfun1csd+hTLAL51amMAt6BDV8tFO/IxelODbv0nsw==','cffc7c6b-c9c7-4926-90d8-b85297b7ed3d',NULL,0,0,NULL,1,0,'test_employee@newssite.co.uk')";
            string cmdText_restore_test_publisher_user = $"INSERT INTO [AspNetUsers] VALUES ('29d68301-933c-4e37-a8ed-84630aacdd86','London','test_publisher@newssite.co.uk',0,'ALQESUdEJbcK8xzI/LtTbdhgmTGIpNGoOVpF9A4Ylxx/a/NvI8Lzjc7DzWfVHh4+pw==','b669f0e1-6448-46bc-bd16-543cd476fadd',NULL,0,0,NULL,1,0,'test_publisher@newssite.co.uk')";
            ExecuteNonQuery(connString, cmdText_restore_test_employee_user);
            ExecuteNonQuery(connString, cmdText_restore_test_publisher_user);

            string cmdText_restore_test_employee_user_role = $"INSERT INTO [AspNetUserRoles] VALUES ('da4c1e3d-8d68-48c2-bbfd-baa2601acaf8','b938a800-076f-42aa-aeb8-54fad97a3624')";
            string cmdText_restore_test_publisher_user_role = $"INSERT INTO [AspNetUserRoles] VALUES ('29d68301-933c-4e37-a8ed-84630aacdd86','96a509fa-7233-4312-a0d4-c15a7896b365')";
            ExecuteNonQuery(connString, cmdText_restore_test_employee_user_role);
            ExecuteNonQuery(connString, cmdText_restore_test_publisher_user_role);

            return new Tuple<string, string>("da4c1e3d-8d68-48c2-bbfd-baa2601acaf8", "29d68301-933c-4e37-a8ed-84630aacdd86");
        }

        public virtual int CreateSingleNews(string authorId)
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;

            string cmdText_create_news = $"INSERT INTO [News] ([Title],[Body],[CreatedDate],[AuthorId]) VALUES ('{NewsTitle}','{NewsBody}',GETDATE(),'{authorId}'); SELECT @@IDENTITY;";
            return Convert.ToInt32(ExecuteScalar(connString, cmdText_create_news));
        }

        public virtual int AddLike(string userId, int newsId)
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;

            string cmdText_create_like = $"INSERT INTO [Like] ([UserId],[NewsId],[CreatedDate]) VALUES ('{userId}','{newsId}',GETDATE()); SELECT @@IDENTITY;";
            int likeId = Convert.ToInt32(ExecuteScalar(connString, cmdText_create_like));
            return likeId;

        }
    }
}
