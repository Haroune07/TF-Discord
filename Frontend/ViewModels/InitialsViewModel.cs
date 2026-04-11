using Frontend.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public class InitialsViewModel
    {
        public string Initials { get; set; } = Session.Current.User.Username.Substring(0, 2).ToUpper();
    }
}
