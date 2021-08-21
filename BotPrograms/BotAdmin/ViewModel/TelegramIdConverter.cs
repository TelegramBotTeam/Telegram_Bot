using BotAdmin.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using Telegram.Bot.Types;

namespace BotAdmin.ViewModel
{
    //public class TelegramIdConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return Logic.client.GetChatMemberAsync(new ChatId(Logic.chat.Chat_TelegramId), long.Parse(values[0].ToString())).Result.User.Username;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        return (object[])Binding.DoNothing;
    //    }
    //}

    public class TelegramIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Logic.client.GetChatMemberAsync(new ChatId(Logic.chat.Chat_TelegramId), long.Parse(value.ToString())).Result.User.Username;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
