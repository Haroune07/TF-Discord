using Frontend.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frontend.Views
{
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && DataContext is ChatViewModel vm && vm.IsEditing)
            {
                vm.CancelEditCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
