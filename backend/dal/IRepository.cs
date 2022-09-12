using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace dal
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool> predicate = null);
        IEnumerable<T> GetAll(int index, int count);

        T GetById(object id);
        Task<T> GetByIdAsync(object id);

        void Insert(T obj);
        Task InsertAsync(T obj);

        void InsertMany(IEnumerable<T> obj);
        Task InsertManyAsync(IEnumerable<T> obj);

        void Update(T obj);
        Task UpdateAsync(T obj);

        //void UpdateMany(IEnumerable<T> obj);
        //Task<T> UpdateManyAsync(IEnumerable<T> obj);

        void Delete(object id);
        Task DeleteAsync(object id);

        void DeleteMany(IEnumerable<T> obj);
        Task DeleteManyAsync(IEnumerable<T> obj);

        // 1. Sql transaction
        Task ExecuteSqlBulkInsertAsync(IEnumerable<T> obj, string tablename);

        // 2. SqlCommand approach
        void ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] parameters = null);

        ICollection ExecuteReader(string commandText, CommandType commandType, SqlParameter[] parameters = null);

        // 2. SqlQuery approach
        Task<object> ExecuteStoredProc(string sqlQuery, Dictionary<string, object> parameters = null);

        

    }

}