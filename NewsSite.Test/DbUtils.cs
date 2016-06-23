using NewsSite.Common.Abstract;
using NewsSite.Test.Abstract;
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
    /// <summary>
    /// Utility class
    /// Creates some basic data into the DB to support stateless tests
    /// </summary>
    internal class DbUtils : BaseUtils, ITestUtils<string>
    {

        public DbUtils(IConnStringWrapper wrapper)
            : base(wrapper)
        {
            _connStringName = wrapper.ConnectionName;
        }

        public int AddLike(int userId, int articleId)
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
            string cmdText_clear_article = "DELETE FROM [Articles]";
            string cmdText_clear_user_roles = "DELETE FROM [AspNetUserRoles]";
            string cmdText_clear_users = "DELETE FROM [AspNetUsers]";
            string cmdText_clear_roles = "DELETE FROM [AspNetRoles]";
            ExecuteNonQuery(connString, cmdText_clear_like);
            ExecuteNonQuery(connString, cmdText_clear_article);
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

            /* Passwords
             * 
             * employee: Test_employee16
             * publisher: Test_publisher16
             * publisher2: Test_publisher216
             * 
             */
            string cmdText_restore_test_employee_user = "INSERT INTO [AspNetUsers] VALUES ('fc9c5c06-814a-4c30-a541-a76ba3465b6e','London','test_employee@newssite.co.uk',0,'AMYnIA4EnbLVqvgEX7+dShc7hkAv4QCvI+nwP8LBKQQaEQqwqKndvnl6vNbnaQO0/A==','c176ec16-8277-4be4-886c-bb2d5ddeba93',NULL,0,0,NULL,1,0,'test_employee@newssite.co.uk')";
            string cmdText_restore_test_publisher_user = "INSERT INTO [AspNetUsers] VALUES ('a095d0a7-b38b-4827-97a9-36be9b91520d','London','test_publisher@newssite.co.uk',0,'AMWMlyyvq4qoydtP/wsuKbTdnrxKmEfWHlgSbGlvxAVMuk+B4OBDj3SVKlSV2j69qQ==','bab4e758-e486-4286-80e0-2864066b4ab4',NULL,0,0,NULL,1,0,'test_publisher@newssite.co.uk')";
            string cmdText_restore_test_publisher2_user = "INSERT INTO [AspNetUsers] VALUES ('fd6a6985-ae73-45f4-83a2-ede321906fb1','London','test_publisher2@newssite.co.uk',0,'AL/12ULOq0h1f8xckr2ylvBlMWJS1uJ1MeJOKvjXiCjkIUhhPg2HIdvw2HSFtnd2hQ==','a1a2a678-f242-4b43-8ecf-edc8753f7c94',NULL,0,0,NULL,1,0,'test_publisher2@newssite.co.uk')";
            ExecuteNonQuery(connString, cmdText_restore_test_employee_user);
            ExecuteNonQuery(connString, cmdText_restore_test_publisher_user);
            ExecuteNonQuery(connString, cmdText_restore_test_publisher2_user);

            string cmdText_restore_test_employee_user_role = "INSERT INTO [AspNetUserRoles] VALUES ('fc9c5c06-814a-4c30-a541-a76ba3465b6e','b938a800-076f-42aa-aeb8-54fad97a3624')";
            string cmdText_restore_test_publisher_user_role = "INSERT INTO [AspNetUserRoles] VALUES ('a095d0a7-b38b-4827-97a9-36be9b91520d','96a509fa-7233-4312-a0d4-c15a7896b365')";
            string cmdText_restore_test_publisher2_user_role = "INSERT INTO [AspNetUserRoles] VALUES ('fd6a6985-ae73-45f4-83a2-ede321906fb1','96a509fa-7233-4312-a0d4-c15a7896b365')";
            ExecuteNonQuery(connString, cmdText_restore_test_employee_user_role);
            ExecuteNonQuery(connString, cmdText_restore_test_publisher_user_role);
            ExecuteNonQuery(connString, cmdText_restore_test_publisher2_user_role);

            return new Tuple<string, string>("fc9c5c06-814a-4c30-a541-a76ba3465b6e", "a095d0a7-b38b-4827-97a9-36be9b91520d");
        }

        public virtual int CreateSingleArticle(string authorId)
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;

            string cmdText_create_article = $"INSERT INTO [Articles] ([Title],[Body],[CreatedDate],[AuthorId]) VALUES ('{ArticleTitle}','{ArticleBody}',GETDATE(),'{authorId}'); SELECT @@IDENTITY;";
            return Convert.ToInt32(ExecuteScalar(connString, cmdText_create_article));
        }

        public virtual int AddLike(string userId, int articleId)
        {
            string connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;

            string cmdText_create_like = $"INSERT INTO [Like] ([UserId],[ArticleId],[CreatedDate]) VALUES ('{userId}','{articleId}',GETDATE()); SELECT @@IDENTITY;";
            int likeId = Convert.ToInt32(ExecuteScalar(connString, cmdText_create_like));
            return likeId;

        }
    }
}
