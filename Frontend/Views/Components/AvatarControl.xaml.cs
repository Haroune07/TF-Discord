using Frontend.Global;
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

        public static readonly DependencyProperty InitialsProperty =
            DependencyProperty.Register("Initials", typeof(string), typeof(AvatarControl), new PropertyMetadata(""));

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(AvatarControl), new PropertyMetadata(false));

        public UserDTO User
        {
            get => (UserDTO)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public string Initials
        {
            get => (string)GetValue(InitialsProperty);
            set => SetValue(InitialsProperty, value);
        }

        public bool IsOnline
        {
            get => (bool)GetValue(IsOnlineProperty);
            set => SetValue(IsOnlineProperty, value);
        }

        public AvatarControl()
        {
            InitializeComponent();
        }

        private static void OnUserChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AvatarControl)d;
            var user = e.NewValue as UserDTO ?? Session.Current.User;

            if (user != null)
            {
                string name = user.Username ?? "??";
                control.Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
                control.IsOnline = user.IsOnline;
            }
        }
    }
}
