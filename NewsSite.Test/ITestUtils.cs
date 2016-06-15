using System;
using System.Data;

namespace NewsSite.Test
{
    internal interface ITestUtils<T>
    {
        int AddLike(T userId, int newsId);
        void CleanTables();
        int CreateSingleNews(T authorId);
        Tuple<T, T> CreateUsers();
        void ExecuteNonQuery(string connString, string cmdText);
        object ExecuteScalar(string connString, string cmdText);
        DataTable QueryTable(string cmdText);
    }
}