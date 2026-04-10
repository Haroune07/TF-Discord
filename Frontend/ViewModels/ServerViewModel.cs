using Frontend.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class ServerViewModel
    {
        public string Name { get; set; }

        public ICommand? Test { get; }
        public ServerViewModel() 
        {
            Name = string.Empty;
            Test = new RelayCommand(declenche, ()=> true);
        }

        public void declenche()
        {
            Console.WriteLine("Déclenché");
        }
    }
}
