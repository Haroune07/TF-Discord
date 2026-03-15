using System.Windows;
using System.Windows.Controls;
using Frontend.ViewModels;

namespace Frontend.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void BackToRegister_Click(object sender, RoutedEventArgs e)
        {
            (MainWindow.GetWindow(this) as MainWindow)?.MainContent.Content = new RegisterView();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel)
            {
                ((LoginViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
