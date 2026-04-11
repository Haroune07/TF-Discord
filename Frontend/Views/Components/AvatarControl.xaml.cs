using Frontend.ViewModels;
using Shared.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Views.Components
{
    public partial class AvatarControl : UserControl
    {
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(UserDTO), typeof(AvatarControl),
                new PropertyMetadata(null, OnUserChanged));

        public UserDTO User
        {
            get => (UserDTO)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public AvatarControl()
        {
            InitializeComponent();
            DataContext = new AvatarControlViewModel();
        }

        private static void OnUserChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AvatarControl)d;
            control.DataContext = new AvatarControlViewModel(e.NewValue as UserDTO);
        }
    }
}
