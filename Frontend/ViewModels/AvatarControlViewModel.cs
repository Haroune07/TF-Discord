using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Frontend.Global;

namespace Frontend.ViewModels
{
    public class AvatarControlViewModel
    {
        public string Initials { get; set; } = Session.Current.User.Username.Substring(0, 2).ToUpper();

        public string AvatarImage { get; set; } = Session.Current.User.ProfileImageUrl;

        public int AvatarOpacity { get; set; }

        public AvatarControlViewModel() 
        { 
            if(AvatarImage == null)
            {
                AvatarOpacity = 0;
            }
            else { AvatarOpacity = 1; }
        }
    }
}
