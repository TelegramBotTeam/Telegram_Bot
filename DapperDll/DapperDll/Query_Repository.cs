using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDll
{
    public class Query
    {
        public int Query_Id { get; set; }
        public string Query_TelegramId { get; set; }
        public string Query_Text { get; set; }
        public int Query_ChatId { get; set; }
    }

    public class Query_Repository
    {
        private static readonly string ConnStr = ConfigurationManager.ConnectionStrings["bot_db"].ConnectionString;

        public static List<Query> Select()
        {
            const string procedure = "EXEC [SelectQuery]";

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();
                List<Query> queries;

                try
                {
                    queries = db.Query<Query>(procedure).ToList();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                return queries;
            }
        }

        public static void Delete(Query value)
        {
            const string procedure = "EXEC [DeleteQuery] @Query_Id";
            var values = new { Query_Id = value.Query_Id };

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Query(procedure, values, transaction);
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static void Insert(Query value)
        {
            const string procedure = "EXEC [InsertQuery] @Query_TelegramId, @Query_Text, @Query_ChatId";
            var values = new { Query_TelegramId = value.Query_TelegramId, Query_Text = value.Query_Text, Query_ChatId = value.Query_ChatId };

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Query(procedure, values, transaction);
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static void Update(Query oldValue, Query newValue)
        {
            const string procedure = "EXEC [UpdateQuery] @Query_Id, @Query_TelegramId, @Query_Text";
            var values = new { Query_Id = oldValue.Query_Id, Query_TelegramId = newValue.Query_TelegramId, Query_Text = newValue.Query_Text };

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();

                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Query(procedure, values, transaction);
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}
