using DapperDll;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using Telegram.Bot;

namespace BotModerator.Model
{
    public static class Logic
    {
        public static ITelegramBotClient client = new TelegramBotClient(ConfigurationManager.ConnectionStrings["bot_key"].ConnectionString);
        //public static List<Chat> chats = Chat_Repository.Select();
        public static Chat chat;

        internal static bool Auth(string loginStr, string passStr)
        {
            List<Moderator> moderators = Moderator_Repository.Select();
            Moderator currentModerator;

            if ((currentModerator = moderators.Find(x => x.Moderator_Login == loginStr && x.Moderator_Password == passStr)) == null)
                return false;

            new MainWindow(currentModerator).Show();            
            return true;
        }

        internal static void SendToGroup(Telegram.Bot.Types.ChatId chatId, string memberText)
        {
            Logic.client.SendTextMessageAsync(chatId, memberText);
        }

        internal static void SendToMember(Member selectedMember, string memberText)
        {
            Logic.client.SendTextMessageAsync(selectedMember.Member_TelegramId, memberText);
        }

        internal static void SendToMembers(ObservableCollection<Member> members, string memberText)
        {
            foreach (Member item in members)
            {
                Logic.client.SendTextMessageAsync(item.Member_TelegramId, memberText);
            }
        }

        internal static void KickMember(Telegram.Bot.Types.ChatId chatId, string memberText, Member selectedMember)
        {
            Logic.client.KickChatMemberAsync(chatId, selectedMember.Member_TelegramId);
            Logic.client.SendTextMessageAsync(selectedMember.Member_TelegramId, memberText);
            Member_Repository.Delete(selectedMember);
        }
    }
}
