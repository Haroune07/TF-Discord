using Shared.Constants;

namespace Shared.Utils
{
    public static class Validator
    {

        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;


            if (username.Length > Auth.MaxUsernameLength)
                return false;

            return true;
        }


        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;


            if (password.Length < Auth.MinPasswordLength)
                return false;

            return true;
        }


        public static bool IsValidMessageContent(string content)
        {

            return !string.IsNullOrWhiteSpace(content);
        }
    }
}
