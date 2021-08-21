using BotModerator.ViewModel;
using DapperDll;
using System.Windows;

namespace BotModerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Moderator currentModerator)
        {
            InitializeComponent();
            this.DataContext = new BotModeratorViewModel(currentModerator, ref memberButton, ref kickButton);
        }
    }
}
