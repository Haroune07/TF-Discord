using Frontend.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frontend.Views
{
    public partial class SearchUserView : UserControl
    {
        public SearchUserView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            System.Diagnostics.Debug.WriteLine($"Touche pressée: {e.Key}, Texte: {textBox.Text}");

            if (DataContext is SearchUserViewModel vm)
            {
                _ = vm.LoadUserInvite(textBox.Text);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERREUR : DataContext n'est pas un SearchUserViewModel !");
            }
        }
    }
}
