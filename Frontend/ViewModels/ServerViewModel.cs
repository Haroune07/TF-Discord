using Frontend.Commands;
using Frontend.ViewModels;
using Frontend.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class ServerViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public ICommand SelectCommand { get; }

        public ServerViewModel(string name, string id, Action<string> onSelected)
        {
            Name = name;
            Id = id;
            SelectCommand = new RelayCommand(() => onSelected(Id), () => true);
        }
    }
}