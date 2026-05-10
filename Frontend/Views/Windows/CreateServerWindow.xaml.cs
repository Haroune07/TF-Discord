using Frontend.ViewModels;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.Views
{
    public partial class CreateServerWindow : Window
    {
        public CreateServerWindow()
        {
            InitializeComponent();
            var vm = App.Services.GetRequiredService<CreateServerViewModel>();
            vm.OnCreated = () => { DialogResult = true; Close(); };
            DataContext = vm;
        }
    }
}
