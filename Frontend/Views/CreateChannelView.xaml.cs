using Frontend.ViewModels;
using System.Windows;

namespace Frontend.Views
{
    public partial class CreateChannelView : Window
    {
        public string ChannelName { get; private set; } = string.Empty;

        public CreateChannelView()
        {
            InitializeComponent();

            if (DataContext is CreateChannelViewModel vm)
            {
                vm.CloseRequested += result =>
                {
                    if (result)
                        ChannelName = vm.ChannelName.Trim();

                    DialogResult = result;
                    Close();
                };
            }
        }
    }
}