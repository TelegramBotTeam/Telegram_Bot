using BotAdmin.Model;
using BotAdmin.View;
using DapperDll;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types;

namespace BotAdmin.ViewModel
{
    public class QueryWindowViewModel : INotifyPropertyChanged
    {
        private Query selectedQuery;
        private ChatId chatId;

        private string windowTitle;
        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; OnPropertyChanged("WindowTitle"); }
        }

        private string login;
        public string Login
        {
            get { return login; }
            set { login = value; OnPropertyChanged("Login"); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged("Password"); }
        }

        private RelayCommand<QueryWindow> addCommand;
        public RelayCommand<QueryWindow> AddCommand
        {
            get
            {
                return addCommand ?? new RelayCommand<QueryWindow>(act =>
                {
                    Moderator_Repository.Insert(new Moderator()
                    {
                        Moderator_TelegramId = selectedQuery.Query_TelegramId,
                        Moderator_Login = Login,
                        Moderator_Password = Password,
                        Moderator_ChatId = selectedQuery.Query_ChatId
                    });

                    Query_Repository.Delete(selectedQuery);

                    Logic.client.SendTextMessageAsync(selectedQuery.Query_TelegramId, 
                        $"Вы стали модератором в группе {Logic.client.GetChatAsync(chatId).Result.Title}! \nВаш логин: {Login} \nВаш пароль: {Password}");

                    act.Close();
                });               
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public QueryWindowViewModel(Query selectedQuery, ChatId chatId)
        {
            this.selectedQuery = selectedQuery;
            this.chatId = chatId;

            WindowTitle = Logic.client.GetChatAsync(this.chatId).Result.Title
                + " : " + Logic.client.GetChatMemberAsync(this.chatId, this.selectedQuery.Query_TelegramId).Result.User.Username;
        }
    }
}
