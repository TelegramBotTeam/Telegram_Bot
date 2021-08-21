using BotAdmin.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BotAdmin.ViewModel
{
    public class ChatIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Logic.client.GetChatAsync(Logic.chats.Find(x => x.Chat_Id == int.Parse(value.ToString())).Chat_TelegramId).Result.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
