using System.Windows;
using System.Windows.Controls;
using Shared.DTOs;
using Frontend.ViewModels;

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
            // Par défaut, s'initialise sur la session
            this.DataContext = new AvatarControlViewModel();
        }

        private static void OnUserChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AvatarControl)d;
            if (e.NewValue is UserDTO user)
            {
                // On injecte le nouvel utilisateur dans un nouveau VM
                control.DataContext = new AvatarControlViewModel(user);
            }
        }
    }
}