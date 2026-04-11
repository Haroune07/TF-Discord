using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Shared.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;



        private readonly ChatService _chat = new();
        public ObservableCollection<MessageDTO> Messages { get; } = new();

        public ICommand TestSendCommand { get; }


        public UserDTO? User { get; private set; } = Session.Current.User;

        public bool IsUserOnline => User?.IsOnline == true;
        public string OnlineStatus => IsUserOnline ? "Online" : "Offline";

        public string MemberSince =>
            User != null
                ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
                : "Member since unknown";

        public ICommand? GoToLoginCommand { get; }

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);

            TestSendCommand = new RelayCommand(() => _ = TestSendAsync(), () => true);

            _ = InitChatAsync();
        }

        private void Logout()
        {
            Session.Current.Logout();
            _main!.CurrentViewModel = new LoginViewModel(_main);
        }


        private async Task InitChatAsync()
        {
            try
            {

                _chat.MessageReceived += msg =>
                {
                    Console.WriteLine($"[VM] MessageReceived event fired: {msg.Content}");
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"[VM] Adding to Messages collection");
                        Messages.Add(msg);
                        Console.WriteLine($"[VM] Messages count: {Messages.Count}");
                    });
                };

                await _chat.ConnectAsync();
                await _chat.JoinChannelAsync("test-channel");
            }
            catch (Exception ex)
            {
                // Put a breakpoint here
                System.Diagnostics.Debug.WriteLine($"SignalR error: {ex.Message}");
            }
        }
        public async Task TestSendAsync()
        {
            await _chat.SendMessageAsync("test-channel", "test SignalR");
        }


    }
}