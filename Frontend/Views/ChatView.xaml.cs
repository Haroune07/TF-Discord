using Frontend.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frontend.Views
{
    public partial class ChatView : UserControl
    {
        private INotifyCollectionChanged? _messagesCollection;

        public ChatView()
        {
            InitializeComponent();

            DataContextChanged += ChatView_DataContextChanged;
            Unloaded += ChatView_Unloaded;
        }

        private void ChatView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_messagesCollection != null)
                _messagesCollection.CollectionChanged -= Messages_CollectionChanged;

            _messagesCollection = null;

            if (DataContext is ChatViewModel vm && vm.Messages is INotifyCollectionChanged messages)
            {
                _messagesCollection = messages;
                _messagesCollection.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            Dispatcher.InvokeAsync(() =>
            {
                if (ChatScrollViewer.VerticalOffset >= ChatScrollViewer.ScrollableHeight - 50)
                {
                    ChatScrollViewer.ScrollToEnd();
                }
            });
        }

        private void ChatView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_messagesCollection != null)
                _messagesCollection.CollectionChanged -= Messages_CollectionChanged;
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