using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDll
{
    public class Admin
    {
        public int Admin_Id { get; set; }
        public long Admin_TelegramId { get; set; }
        public string Admin_Login { get; set; }
        public string Admin_Password { get; set; }
        public long Admin_ChatId { get; set; }
    }

    public class Admin_Repository
    {
        private static readonly string ConnStr = ConfigurationManager.ConnectionStrings["bot_db"].ConnectionString;

        public static List<Admin> Select()
        {
            const string procedure = "EXEC [SelectAdmin]";

            using (IDbConnection db = new SqlConnection(ConnStr))
            {
                db.Open();
                List<Admin> admins;

                try
                {
                    admins = db.Query<Admin>(procedure).ToList();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                return admins;
            }
        }

        public static void Delete(Admin value)
        {
            const string procedure = "EXEC [DeleteAdmin] @Admin_Id";
            var values = new { Admin_Id = value.Admin_Id };

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

        public static void Insert(Admin value)
        {
            const string procedure = "EXEC [InsertAdmin] @Admin_TelegramId, @Admin_Login, @Admin_Password, @Admin_ChatId";
            var values = new { Admin_TelegramId = value.Admin_TelegramId, Admin_Login = value.Admin_Login, Admin_Password = value.Admin_Password, Admin_ChatId = value.Admin_ChatId };

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

        public static void Update(Admin oldValue, Admin newValue)
        {
            const string procedure = "EXEC [UpdateAdmin] @Admin_Id, @Admin_TelegramId, @Admin_Login, @Admin_Password";
            var values = new { Admin_Id = oldValue.Admin_Id, Admin_TelegramId = newValue.Admin_TelegramId, Admin_Login = newValue.Admin_Login, Admin_Password = newValue.Admin_Password };

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
