using BotAdmin.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using Telegram.Bot.Types;

namespace BotAdmin.ViewModel
{
    public class TelegramIdConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Logic.client.GetChatMemberAsync(new ChatId(Logic.chats.Find(x => x.Chat_Id == int.Parse(values[1].ToString())).Chat_TelegramId), long.Parse(values[0].ToString())).Result.User.Username;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }

}
