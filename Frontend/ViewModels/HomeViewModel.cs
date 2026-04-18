using Frontend.Commands;
using Frontend.Global;
using Frontend.Services;
using Frontend.ViewModels.Base;
using Frontend.Views;
using Shared.DTOs;
using System.Net.Http;
using System.Windows.Input;
using System.Net.Http.Json;


namespace Frontend.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private MainViewModel? _main;

        public UserDTO? User { get; private set; } = Session.Current.User;
        public AvatarControlViewModel CurrentUserAvatar { get; set; }

        public ServerListViewModel? ServerList => _main?.ServerList;
        public ChannelListViewModel? ChannelList => _main?.ChannelList;
        public UserListViewModel UserList { get; }

        public ChatViewModel ActiveChat { get; }

        private bool _isDMMode = false;
        public bool IsDMMode
        {
            get => _isDMMode;
            set { 
                _isDMMode = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(IsServerMode));
                System.Diagnostics.Debug.WriteLine($"IsDMMode={_isDMMode}, IsServerMode={!_isDMMode}");
            }
        }
        public bool IsServerMode => !IsDMMode;
        public string MemberSince => User != null
            ? $"Member since {User.CreatedAt:MMMM dd, yyyy}"
            : "Member since unknown";

        public ICommand? GoToLoginCommand { get; }
        public ICommand? GoToHomeCommand { get; }

        public ICommand UploadAvatarCommand { get; }
        public ICommand SetOnlineStatusCommand { get; }

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToLoginCommand = new RelayCommand(Logout, () => true);
            GoToHomeCommand = new RelayCommand(SwitchToDMMode, () => true);
            UploadAvatarCommand = new RelayCommand(OpenUrlInputDialog, () => true);
            SetOnlineStatusCommand = new RelayCommand<string>(parameter => SetOnlineStatus(parameter),parameter => true);

            var apiService = new ApiService();
            var chatService = new ChatService();
            ActiveChat = new ChatViewModel(apiService, chatService);

            CurrentUserAvatar = new(User);

            UserList = new UserListViewModel(async (channelId) =>
            {
                IsDMMode = true;
                await ActiveChat.LoadChannelAsync(channelId);
            });

            _ = chatService.ConnectAsync();

            if (_main?.ChannelList != null)
                _main.ChannelList.OnChannelSelected += async (id) => await SelectChannelAsync(id);

            if (_main?.ServerList != null)
            {
                _main.ServerList.OnServerSelected += (id) => IsDMMode = false;
                _ = _main.ServerList.LoadServersAsync();
            }
        }

        private async void SetOnlineStatus(string status)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = $"http://localhost:8080/api/user/{User.Id}/update-status";

                    // Cette ligne transforme Online en "Online" (avec guillemets)
                    // C'est ce que [FromBody] string attend pour ne pas faire d'erreur 400
                    var jsonStatus = System.Text.Json.JsonSerializer.Serialize(status);
                    var content = new StringContent(jsonStatus, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        User.OnlineStatus = status;
                        System.Diagnostics.Debug.WriteLine("[TEST] Statut mis à jour !");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"[TEST] Erreur : {response.StatusCode} - {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Erreur] : {ex.Message}");
            }
        }



        private void SwitchToDMMode()
        {
            IsDMMode = true;
            _ = UserList.LoadUsersAsync();
        }

        private void Logout()
        {
            Session.Current.Logout();
            _main!.CurrentViewModel = new LoginViewModel(_main);
        }

        public async Task SelectChannelAsync(string channelId)
        {
            IsDMMode = false;
            await ActiveChat.LoadChannelAsync(channelId);
        }

        public async void OpenUrlInputDialog()
        {
            var dialog = new UrlInputWindowView();
            dialog.Owner = App.Current.MainWindow;

            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Url))
            {
                if (User != null)
                {
                    // --- PRINT DE TEST ---
                    System.Diagnostics.Debug.WriteLine($"[TEST] Tentative de modification pour : {User.Username}");
                    System.Diagnostics.Debug.WriteLine($"[TEST] Ancienne URL : {User.ProfileImageUrl}");
                    System.Diagnostics.Debug.WriteLine($"[TEST] Nouvelle URL demandée : {dialog.Url}");

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var apiUrl = $"http://localhost:8080/api/user/{User.Id}/update-pfp";

                            
                            var response = await client.PutAsJsonAsync(apiUrl, dialog.Url);

                            System.Diagnostics.Debug.WriteLine($"[TEST] Réponse du serveur : {response.StatusCode}");

                            if (response.IsSuccessStatusCode)
                            {
                                User.ProfileImageUrl = dialog.Url;
                                System.Diagnostics.Debug.WriteLine("[TEST] Mise à jour réussie dans l'UI !");
                            }
                            else
                            {
                                var errorBody = await response.Content.ReadAsStringAsync();
                                System.Diagnostics.Debug.WriteLine($"[TEST] Erreur API : {errorBody}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[TEST] CRASH CONNECTION : {ex.Message}");
                    }
                }
            }
        }


    }
}