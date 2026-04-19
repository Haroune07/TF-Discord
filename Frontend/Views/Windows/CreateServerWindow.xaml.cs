using Frontend.ViewModels;
using System.Windows;

namespace Frontend.Views
{
    public partial class CreateServerWindow : Window
    {
        public CreateServerWindow()
        {
            InitializeComponent();
            var vm = new CreateServerViewModel();
            vm.OnCreated = () => { DialogResult = true; Close(); };
            DataContext = vm;
        }
    }
}