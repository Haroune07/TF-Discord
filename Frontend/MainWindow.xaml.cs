using System.Windows;
using System.Windows.Controls;
using Frontend.Views;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContent.Content = new RegisterView();
        }
        public void NavigateTo(UserControl dest)
        {
            MainContent.Content = dest;
        }
    }
}

    