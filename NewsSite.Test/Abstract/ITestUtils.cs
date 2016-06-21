using System;
using System.Data;

namespace NewsSite.Test.Abstract
{
    internal interface ITestUtils<T>
    {
        int AddLike(T userId, int newsId);
        void CleanTables();
        int CreateSingleArticle(T authorId);
        Tuple<T, T> CreateUsers();
        void ExecuteNonQuery(string connString, string cmdText);
        object ExecuteScalar(string connString, string cmdText);
        DataTable QueryTable(string cmdText);
    }
}