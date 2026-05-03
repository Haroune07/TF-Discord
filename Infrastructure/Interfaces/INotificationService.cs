namespace Infrastructure.Interfaces
{
    public interface INotificationService
    {
        Task SendLoginNotificationAsync(string toPhoneNumber, string username);
    }
}
