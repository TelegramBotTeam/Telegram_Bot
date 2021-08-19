using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDll
{
    public class Moderator
    {
        public int Moderator_Id { get; set; }
        public string Moderator_TelegramId { get; set; }
        public int Moderator_ChatId { get; set; }
    }

    public class Moderator_Repository
    {
        private static readonly string ConnStr = ConfigurationManager.ConnectionStrings["bot_db"].ConnectionString;

        public static List<Moderator> Select()
        {
            const string procedure = "EXEC [SelectModerator]";

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();
                List<Moderator> moderators;

                try
                {
                    moderators = db.Query<Moderator>(procedure).ToList();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                return moderators;
            }
        }

        public static void Delete(Moderator value)
        {
            const string procedure = "EXEC [DeleteModerator] @Moderator_Id";
            var values = new { Moderator_Id = value.Moderator_Id };

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

        public static void Insert(Moderator value)
        {
            const string procedure = "EXEC [InsertModerator] @Moderator_TelegramId, @Moderator_ChatId";
            var values = new { Moderator_TelegramId = value.Moderator_TelegramId, Moderator_ChatId = value.Moderator_ChatId };

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

        public static void Update(Moderator oldValue, Moderator newValue)
        {
            const string procedure = "EXEC [UpdateModerator] @Moderator_Id, @Moderator_TelegramId";
            var values = new { Moderator_Id = oldValue.Moderator_Id, Moderator_TelegramId = newValue.Moderator_TelegramId };

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
