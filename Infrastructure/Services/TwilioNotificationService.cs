using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Infrastructure.Services
{
    public class TwilioNotificationService : INotificationService
    {
        private readonly ILogger<TwilioNotificationService> _logger;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public TwilioNotificationService(ILogger<TwilioNotificationService> logger, string accountSid, string authToken, string fromNumber)
        {
            _logger = logger;
            _accountSid = accountSid;
            _authToken = authToken;
            _fromNumber = fromNumber;
        }

        public async Task SendLoginNotificationAsync(string toPhoneNumber, string username)
        {
            if (string.IsNullOrWhiteSpace(toPhoneNumber) || string.IsNullOrWhiteSpace(_accountSid) || string.IsNullOrWhiteSpace(_authToken) || string.IsNullOrWhiteSpace(_fromNumber))
                return;

            TwilioClient.Init(_accountSid, _authToken);
            try
            {
                await MessageResource.CreateAsync(
                    body: $"Bonjour {username}, vous venez de vous connecter à Discord-TF.",
                    from: new PhoneNumber(_fromNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "Twilio SMS failed.");
                return;
            }
        }
    }
}
