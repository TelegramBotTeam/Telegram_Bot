using System;
using System.Windows;
using System.Windows.Media;

namespace BotAdmin.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            this.DataContext = new AuthViewModel(this);

            this.Background = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
            InitializeComponent();

            string themeName = "AuthTheme";

            var uri = new Uri(@"View/AuthWindowFolder/" + themeName + ".xaml", UriKind.Relative);
            ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
