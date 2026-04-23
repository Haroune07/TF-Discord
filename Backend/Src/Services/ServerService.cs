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
        private readonly IRepository<User> _users;

        public ServerService(
            IRepository<Server> servers,
            IRepository<ServerMember> members,
            IRepository<User> users)
        {
            _servers = servers;
            _members = members;
            _users = users;
        }

        public async Task<ServerDTO> CreateServerAsync(CreateServerRequest req)
        {

            // on ne fait pas de vérification de nom car deux serveurs peuvent avoir le même nom dans l'original
            var server = new Server
            {
                Name = req.Name,
                OwnerId = req.OwnerId,
                CreatedAt = DateTime.UtcNow,
                ServerImageUrl = req.ServerImageUrl
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
                CreatedAt = server.CreatedAt,
                ServerImageUrl = server.ServerImageUrl
            };
        }

        public async Task<List<ServerDTO>> GetAllServersAsync()
        {
            return (await _servers.GetAllAsync()).Select(MapToDTO).ToList();
        }

        public async Task<List<ServerMemberDTO>> GetServerMembersAsync(string serverId) 
        {
            var members = await _members.FindAsync(member => member.ServerId == serverId);

            var serverMembers = new List<ServerMemberDTO>();
            foreach (var member in members) {
                
                var user = await _users.GetByIdAsync(member.UserId);
                if (user is null) continue;

                serverMembers.Add(new ServerMemberDTO
                {
                    Id = member.Id,
                    ServerId = member.ServerId,
                    Role = member.Role,
                    JoinedAt = member.JoinedAt,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        IsOnline = user.IsOnline,
                        CreatedAt = user.CreatedAt,
                        ProfileImageUrl = user.ProfileImageUrl
                    }
                });

            }

            return serverMembers;

        }

        private async Task<ServerMember?> GetMemberAsync(string serverId, string userId)
        {
            var matches = await _members.FindAsync(m => m.ServerId == serverId && m.UserId == userId);
            return matches.FirstOrDefault();
        }


        public async Task<bool> UpdateMemberRoleAsync(string serverId, string targetUserId, UpdateMemberRoleRequest req)
        {
            var requester = await GetMemberAsync(serverId, req.RequesterId);

            if (requester is null || requester.Role == MemberRole.Member) return false;

            var targetUser = await GetMemberAsync(serverId, targetUserId);

            if (targetUser is null) return false;

            if (targetUser.Role == MemberRole.Owner) return false;

            targetUser.Role = req.NewRole;
            await _members.UpdateAsync(targetUser.Id, targetUser);
            return true;

        }


        public async Task<bool> KickMemberAsync(string serverId, string targetUserId, KickMemberRequest req)
        {

            var requester = await GetMemberAsync(serverId, req.RequesterId);
            if (requester is null || requester.Role != MemberRole.Owner || requester.Role != MemberRole.Admin) return false;

            var targetUser = await GetMemberAsync(serverId, targetUserId);
            if (targetUser is null) return false;

            if (requester.Role == MemberRole.Admin && targetUser.Role != MemberRole.Member) return false;

            await _members.DeleteAsync(targetUser.Id);
            return true;

        }

    }
}