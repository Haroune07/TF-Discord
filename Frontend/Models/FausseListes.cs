using System.Collections.Generic;
using System.Windows;
using Shared.DTOs;

namespace Frontend.Models
{
    public static class MockServerData
    {
        public static List<ServerDTO> GetFakeServers()
        {
            return new List<ServerDTO>
            {
                new ServerDTO { Id = "1", Name = "Serveur Europe"},
                new ServerDTO { Id = "2", Name = "Serveur America"},
                new ServerDTO { Id = "3", Name = "Serveur Asia"}
            };
        }

        public static List<ChannelDTO> GetFakeChannels()
            {
                return new List<ChannelDTO>
            {
                new ChannelDTO { Id = "1", ServerId = "1", Name = "📢-annonces" },
                new ChannelDTO { Id = "2", ServerId = "1", Name = "💬-général" },
                new ChannelDTO { Id = "3", ServerId = "1", Name = "🎮-gaming" },
                new ChannelDTO { Id = "4", ServerId = "2", Name = "💻-dev-talk" },
                new ChannelDTO { Id = "5", ServerId = "3", Name = "🤖-bots" }
            };
            }
    }
}
