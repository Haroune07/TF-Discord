using Backend.Src.Models;
using Backend.Src.Repository;
using Shared.DTOs;
using Shared.DTOs.Requests;
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

        public async Task<ServerDTO> CreateServerAsync(CreateServerRequest req)
        {

            // on ne fait pas de vérification de nom car deux serveurs peuvent avoir le même nom dans l'original
            var server = new Server
            {
                Name = req.Name,
                OwnerId = req.OwnerId,
                CreatedAt = DateTime.UtcNow
            };

            await _servers.InsertAsync(server);

            // Le créateur devient owner du serveur
            var member = new ServerMember
            {
                ServerId = server.Id,
                UserId = req.OwnerId,
                Role = MemberRole.Owner,
                JoinedAt = DateTime.UtcNow
            };

            await _members.InsertAsync(member);

            return MapToDTO(server);
        }

        public async Task<List<ServerDTO>> GetUserServersAsync(string userId)
        {
            var memberships = await _members.FindAsync(m => m.UserId == userId);
            var serverIds = memberships.Select(m => m.ServerId).ToList();

            var servers = await _servers.FindAsync(s => serverIds.Contains(s.Id));

            return servers.Select(serv => MapToDTO(serv)).ToList();
        }

        // JOIN SERVER
        public async Task JoinServerAsync(JoinOrLeaveServerRequest req)
        {
            // vérifier si l'utilisateur est déjà membre
            var existing = await _members.FindAsync(m =>
                m.ServerId == req.ServerId && m.UserId == req.UserId);

            if (existing.Any())
                throw new Exception("User already in server");

            var member = new ServerMember
            {
                ServerId = req.ServerId,
                UserId = req.UserId,
                Role = MemberRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            await _members.InsertAsync(member);
        }

        public async Task LeaveServer(JoinOrLeaveServerRequest req)
        {
            var members = await _members.FindAsync(m =>
                m.ServerId == req.ServerId && m.UserId == req.UserId);

            var member = members.FirstOrDefault();

            if (member == null)
                throw new Exception("User not in server");

            await _members.DeleteAsync(member.Id);
        }

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