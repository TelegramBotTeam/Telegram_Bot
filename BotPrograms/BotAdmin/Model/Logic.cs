using DapperDll;
using System.Collections.Generic;
using System.Configuration;
using Telegram.Bot;

namespace BotAdmin.Model
{
    public static class Logic
    {
        public static ITelegramBotClient client = new TelegramBotClient(ConfigurationManager.ConnectionStrings["bot_key"].ConnectionString);
        public static List<Chat> chats = Chat_Repository.Select();

        internal static bool Auth(string loginStr, string passStr)
        {
            List<Admin> admins = Admin_Repository.Select();
            Admin currentAdmin;

            if ((currentAdmin = admins.Find(x => x.Admin_Login == loginStr && x.Admin_Password == passStr)) == null)
                return false;

            new MainWindow(currentAdmin).Show();
            return true;
        }
    }
}
