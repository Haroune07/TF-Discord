using Backend.Src.Models;
using Backend.Src.Repository;
using Backend.Src.Services;
using Moq;
using Shared.DTOs.Requests;
using Shared.Enums;

namespace TF_Discord.Tests.Tests.Services
{
    public class MessageServiceTests
    {
        private readonly Mock<IRepository<Message>> _mockMessageRepo;
        private readonly Mock<IRepository<User>> _mockUserRepo;
        private readonly Mock<IRepository<Channel>> _mockChannelRepo;
        private readonly Mock<IRepository<ServerMember>> _mockMemberRepo;
        private readonly MessageService _service;

        public MessageServiceTests()
        {
            _mockMessageRepo = new Mock<IRepository<Message>>();
            _mockUserRepo = new Mock<IRepository<User>>();
            _mockChannelRepo = new Mock<IRepository<Channel>>();
            _mockMemberRepo = new Mock<IRepository<ServerMember>>();

            _service = new MessageService(
                _mockMessageRepo.Object,
                _mockUserRepo.Object,
                _mockChannelRepo.Object,
                _mockMemberRepo.Object
            );
        }

        private void SetupServerChannelAccess(string channelId, string serverId, string userId)
        {
            _mockChannelRepo
                .Setup(r => r.GetByIdAsync(channelId))
                .ReturnsAsync(new Channel
                {
                    Id = channelId,
                    ServerId = serverId,
                    Type = ChannelType.Server
                });

            _mockMemberRepo
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ServerMember, bool>>>()))
                .ReturnsAsync(new List<ServerMember>
                {
                    new() { ServerId = serverId, UserId = userId, Role = MemberRole.Member }
                });
        }

        [Fact]
        public async Task SendMessage_WithValidRequest_ReturnsMessageDTO()
        {
            // Arrange
            var user = new User
            {
                Id = "user1",
                Username = "salah"
            };

            _mockUserRepo
                .Setup(r => r.GetByIdAsync("user1"))
                .ReturnsAsync(user);

            SetupServerChannelAccess("channel1", "srv1", "user1");

            var request = new CreateMessageRequest
            {
                SenderId = "user1",
                ChannelId = "channel1",
                Content = "bonjour"
            };

            // Act
            var result = await _service.SendMessageAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("bonjour", result!.Content);
            Assert.Equal("user1", result.Sender.Id);
        }

        [Fact]
        public async Task SendMessage_WhenNotServerMember_ReturnsNull()
        {
            _mockChannelRepo
                .Setup(r => r.GetByIdAsync("channel1"))
                .ReturnsAsync(new Channel
                {
                    Id = "channel1",
                    ServerId = "srv1",
                    Type = ChannelType.Server
                });

            _mockMemberRepo
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ServerMember, bool>>>()))
                .ReturnsAsync(new List<ServerMember>());

            var result = await _service.SendMessageAsync(new CreateMessageRequest
            {
                SenderId = "user1",
                ChannelId = "channel1",
                Content = "bonjour"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task EditMessage_ByOwner_ReturnsUpdatedMessage()
        {
            // Arrange
            var message = new Message
            {
                Id = "msg1",
                SenderId = "user1",
                Content = "ancien"
            };

            var user = new User
            {
                Id = "user1",
                Username = "salah"
            };

            _mockMessageRepo
                .Setup(r => r.GetByIdAsync("msg1"))
                .ReturnsAsync(message);

            _mockUserRepo
                .Setup(r => r.GetByIdAsync("user1"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.EditMessageAsync(
                "msg1",
                "user1",
                "nouveau"
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal("nouveau", result.Content);
            Assert.True(result.IsEdited);
        }

        [Fact]
        public async Task EditMessage_ByNonOwner_ReturnsNull()
        {
            // Arrange
            var message = new Message
            {
                Id = "msg1",
                SenderId = "owner"
            };

            _mockMessageRepo
                .Setup(r => r.GetByIdAsync("msg1"))
                .ReturnsAsync(message);

            // Act
            var result = await _service.EditMessageAsync(
                "msg1",
                "notOwner",
                "hack"
            );

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteMessage_ByOwner_ReturnsTrue()
        {
            // Arrange
            var message = new Message
            {
                Id = "msg1",
                SenderId = "user1"
            };

            _mockMessageRepo
                .Setup(r => r.GetByIdAsync("msg1"))
                .ReturnsAsync(message);

            // Act
            var result = await _service.DeleteMessageAsync(
                "msg1",
                "user1"
            );

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteMessage_ByNonOwner_ReturnsFalse()
        {
            // Arrange
            var message = new Message
            {
                Id = "msg1",
                SenderId = "owner"
            };

            _mockMessageRepo
                .Setup(r => r.GetByIdAsync("msg1"))
                .ReturnsAsync(message);

            // Act
            var result = await _service.DeleteMessageAsync(
                "msg1",
                "notOwner"
            );

            // Assert
            Assert.False(result);
        }
    }
}