using Frontend.Global;
using Shared.DTOs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is UserDTO user)
            {
                UpdateDisplay(user);
            }
            else if (e.NewValue is ViewModels.AvatarControlViewModel vm)
            {
                // Bind DP to VM properties
                SetBinding(InitialsProperty, new Binding("Initials") { Source = vm });
                SetBinding(IsOnlineProperty, new Binding("IsOnline") { Source = vm });
                SetBinding(OnlineStatusImageProperty, new Binding("OnlineStatusImage") { Source = vm });
                SetBinding(AvatarImageProperty, new Binding("AvatarImage") { Source = vm, Converter = new StringToUriConverter() });
                SetBinding(AvatarOpacityProperty, new Binding("AvatarOpacity") { Source = vm });
            }
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

            OnlineStatusImage = user.IsOnline 
                ? "pack://application:,,,/Static/Images/online.png" 
                : "pack://application:,,,/Static/Images/invisible.png";
        }
    }

    public class StringToUriConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string url && Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                return uri;
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }
}
