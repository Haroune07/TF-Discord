using System.Collections.Generic;
using System.Windows;

namespace Frontend.Models
{
    public static class MockServerData
    {
        public static List<Server> GetFakeServers()
        {
            return new List<Server>
            {
                new Server { Id = 1, Name = "Serveur Europe"},
                new Server { Id = 2, Name = "Serveur America"},
                new Server { Id = 3, Name = "Serveur Asia"}
            };
        }
    }
}
