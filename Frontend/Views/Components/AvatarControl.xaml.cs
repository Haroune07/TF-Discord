using Frontend.Global;
using Shared.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.Views.Components
{
    public partial class AvatarControl : UserControl
    {

        public class UserDTO
        {
            public string Username { get; set; } = string.Empty;
            public bool IsOnline { get; set; }
            public string ProfileImageUrl { get; set; } = string.Empty;
        }
        public static readonly DependencyProperty UserProperty =
        DependencyProperty.Register(nameof(User), typeof(UserDTO), typeof(AvatarControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty InitialsProperty =
            DependencyProperty.Register("Initials", typeof(string), typeof(AvatarControl), new PropertyMetadata(""));

        public static readonly DependencyProperty OnlineStatusProperty =
            DependencyProperty.Register("OnlineStatus", typeof(bool), typeof(AvatarControl), new PropertyMetadata(false));

        public static readonly DependencyProperty AvatarImageProperty =
            DependencyProperty.Register("AvatarImage", typeof(Uri), typeof(AvatarControl), new PropertyMetadata(null));

        public static readonly DependencyProperty AvatarOpacityProperty =
            DependencyProperty.Register("AvatarOpacity", typeof(double), typeof(AvatarControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty OnlineStatusImageProperty =
            DependencyProperty.Register("OnlineStatusImage", typeof(String), typeof(AvatarControl), new PropertyMetadata(null));


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

        public bool OnlineStatus
        {
            get => (bool)GetValue(OnlineStatusProperty);
            set => SetValue(OnlineStatusProperty, value);
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

        public AvatarControl()
        {
            InitializeComponent();
        }

        public String OnlineStatusImage
        {
            get => (String)GetValue(OnlineStatusImageProperty);
            set => SetValue(OnlineStatusImageProperty, value);
        }

        public void UpdateOnlineStatusImage()
        {
            if (User == null) return;


            string status = User.IsOnline ? "/Static/Images/online.png" : "/Static/Images/invisible.png";

            OnlineStatusImage = status;
        }

        private void UpdateDisplay(UserDTO user)
        {
            if (user == null) return;

            string name = user.Username ?? "??";
            Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
            OnlineStatus = user.IsOnline;

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                // On crée l'Uri. Le fait de changer "AvatarImage" va déclencher le Binding XAML
                AvatarImage = new Uri(user.ProfileImageUrl, UriKind.Absolute);
                AvatarOpacity = 1;
            }
            else
            {
                AvatarImage = null;
                AvatarOpacity = 0;
            }
        }

        private void OnUserPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserDTO.IsOnline))
            {
                // On met à jour le texte ET l'image .png
                UpdateDisplay(User);
                UpdateOnlineStatusImage();
            }
            else if (e.PropertyName == nameof(UserDTO.ProfileImageUrl))
            {
                UpdateDisplay(User);
            }
        }



        
    }
}
