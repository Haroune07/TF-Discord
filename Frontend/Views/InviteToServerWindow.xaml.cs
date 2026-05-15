using System.Windows;

namespace Frontend.Views
{
    public partial class InviteToServerWindow : Window
    {
        public string Username => UsernameBox.Text.Trim();

        public InviteToServerWindow()
        {
            InitializeComponent();
            UsernameBox.Focus();
        }

        private void Invite_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Entrez un nom d'utilisateur.", "Invitation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
