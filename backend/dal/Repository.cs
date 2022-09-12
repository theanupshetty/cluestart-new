
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Reflection;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Metadata;

namespace dal
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbContext _context = null;
        private DbSet<T> table = null;

        public Repository(DbContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll(Func<T, bool> predicate)
        {
            try
            {
                if (predicate != null)
                {
                    return table.Where(predicate);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return table.ToList();
        }

        public IEnumerable<T> GetAll(int index, int count)
        {
            try
            {
                return table.Skip(index).Take(count).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetById(object id)
        {
            try
            {
                return table.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<T> GetByIdAsync(object id)
        {
            try
            {
                return await table.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region Insert
        public void Insert(T obj)
        {
            try
            {
                table.Add(obj);
                _context.Entry(obj).State = EntityState.Added;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task InsertAsync(T obj)
        {
            try
            {
                table.Add(obj);
                _context.Entry(obj).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void InsertMany(IEnumerable<T> obj)
        {
            try
            {
                table.AddRange(obj);
                _context.Entry(obj).State = EntityState.Added;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task InsertManyAsync(IEnumerable<T> obj)
        {
            try
            {
                table.AddRange(obj);
                _context.Entry(obj).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public void Update(T obj)
        {
            try
            {
                table.Attach(obj);
                _context.Entry(obj).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task UpdateAsync(T obj)
        {
            try
            {
                table.Attach(obj);
                _context.Entry(obj).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(object id)
        {
            try
            {
                T existing = table.Find(id);
                table.Remove(existing);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteAsync(object id)
        {
            try
            {
                T existing = table.Find(id);
                table.Remove(existing);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteMany(IEnumerable<T> obj)
        {
            try
            {
                table.RemoveRange(obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteManyAsync(IEnumerable<T> obj)
        {
            try
            {
                table.RemoveRange(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ExecuteSqlBulkInsertAsync(IEnumerable<T> obj, string tablename)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {

                    bulkCopy.BatchSize = 10000;
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.DestinationTableName = "[dbo].[" + tablename + "]";
                    try
                    {
                        DataTable dt = obj.AsDataTable();
                        await bulkCopy.WriteToServerAsync(dt);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }


            }
        }

        public void ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {
            try
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Closed)
                {
                    _context.Database.GetDbConnection().Open();
                }

                var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = commandText;
                command.CommandType = commandType;

                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICollection ExecuteReader(string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {
            try
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Closed)
                {
                    _context.Database.GetDbConnection().Open();
                }

                var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = commandText;
                command.CommandType = commandType;

                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                using (var reader = command.ExecuteReader())
                {
                    var mapper = new DataReaderMapper<T>();
                    return mapper.MapToList(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<object> ExecuteStoredProc(string sqlQuery, Dictionary<string, object> parameters = null)
        {
            try
            {
                return await StoredProc(sqlQuery, parameters);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private async Task<List<T>> StoredProc(string storedProcName, Dictionary<string, object> procParams)
        {

            DbConnection conn = _context.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                await using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> procParam in procParams)
                    {
                        DbParameter param = command.CreateParameter();
                        param.ParameterName = procParam.Key;
                        param.Value = procParam.Value;
                        command.Parameters.Add(param);
                    }

                    DbDataReader reader = command.ExecuteReader();
                    List<T> objList = new List<T>();
                    IEnumerable<PropertyInfo> props = typeof(T).GetRuntimeProperties();
                    Dictionary<string, DbColumn> colMapping = reader.GetColumnSchema()
                        .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                        .ToDictionary(key => key.ColumnName.ToLower());

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            T obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in props)
                            {
                                object val =
                                    reader.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                                prop.SetValue(obj, val == DBNull.Value ? null : val);
                            }
                            objList.Add(obj);
                        }
                    }
                    reader.Dispose();

                    return objList;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
            }
            finally
            {
                conn.Close();
            }

            return null; // default state
        }
    }
}
