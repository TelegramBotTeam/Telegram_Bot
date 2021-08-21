using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDll
{
    public class Member
    {
        public int Member_Id { get; set; }
        public long Member_TelegramId { get; set; }
        public long Member_ChatId { get; set; }
    }
    public class Member_Repository
    {
        private static readonly string ConnStr = ConfigurationManager.ConnectionStrings["bot_db"].ConnectionString;

        public static List<Member> Select()
        {
            const string procedure = "EXEC [SelectMember]";

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();
                List<Member> members;

                try
                {
                    members = db.Query<Member>(procedure).ToList();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                return members;
            }
        }

        public static void Delete(Member value)
        {
            const string procedure = "EXEC [DeleteMember] @Member_Id";
            var values = new { Member_Id = value.Member_Id };

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

        public static void Insert(Member value)
        {
            const string procedure = "EXEC [InsertMember] @Member_TelegramId, @Member_ChatId";
            var values = new { Member_TelegramId = value.Member_TelegramId, Member_ChatId = value.Member_ChatId };

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

        public static void Update(Member oldValue, Member newValue)
        {
            const string procedure = "EXEC [UpdateMember] @Member_Id, @Member_TelegramId";
            var values = new { Member_Id = oldValue.Member_Id, Member_TelegramId = newValue.Member_TelegramId };

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
