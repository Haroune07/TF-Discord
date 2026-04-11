using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs;
using Shared.Enums;

namespace Backend.Src.Services
{
    public class ServerService
    {
        private readonly IRepository<Server> _servers;
        private readonly IRepository<ServerMember> _members;

        public ServerService(
            IRepository<Server> servers,
            IRepository<ServerMember> members)
        {
            _servers = servers;
            _members = members;
        }

        // CREATE SERVER
        public async Task<ServerDTO> CreateServer(string name, string ownerId)
        {
            var server = new Server
            {
                Name = name,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow // date de création automatique
            };

            await _servers.InsertAsync(server);

            // Le créateur devient automatiquement OWNER du serveur
            var member = new ServerMember
            {
                ServerId = server.Id,
                UserId = ownerId,
                Role = MemberRole.Owner,
                JoinedAt = DateTime.UtcNow
            };

            await _members.InsertAsync(member);

            return MapToDTO(server);
        }

        // GET ALL SERVERS
        public async Task<List<ServerDTO>> GetServers()
        {
            var servers = await _servers.GetAllAsync();

            // transformation en DTO pour ne pas exposer le modèle Mongo
            return servers.Select(MapToDTO).ToList();
        }

        // JOIN SERVER
        public async Task JoinServer(string serverId, string userId)
        {
            // vérifier si l'utilisateur est déjà membre
            var existing = await _members.FindAsync(m =>
                m.ServerId == serverId && m.UserId == userId);

            if (existing.Any())
                throw new Exception("User already in server");

            var member = new ServerMember
            {
                ServerId = serverId,
                UserId = userId,
                Role = MemberRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            await _members.InsertAsync(member);
        }

        // LEAVE SERVER
        public async Task LeaveServer(string serverId, string userId)
        {
            var members = await _members.FindAsync(m =>
                m.ServerId == serverId && m.UserId == userId);

            var member = members.FirstOrDefault();

            if (member == null)
                throw new Exception("User not in server");

            await _members.DeleteAsync(member.Id);
        }

        // MAPPING vers DTO (bonne pratique pour séparer modèle et réponse API)
        private ServerDTO MapToDTO(Server server)
        {
            return new ServerDTO
            {
                Id = server.Id,
                Name = server.Name,
                OwnerId = server.OwnerId,
                CreatedAt = server.CreatedAt
            };
        }
    }
}