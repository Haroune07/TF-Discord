using Frontend.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            (MainWindow.GetWindow(this) as MainWindow)?.MainContent.Content = new LoginView();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is RegisterViewModel)
            {
                ((RegisterViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
