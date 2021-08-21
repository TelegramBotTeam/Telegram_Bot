using BotAdmin.Model;
using BotAdmin.ViewModel;
using DapperDll;
using System.Windows;
using Telegram.Bot.Types;

namespace BotAdmin.View
{
    /// <summary>
    /// Логика взаимодействия для QueryWindow.xaml
    /// </summary>
    public partial class QueryWindow : Window
    {
        public QueryWindow()
        {
            InitializeComponent();
        }

        public QueryWindow(Query selectedQuery, ChatId chatId)
        {
            InitializeComponent();
            this.DataContext = new QueryWindowViewModel(selectedQuery, chatId);          
        }
    }
}
