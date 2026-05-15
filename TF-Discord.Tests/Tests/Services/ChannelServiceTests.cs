using Backend.Src.Models;
using Backend.Src.Repository;
using Backend.Src.Services;
using Moq;
using Shared.DTOs.Requests;
using Shared.Enums;

namespace TF_Discord.Tests.Tests.Services;

public class ChannelServiceTests
{
    private static ChannelService Build(
        Mock<IRepository<Channel>> channels,
        Mock<IRepository<Server>> servers,
        Mock<IRepository<ServerMember>> members,
        Mock<IRepository<Message>>? messageRepo = null)
    {
        messageRepo ??= new Mock<IRepository<Message>>();
        var messageService = new MessageService(
            messageRepo.Object,
            Mock.Of<IRepository<User>>(),
            Mock.Of<IRepository<Channel>>(),
            Mock.Of<IRepository<ServerMember>>());

        return new ChannelService(
            channels.Object,
            Mock.Of<IRepository<User>>(),
            servers.Object,
            members.Object,
            messageService);
    }

    [Fact]
    public async Task CreateChannelAsync_ReturnsNull_WhenRequesterIsMember()
    {
        var members = new Mock<IRepository<ServerMember>>();
        members.Setup(m => m.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ServerMember, bool>>>()))
            .ReturnsAsync(new List<ServerMember>
            {
                new() { ServerId = "srv1", UserId = "user1", Role = MemberRole.Member }
            });

        var service = Build(new Mock<IRepository<Channel>>(), new Mock<IRepository<Server>>(), members);

        var result = await service.CreateChannelAsync("srv1", "user1", new CreateChannelRequest { Name = "general" });

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateChannelAsync_CreatesChannel_WhenRequesterIsAdmin()
    {
        var members = new Mock<IRepository<ServerMember>>();
        members.Setup(m => m.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ServerMember, bool>>>()))
            .ReturnsAsync(new List<ServerMember>
            {
                new() { ServerId = "srv1", UserId = "admin1", Role = MemberRole.Admin }
            });

        var servers = new Mock<IRepository<Server>>();
        servers.Setup(s => s.GetByIdAsync("srv1")).ReturnsAsync(new Server { Id = "srv1", Name = "Test" });

        var channels = new Mock<IRepository<Channel>>();
        Channel? inserted = null;
        channels.Setup(c => c.InsertAsync(It.IsAny<Channel>()))
            .Callback<Channel>(c => inserted = c)
            .Returns(Task.CompletedTask);

        var service = Build(channels, servers, members);

        var result = await service.CreateChannelAsync("srv1", "admin1", new CreateChannelRequest { Name = "general" });

        Assert.NotNull(result);
        Assert.Equal("general", result!.Name);
        Assert.Equal(ChannelType.Server, inserted!.Type);
    }

    [Fact]
    public async Task DeleteChannelAsync_DeletesMessagesAndChannel_WhenRequesterIsOwner()
    {
        var channel = new Channel
        {
            Id = "ch1",
            Name = "general",
            ServerId = "srv1",
            Type = ChannelType.Server
        };

        var channels = new Mock<IRepository<Channel>>();
        channels.Setup(c => c.GetByIdAsync("ch1")).ReturnsAsync(channel);

        var members = new Mock<IRepository<ServerMember>>();
        members.Setup(m => m.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ServerMember, bool>>>()))
            .ReturnsAsync(new List<ServerMember>
            {
                new() { ServerId = "srv1", UserId = "owner1", Role = MemberRole.Owner }
            });

        var messages = new Mock<IRepository<Message>>();
        messages.Setup(m => m.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Message, bool>>>()))
            .ReturnsAsync(new List<Message>
            {
                new() { Id = "m1", ChannelId = "ch1", SenderId = "u1", Content = "hi" }
            });

        var service = Build(channels, new Mock<IRepository<Server>>(), members, messages);

        var deleted = await service.DeleteChannelAsync("ch1", "owner1");

        Assert.True(deleted);
        messages.Verify(m => m.DeleteAsync("m1"), Times.Once);
        channels.Verify(c => c.DeleteAsync("ch1"), Times.Once);
    }
}
