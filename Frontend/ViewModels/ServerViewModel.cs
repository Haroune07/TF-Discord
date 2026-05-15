using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Frontend.ViewModels
{
    public  partial class  ServerViewModel : ObservableObject
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string? ServerImageUrl { get; set; }
        public string Initials => Name.Length >= 2 ? Name.Substring(0, 2).ToUpper() : Name.ToUpper();
        public IRelayCommand SelectCommand { get; }

        [ObservableProperty]
        private bool isSelected;

        public ServerViewModel(string name, string id, Func<string, Task> onSelected, string? serverImageUrl = null)
        {
            Name = name;
            Id = id;
            ServerImageUrl = serverImageUrl;
            SelectCommand = new AsyncRelayCommand(() => onSelected(Id));
        }
    }
}