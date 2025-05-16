using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Archer.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        /// <summary>
        /// Base query. T is generic
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        List<Table> Query<Table>(string sql = "", object param = null, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        Table QuerySingle<Table>(string sql = "", object param = null, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        /// <summary>
        /// 幫助產生 where 敘述，會自動產生 [參數=@參數] 的敘述
        /// <br></br>
        /// ex: T tObject = { book= "clean code", bookId= "1" }
        /// <br></br>
        /// string whereSqlScript = GenerateWhereSqlScript{T}(tObject);
        /// <br></br>
        /// Then, 
        /// whereSqlScript's value is " WHERE 1=1 AND book=@book AND bookId=@bookId "
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tObject"></param>
        /// <returns></returns>
        string GenerateWhereSqlScript<Table>(Table tObject);

        string GenerateWhereSqlScript(object tObject);

        string FindTableName<Table>(string regexPattern = "^(.+)$");

        int Create<Table>(Table model, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        int Create<Table>(object model, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        void Create<Table>(IEnumerable<Table> modelList, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.KeepIdentity);

        int Update<Table>(Table model, Table key, List<RepositoryOption> options = null, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        int Update<Table>(object model, object key, List<RepositoryOption> options = null, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        int Delete<Table>(Table model, IsolationLevel isolationLevel = IsolationLevel.Serializable);

        int Delete<Table>(object model, IsolationLevel isolationLevel = IsolationLevel.Serializable);
    }
}
