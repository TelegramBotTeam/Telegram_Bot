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
        public ObservableCollection<Moderator> Moderators { get; set; }

        private Admin currentAdmin;
        private ChatId chatId;

        private Button acceptButton;
        private Button rejectButton;

        private Button deleteButton;

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

        private Moderator selectedModerator;
        public Moderator SelectedModerator
        {
            get { return selectedModerator; }
            set
            {
                selectedModerator = value;
                OnPropertyChanged("SelectedModerator");

                if (selectedModerator != null)
                    deleteButton.IsEnabled = true;
                else
                    deleteButton.IsEnabled = false;
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

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? new RelayCommand(act =>
                {
                    Moderator_Repository.Delete(SelectedModerator);
                    UpdateModerators();
                });
            }
        }


        private void UpdateQueries()
        {
            Queries = new ObservableCollection<Query>(Query_Repository.Select().Where(x => x.Query_ChatId == currentAdmin.Admin_ChatId));
            OnPropertyChanged("Queries");
        }

        private void UpdateModerators()
        {
            Moderators = new ObservableCollection<Moderator>(Moderator_Repository.Select().Where(x => x.Moderator_ChatId == currentAdmin.Admin_ChatId));
            OnPropertyChanged("Moderators");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public BotAdminViewModel(Admin currentAdmin, ref Button acceptButton, ref Button rejectButton, ref Button deleteButton)
        {
            this.currentAdmin = currentAdmin;
            UpdateQueries();
            UpdateModerators();
            //Queries = new ObservableCollection<Query>(Query_Repository.Select());

            this.acceptButton = acceptButton;
            this.rejectButton = rejectButton;
            this.deleteButton = deleteButton;

            Logic.chat = Chat_Repository.Select().Find(x => x.Chat_Id == currentAdmin.Admin_ChatId);
            chatId = new ChatId(Logic.chat.Chat_TelegramId);
            WindowTitle = Logic.client.GetChatAsync(chatId).Result.Title
                + " : " + Logic.client.GetChatMemberAsync(chatId, this.currentAdmin.Admin_TelegramId).Result.User.Username;
        }
    }
}
