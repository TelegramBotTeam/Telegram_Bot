using BotAdmin.ViewModel;
using DapperDll;
using System.Windows;

namespace BotAdmin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Admin currentAdmin)
        {
            InitializeComponent();
            this.DataContext = new BotAdminViewModel(currentAdmin, ref acceptButton, ref rejectButton);
        }
    }
}
