using Frontend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend.Views
{
    /// <summary>
    /// Logique d'interaction pour SearchUserView.xaml
    /// </summary>
    public partial class SearchUserView : UserControl
    {
        public SearchUserView()
        {
            InitializeComponent();
            this.DataContext = new SearchUserViewModel();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Affiche le texte dans la console de sortie (Output) de Visual Studio
            var textBox = sender as TextBox;
            System.Diagnostics.Debug.WriteLine($"Touche pressée: {e.Key}, Texte: {textBox.Text}");

            // Force l'appel manuel pour tester si le ViewModel est là
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
