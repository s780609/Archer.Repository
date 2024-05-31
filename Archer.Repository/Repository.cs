using System.Data;
using System.Text;
using Dapper;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Collections.Generic;
using System;
using System.Linq;
using Archer.Extension.DatabaseHelper;
using Archer.Extension;
using Microsoft.Data.SqlClient;
using Archer.Extension.SecurityHelper;

namespace Archer.Repository
{
    public class Repository : IRepository
    {
        private readonly string _connectionName;
        private readonly string _connectionString;
        internal readonly DatabaseHelper _databaseHelper;
        internal readonly SecurityHelper _securityHelper;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Repository(string connectionString, SecurityHelper securityHelper)
        {
            _connectionString = connectionString;
            _securityHelper = securityHelper;
        }

        public Repository(DatabaseHelper databaseHelper, string connectionName)
        {
            _databaseHelper = databaseHelper;
            _connectionName = connectionName;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int Execute(string sql, object param = null)
        {
            IDbConnection conn;

            if (_databaseHelper != null)
            {
                conn = _databaseHelper.CreateConnectionBy(_connectionName);
            }
            else if (_securityHelper != null)
            {
                conn = new SqlConnection(_securityHelper.DecryptConn(_connectionString));
            }
            else
            {
                conn = new SqlConnection(_connectionString);
            }

            conn.Open();

            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    int rows = conn.Execute(sql, param, transaction);
                    transaction.Commit();

                    return rows;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();

                    throw;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Base query. T is generic
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual List<Table> Query<Table>(string sql = "", object param = null)
        {
            IDbConnection conn;

            if (_databaseHelper != null)
            {
                conn = _databaseHelper.CreateConnectionBy(_connectionName);
            }
            else if (_securityHelper != null)
            {
                conn = new SqlConnection(_securityHelper.DecryptConn(_connectionString));
            }
            else
            {
                conn = new SqlConnection(_connectionString);
            }

            conn.Open();

            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    string tableName = FindTableName<Table>();

                    sql += $" SELECT * FROM {tableName} ";

                    if (param != null)
                    {
                        sql += GenerateWhereSqlScript(param);
                    }
                }

                try
                {
                    IEnumerable<Table> result = conn.Query<Table>(sql, param, transaction);
                    transaction.Commit();

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();

                    throw;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public virtual Table QuerySingle<Table>(string sql = "", object param = null)
        {
            IDbConnection conn;

            if (_databaseHelper != null)
            {
                conn = _databaseHelper.CreateConnectionBy(_connectionName);
            }
            else if (_securityHelper != null)
            {
                conn = new SqlConnection(_securityHelper.DecryptConn(_connectionString));
            }
            else
            {
                conn = new SqlConnection(_connectionString);
            }

            conn.Open();

            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    string tableName = FindTableName<Table>();

                    sql += $" SELECT * FROM {tableName} ";

                    if (param != null)
                    {
                        sql += GenerateWhereSqlScript(param);
                    }
                }

                try
                {
                    Table result = conn.QuerySingle<Table>(sql, param, transaction);
                    transaction.Commit();

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();

                    throw;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// 幫助產生 where 敘述，會自動產生 [參數=@參數] 的敘述
        /// ex: T tObject = { book= "clean code", bookId= "1" }
        /// string whereSqlScript = GenerateWhereSqlScript{T}(tObject);
        /// whereSqlScript is " WHERE 1=1 AND @book="clean code" AND @bookId="1" "
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tObject"></param>
        /// <returns></returns>
        public virtual string GenerateWhereSqlScript<T>(T tObject)
        {
            if (tObject == null)
            {
                throw new ArgumentNullException(nameof(tObject));
            }

            StringBuilder stringBuilder = new StringBuilder();

            string[] propsName = tObject.GetPropsName();
            string[] propsValue = tObject.GetPropsValue();

            if (propsName.Length != propsValue.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(propsName) + "'s length != " + nameof(propsValue) + "'s length.");
            }

            stringBuilder.Append(" WHERE 1=1 ");

            for (int i = 0; i < propsName.Length; i++)
            {
                if (propsValue[i] != null)
                {
                    stringBuilder.Append($" AND {propsName[i]} = @{propsName[i]} ");
                }
            }

            return stringBuilder.ToString();
        }

        public virtual string GenerateWhereSqlScript(object tObject)
        {
            if (tObject == null)
            {
                throw new ArgumentNullException(nameof(tObject));
            }

            StringBuilder stringBuilder = new StringBuilder();

            string[] propsName = tObject.GetPropsName();
            string[] propsValue = tObject.GetPropsValue();

            if (propsName.Length != propsValue.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(propsName) + "'s length != " + nameof(propsValue) + "'s length.");
            }

            stringBuilder.Append(" WHERE 1=1 ");

            for (int i = 0; i < propsName.Length; i++)
            {
                if (propsValue[i] != null)
                {
                    stringBuilder.Append($" AND {propsName[i]} = @{propsName[i]} ");
                }
            }

            return stringBuilder.ToString();
        }

        public virtual string FindTableName<Table>(string regexPattern = "^(.+)$")
        {
            Match match = Regex.Match(typeof(Table).Name, regexPattern);

            return match.Groups[1].Value;
        }

        public virtual int Create<Table>(Table model)
        {
            return this.Create<Table>((object)model);
        }

        public virtual int Create<Table>(object model)
        {
            string[] propsName = model.GetPropsName();
            string[] propsValue = model.GetPropsValue();

            if (propsName.Length != propsValue.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(propsName) + "'s length != " + nameof(propsValue) + "'s length.");
            }

            string tableName = this.FindTableName<Table>();

            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append($" INSERT INTO [dbo].[{tableName}] ( ");

            for (int i = 0; i < propsName.Length; i++)
            {
                if (propsValue[i] != null)
                {
                    sqlBuilder.Append($"[{propsName[i]}]");
                    sqlBuilder.Append(", ");
                }
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

            sqlBuilder.Append(" ) ");
            sqlBuilder.Append(" VALUES ( ");

            for (int i = 0; i < propsValue.Length; i++)
            {
                if (propsValue[i] != null)
                {
                    sqlBuilder.Append("@" + propsName[i]);
                    sqlBuilder.Append(", ");
                }
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

            sqlBuilder.Append(" ) ");

            return this.Execute(sqlBuilder.ToString(), model);
        }

        public virtual int Update<Table>(Table model, Table key)
        {
            return this.Update<Table>(model, key);
        }

        public virtual int Update<Table>(object model, object key)
        {
            key.ThrowIfNull(nameof(key));
            model.ThrowIfNull(nameof(model));

            string[] modelNames = model.GetPropsName();
            string[] modelValues = model.GetPropsValue();
            string[] keyNames = key.GetPropsName();
            string[] keyValues = key.GetPropsValue();

            int checkLoopNum = keyNames.Length < modelNames.Length ? keyNames.Length : modelValues.Length;

            for (int i = 0; i < checkLoopNum; i++)
            {
                if (modelNames[i] == keyNames[i])
                {
                    if (modelValues[i] == null && keyValues[i] == null)
                    {
                        continue;
                    }

                    throw new ArgumentException("model and key have same props.");
                }
            }

            if (modelNames.Length == 0)
            {
                throw new Exception($"{nameof(model)} has no any props. It at least need one prop.");
            }

            string tableName = this.FindTableName<Table>();

            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append($" UPDATE [dbo].[{tableName}] ");

            sqlBuilder.Append(" SET ");

            for (int i = 0; i < modelNames.Length; i++)
            {
                if (modelValues[i] != null)
                {
                    sqlBuilder.Append($" [{modelNames[i]}] = @{modelNames[i]} ");
                    sqlBuilder.Append(", ");
                }
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

            sqlBuilder.Append(this.GenerateWhereSqlScript(key));

            return this.Execute(sqlBuilder.ToString(), MergeObjects(key, model));
        }

        public virtual int Delete<Table>(Table model)
        {
            return this.Delete<Table>((object)model);
        }

        public virtual int Delete<Table>(object model)
        {
            model.ThrowIfNull(nameof(model));

            string tableName = this.FindTableName<Table>();

            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append($" DELETE dbo.{tableName} ");

            sqlBuilder.Append(this.GenerateWhereSqlScript(model));

            return this.Execute(sqlBuilder.ToString(), model);
        }

        public object MergeObjects(params object[] objects)
        {
            var dict = new Dictionary<string, object>();

            foreach (var obj in objects)
            {
                var props = obj.GetType().GetProperties();

                foreach (var prop in props)
                {
                    object value = prop.GetValue(obj);

                    if (value != null)
                    {
                        dict[prop.Name] = value;
                    }
                }
            }

            var mergedObj = new ExpandoObject();
            var mergedObjDict = (IDictionary<string, object>)mergedObj;

            foreach (var kvp in dict)
            {
                mergedObjDict[kvp.Key] = kvp.Value;
            }

            return mergedObj;
        }
    }
}
