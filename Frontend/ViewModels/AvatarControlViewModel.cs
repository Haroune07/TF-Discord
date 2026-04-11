using Frontend.Global;
using Frontend.ViewModels.Base;
using Shared.DTOs;

namespace Frontend.ViewModels
{
    public class AvatarControlViewModel : BaseViewModel
    {
        public string Initials { get; set; } = string.Empty;
        public string? AvatarImage { get; set; }
        public double AvatarOpacity { get; set; }
        public bool IsOnline { get; set; }

        //le userDTO est passé en parametre ca nous permet de reutiliser aillerus (msg, dm, user status)
        public AvatarControlViewModel(UserDTO? user = null)
        {
            user ??= Session.Current.User; // Si pas d'user fourni, on prend moi

            if (user != null)
            {
                string name = user.Username ?? "??";
                Initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
                AvatarImage = user.ProfileImageUrl;
                IsOnline = user.IsOnline;
                AvatarOpacity = string.IsNullOrEmpty(AvatarImage) ? 0 : 1;
            }
        }
    }
}
