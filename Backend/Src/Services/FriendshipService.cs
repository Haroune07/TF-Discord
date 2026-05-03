using Backend.Src.Models;
using Backend.Src.Repository;
using Backend.Src.Mappers;
using Shared.DTOs;
using Shared.Enums;
using Shared.Constants;

namespace Backend.Src.Services
{
    public class FriendshipService
    {
        private readonly IRepository<Friendship> _friendships;
        private readonly IRepository<User> _users;

        public FriendshipService(IRepository<Friendship> friendships, IRepository<User> users)
        {
            _friendships = friendships;
            _users = users;
        }

        public async Task<bool> SendFriendRequestAsync(string requesterId, string receiverUsername)
        {
            var receiver = (await _users.FindAsync(u => u.Username == receiverUsername)).FirstOrDefault();
            if (receiver == null || receiver.Id == requesterId) return false;

            // Check if friendship already exists
            var existing = await _friendships.FindAsync(f => 
                (f.RequesterId == requesterId && f.ReceiverId == receiver.Id) ||
                (f.RequesterId == receiver.Id && f.ReceiverId == requesterId));

            if (existing.Any()) return false;

            var friendship = new Friendship
            {
                RequesterId = requesterId,
                ReceiverId = receiver.Id,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _friendships.InsertAsync(friendship);
            return true;
        }

        public async Task<List<FriendshipDTO>> GetFriendsAsync(string userId)
        {
            var friendships = await _friendships.FindAsync(f => 
                (f.RequesterId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Accepted);

            var friendDTOs = new List<FriendshipDTO>();
            foreach (var f in friendships)
            {
                string friendId = f.RequesterId == userId ? f.ReceiverId : f.RequesterId;
                var friend = await _users.GetByIdAsync(friendId);
                
                if (friend != null)
                {
                    friendDTOs.Add(new FriendshipDTO
                    {
                        Id = f.Id,
                        Friend = friend.ToDTO(),
                        Status = f.Status,
                        CreatedAt = f.CreatedAt
                    });
                }
            }

            return friendDTOs;
        }

        public async Task<List<FriendshipDTO>> GetPendingRequestsAsync(string userId)
        {
            var friendships = await _friendships.FindAsync(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending);

            var requestDTOs = new List<FriendshipDTO>();
            foreach (var f in friendships)
            {
                var requester = await _users.GetByIdAsync(f.RequesterId);
                if (requester != null)
                {
                    requestDTOs.Add(new FriendshipDTO
                    {
                        Id = f.Id,
                        Friend = requester.ToDTO(),
                        Status = f.Status,
                        CreatedAt = f.CreatedAt
                    });
                }
            }

            return requestDTOs;
        }

        public async Task<bool> UpdateFriendshipStatusAsync(string friendshipId, string userId, FriendshipStatus newStatus)
        {
            var friendship = await _friendships.GetByIdAsync(friendshipId);
            if (friendship == null) return false;

            // Only receiver can accept/decline
            if (friendship.ReceiverId != userId) return false;

            friendship.Status = newStatus;
            await _friendships.UpdateAsync(friendshipId, friendship);
            return true;
        }
    }
}
