

using Shared.DTOs;

namespace Frontend.Global
{
    internal class Session
    {
        private static Session? _session;
        public static Session Current
        {
            get
            {
                if (_session == null)
                {
                    _session = new();
                }

                return _session;
            }
        } 

        private Session() { }
        public UserDTO? User { get; private set; }

        public void Login(UserDTO user)
        {
            User = user;
        }

        public void Logout()
        {
            User = null;
        }

        public bool IsLoggedIn => User != null;

    }
}
