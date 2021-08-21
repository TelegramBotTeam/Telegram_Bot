using BotAdmin.Model;
using BotAdmin.View;
using BotAdmin.View.ViewModel;
using DapperDll;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Telegram.Bot.Types;

namespace BotAdmin.ViewModel
{
    public class BotAdminViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Query> Queries { get; set; }

        private Admin currentAdmin;
        private ChatId chatId;

        private Button acceptButton;
        private Button rejectButton;

        private Query selectedQuery;
        public Query SelectedQuery
        {
            get { return selectedQuery; }
            set 
            {              
                selectedQuery = value; 
                OnPropertyChanged("SelectedQuery");

                if (selectedQuery != null)
                    acceptButton.IsEnabled = rejectButton.IsEnabled = true;
                else
                    acceptButton.IsEnabled = rejectButton.IsEnabled = false;
            }
        }

        private string windowTitle;
        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; OnPropertyChanged("WindowTitle"); }
        }

        private RelayCommand acceptCommand;
        public RelayCommand AcceptCommand
        {
            get 
            { 
                return acceptCommand ?? new RelayCommand(act => 
                { 
                    new QueryWindow(SelectedQuery, chatId).ShowDialog();
                    UpdateQueries();
                }); 
            }
        }

        private RelayCommand rejectCommand;
        public RelayCommand RejectCommand
        {
            get 
            { 
                return rejectCommand ?? new RelayCommand(act => 
                {
                    Query_Repository.Delete(SelectedQuery);
                    UpdateQueries();
                });
            }
        }

        private void UpdateQueries()
        {
            Queries = new ObservableCollection<Query>(Query_Repository.Select().Where(x => x.Query_ChatId == this.currentAdmin.Admin_ChatId));
            OnPropertyChanged("Queries");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public BotAdminViewModel(Admin currentAdmin, ref Button acceptButton, ref Button rejectButton)
        {
            this.currentAdmin = currentAdmin;
            Queries = new ObservableCollection<Query>(Query_Repository.Select().Where(x => x.Query_ChatId == this.currentAdmin.Admin_ChatId));

            this.acceptButton = acceptButton;
            this.rejectButton = rejectButton;

            chatId = new ChatId(Logic.chats.Find(x => x.Chat_Id == this.currentAdmin.Admin_ChatId).Chat_TelegramId);
            WindowTitle = Logic.client.GetChatAsync(chatId).Result.Title
                + " : " + Logic.client.GetChatMemberAsync(chatId, this.currentAdmin.Admin_TelegramId).Result.User.Username;
        }
    }
}
