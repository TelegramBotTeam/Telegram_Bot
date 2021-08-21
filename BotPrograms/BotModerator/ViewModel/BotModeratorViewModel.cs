using BotModerator.Model;
using BotModerator.View.ViewModel;
using DapperDll;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Telegram.Bot.Types;

namespace BotModerator.ViewModel
{
    public class BotModeratorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Member> Members { get; set; }

        private Moderator currentModerator;
        private ChatId chatId;

        private Button memberButton;
        private Button kickButton;

        private Member selectedMember;
        public Member SelectedMember
        {
            get { return selectedMember; }
            set 
            { 
                selectedMember = value; 
                OnPropertyChanged("SelectedMember");

                if (selectedMember != null)
                    memberButton.IsEnabled = kickButton.IsEnabled = true;
                else
                    memberButton.IsEnabled = kickButton.IsEnabled = false;
            }
        }       

        private string memberText;
        public string MemberText
        {
            get { return memberText; }
            set { memberText = value; OnPropertyChanged("MemberText"); }
        }

        private string windowTitle;
        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; OnPropertyChanged("WindowTitle"); }
        }

        private RelayCommand groupCommand;
        public RelayCommand GroupCommand
        { 
            get 
            { 
                return groupCommand ?? new RelayCommand(act => 
                { 
                    Logic.SendToGroup(chatId, MemberText);
                    MemberText = string.Empty;
                }); 
            }
        }

        private RelayCommand memberCommand;
        public RelayCommand MemberCommand
        {
            get 
            { 
                return memberCommand ?? new RelayCommand(act => 
                { 
                    Logic.SendToMember(SelectedMember, MemberText);
                    MemberText = string.Empty;
                }); 
            }
        }

        private RelayCommand membersCommand;
        public RelayCommand MembersCommand
        {
            get
            {
                return membersCommand ?? new RelayCommand(act =>
                {
                    Logic.SendToMembers(Members, MemberText);
                    MemberText = string.Empty;
                });
            }
        }

        private RelayCommand kickCommand;
        public RelayCommand KickCommand
        {
            get 
            { 
                return kickCommand ?? new RelayCommand(act => 
                { 
                    Logic.KickMember(chatId, MemberText, SelectedMember);
                    UpdateMembers();
                    MemberText = string.Empty;
                }); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = " ")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void UpdateMembers()
        {
            Members = new ObservableCollection<Member>(Member_Repository.Select().Where(x => x.Member_ChatId == this.currentModerator.Moderator_ChatId));

            Moderator_Repository.Select().ForEach(x => Members.Remove(Members.FirstOrDefault(y => y.Member_TelegramId == x.Moderator_TelegramId)));
            Admin_Repository.Select().ForEach(x => Members.Remove(Members.FirstOrDefault(y => y.Member_TelegramId == x.Admin_TelegramId)));
        }

        public BotModeratorViewModel(Moderator currentModerator, ref Button memberButton, ref Button kickButton)
        {
            this.currentModerator = currentModerator;
            UpdateMembers();

            this.memberButton = memberButton;
            this.kickButton = kickButton;

            Logic.chat = Chat_Repository.Select().Find(x => x.Chat_Id == currentModerator.Moderator_ChatId);
            chatId = new ChatId(Logic.chat.Chat_TelegramId);
            WindowTitle = Logic.client.GetChatAsync(chatId).Result.Title
                + " : " + Logic.client.GetChatMemberAsync(chatId, this.currentModerator.Moderator_TelegramId).Result.User.Username;
        }
    }
}
