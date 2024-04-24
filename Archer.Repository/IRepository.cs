using System.Collections.Generic;

namespace Archer.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null);

        /// <summary>
        /// Base query. T is generic
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        List<Table> Query<Table>(string sql = "", object param = null);

        Table QuerySingle<Table>(string sql = "", object param = null);

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

        int Create<Table>(Table model);

        int Create<Table>(object model);

        int Update<Table>(Table model, Table key);

        int Update<Table>(object model, object key);

        int Delete<Table>(Table model);

        int Delete<Table>(object model);
    }
}
