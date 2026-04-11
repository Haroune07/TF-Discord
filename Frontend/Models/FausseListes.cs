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

        public static List<Channel> GetFakeChannels()
            {
                return new List<Channel>
            {
                new Channel { Id = 1, ServerId = 1, Name = "📢-annonces" },
                new Channel { Id = 2, ServerId = 1, Name = "💬-général" },
                new Channel { Id = 3, ServerId = 1, Name = "🎮-gaming" },
                new Channel { Id = 4, ServerId = 2, Name = "💻-dev-talk" },
                new Channel { Id = 5, ServerId = 3, Name = "🤖-bots" }
            };
            }
    }
}
