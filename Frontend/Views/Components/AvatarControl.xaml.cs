using Frontend.Global;
using Shared.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Views.Components
{
    public partial class AvatarControl : UserControl
    {
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register(nameof(User), typeof(UserDTO), typeof(AvatarControl),
                new PropertyMetadata(null, OnUserChanged));

        public static readonly DependencyProperty InitialsProperty =
            DependencyProperty.Register("Initials", typeof(string), typeof(AvatarControl), new PropertyMetadata(""));

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(AvatarControl), new PropertyMetadata(false));

        public static readonly DependencyProperty AvatarImageProperty =
            DependencyProperty.Register("AvatarImage", typeof(Uri), typeof(AvatarControl), new PropertyMetadata(null));

        public static readonly DependencyProperty AvatarOpacityProperty =
            DependencyProperty.Register("AvatarOpacity", typeof(double), typeof(AvatarControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty OnlineStatusImageProperty =
            DependencyProperty.Register("OnlineStatusImage", typeof(string), typeof(AvatarControl), new PropertyMetadata(null));

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

        public Uri AvatarImage
        {
            get => (Uri)GetValue(AvatarImageProperty);
            set => SetValue(AvatarImageProperty, value);
        }

        public double AvatarOpacity
        {
            get => (double)GetValue(AvatarOpacityProperty);
            set => SetValue(AvatarOpacityProperty, value);
        }

        public string OnlineStatusImage
        {
            get => (string)GetValue(OnlineStatusImageProperty);
            set => SetValue(OnlineStatusImageProperty, value);
        }

        public AvatarControl()
        {
            InitializeComponent();
        }

        private static void OnUserChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AvatarControl)d;
            var user = e.NewValue as UserDTO;
            control.UpdateDisplay(user);
        }

        public void UpdateDisplay(UserDTO user)
        {
            if (user == null) return;

            string name = user.Username ?? "??";
            Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
            IsOnline = user.IsOnline;

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                if (Uri.TryCreate(user.ProfileImageUrl, UriKind.Absolute, out Uri uri))
                {
                    AvatarImage = uri;
                    AvatarOpacity = 1;
                }
            }
            else
            {
                AvatarImage = null;
                AvatarOpacity = 0;
            }

            OnlineStatusImage = user.IsOnline ? "/Static/Images/online.png" : "/Static/Images/invisible.png";
        }
    }
}
