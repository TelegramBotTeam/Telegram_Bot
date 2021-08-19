using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDll
{
    public class Chat
    {     
        public int Chat_Id { get; set; }
        public string Chat_TelegramId { get; set; }
    }

    public class Chat_Repository
    {
        private static readonly string ConnStr = ConfigurationManager.ConnectionStrings["bot_db"].ConnectionString;

        public static List<Chat> Select()
        {
            const string procedure = "EXEC [SelectChat]";

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();
                List<Chat> chats;

                try
                {
                    chats = db.Query<Chat>(procedure).ToList();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                return chats;
            }
        }

        public static void Delete(Chat value)
        {
            const string procedure = "EXEC [DeleteChat] @Chat_Id";
            var values = new { Chat_Id = value.Chat_Id };

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

        public static void Insert(Chat value)
        {
            const string procedure = "EXEC [InsertChat] @Chat_TelegramId";
            var values = new { Chat_TelegramId = value.Chat_TelegramId };

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

        public static void Update(Chat oldValue, Chat newValue)
        {
            const string procedure = "EXEC [UpdateChat] @Chat_Id, @Chat_TelegramId";
            var values = new { Chat_Id = oldValue.Chat_Id, Chat_TelegramId = newValue.Chat_TelegramId };

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
