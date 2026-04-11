using Frontend.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Frontend.ViewModels;

namespace Frontend.ViewModels
{
    public class ServerViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public ICommand? Test { get; }
        public ServerViewModel() 
        {
            Name = string.Empty;
            Test = new RelayCommand(declenche, ()=> true);
        }

        public void declenche()
        {
            ServerSelectionService.SelectServer(this.Id);
        }

    }
}
