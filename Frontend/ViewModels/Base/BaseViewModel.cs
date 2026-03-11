// ViewModels/Base/ViewModelBase.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Frontend.ViewModels.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}