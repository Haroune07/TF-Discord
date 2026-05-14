using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace Frontend.ViewModels
{
    public partial class CreateChannelViewModel : ObservableObject
    {
        [ObservableProperty]
        private string channelName = string.Empty;

        public IRelayCommand ConfirmCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public event Action<bool>? CloseRequested;

        public CreateChannelViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
        }

        private void Confirm()
        {
            if (string.IsNullOrWhiteSpace(ChannelName))
            {
                MessageBox.Show("Veuillez entrer un nom de canal.");
                return;
            }

            CloseRequested?.Invoke(true);
        }
    }
}