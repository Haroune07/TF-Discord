using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.Src.Models;
using Backend.Src.Repository;
using Backend.Src.Services;
using Moq;
using Shared.DTOs.Requests;
using Shared.Enums;
using Xunit;

namespace TF_Discord.Tests.Tests.Services
{
    public class ServerServiceTests
    {
        private readonly Mock<IRepository<Server>> _serverRepoMock;
        private readonly Mock<IRepository<ServerMember>> _memberRepoMock;
        private readonly Mock<IRepository<User>> _userRepoMock;
        private readonly ServerService _service;

        public ServerServiceTests()
        {
            _serverRepoMock = new Mock<IRepository<Server>>();
            _memberRepoMock = new Mock<IRepository<ServerMember>>();
            _userRepoMock = new Mock<IRepository<User>>();

            _service = new ServerService(
                _serverRepoMock.Object,
                _memberRepoMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateServer_WithValidRequest_InsertsServerAndOwnerMember()
        {
            // Arrange
            var req = new CreateServerRequest
            {
                Name = "Mon Super Serveur",
                OwnerId = "owner-123",
                ServerImageUrl = "image.com"
            };

            _serverRepoMock
                .Setup(r => r.InsertAsync(It.IsAny<Server>()))
                .Returns(Task.CompletedTask);

            _memberRepoMock
                .Setup(r => r.InsertAsync(It.IsAny<ServerMember>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateServerAsync(req);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(req.Name, result.Name);
            Assert.Equal(req.OwnerId, result.OwnerId);

            _serverRepoMock.Verify(r => r.InsertAsync(It.Is<Server>(s =>
                s.Name == req.Name && s.OwnerId == req.OwnerId)), Times.Once);

            _memberRepoMock.Verify(r => r.InsertAsync(It.Is<ServerMember>(m =>
                m.UserId == req.OwnerId && m.Role == MemberRole.Owner)), Times.Once);
        }

        [Fact]
        public async Task JoinServer_WhenAlreadyMember_ThrowsException()
        {
            // Arrange
            var req = new JoinOrLeaveServerRequest { ServerId = "srv-1", UserId = "usr-1" };

            // Simuler que l'utilisateur est déjà dans la liste
            var existingMembers = new List<ServerMember> { new ServerMember { ServerId = "srv-1", UserId = "usr-1" } };
            _memberRepoMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<ServerMember, bool>>>()))
                .ReturnsAsync(existingMembers);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.JoinServerAsync(req));
            Assert.Equal("User already in server", exception.Message);

            _memberRepoMock.Verify(r => r.InsertAsync(It.IsAny<ServerMember>()), Times.Never);
        }

        [Fact]
        public async Task JoinServer_WhenNotMember_InsertsNewMember()
        {
            // Arrange
            var req = new JoinOrLeaveServerRequest { ServerId = "srv-1", UserId = "usr-2" };

            // Retourner une liste vide = pas encore membre
            _memberRepoMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<ServerMember, bool>>>()))
                .ReturnsAsync(new List<ServerMember>());

            _memberRepoMock
                .Setup(r => r.InsertAsync(It.IsAny<ServerMember>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.JoinServerAsync(req);

            // Assert
            _memberRepoMock.Verify(r => r.InsertAsync(It.Is<ServerMember>(m =>
                m.ServerId == req.ServerId && m.UserId == req.UserId && m.Role == MemberRole.Member)), Times.Once);
        }

        [Fact]
        public async Task KickMember_ByOwner_DeletesMember()
        {
            // Arrange
            var req = new KickMemberRequest { ServerId = "srv-1", RequesterId = "owner-id", TargetUserId = "target-id" };

            var requesterMember = new ServerMember { Id = "m-owner", ServerId = "srv-1", UserId = "owner-id", Role = MemberRole.Owner };
            var targetMember = new ServerMember { Id = "m-target", ServerId = "srv-1", UserId = "target-id", Role = MemberRole.Member };

            // Mock de GetMemberAsync pour le demandeur (Requester)
            _memberRepoMock
                .Setup(r => r.FindAsync(It.Is<Expression<Func<ServerMember, bool>>>(expr => expr.Compile()(requesterMember))))
                .ReturnsAsync(new List<ServerMember> { requesterMember });

            // Mock de GetMemberAsync pour la cible (Target)
            _memberRepoMock
                .Setup(r => r.FindAsync(It.Is<Expression<Func<ServerMember, bool>>>(expr => expr.Compile()(targetMember))))
                .ReturnsAsync(new List<ServerMember> { targetMember });

            _memberRepoMock
                .Setup(r => r.DeleteAsync("m-target"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.KickMemberAsync(req);

            // Assert
            Assert.True(result);
            _memberRepoMock.Verify(r => r.DeleteAsync("m-target"), Times.Once);
        }

        [Fact]
        public async Task KickMember_ByMember_ReturnsFalse()
        {
            // Arrange
            var req = new KickMemberRequest { ServerId = "srv-1", RequesterId = "simple-member-id", TargetUserId = "target-id" };

            var requesterMember = new ServerMember { Id = "m-simple", ServerId = "srv-1", UserId = "simple-member-id", Role = MemberRole.Member };
            var targetMember = new ServerMember { Id = "m-target", ServerId = "srv-1", UserId = "target-id", Role = MemberRole.Member };

            _memberRepoMock
                .Setup(r => r.FindAsync(It.Is<Expression<Func<ServerMember, bool>>>(expr => expr.Compile()(requesterMember))))
                .ReturnsAsync(new List<ServerMember> { requesterMember });

            _memberRepoMock
                .Setup(r => r.FindAsync(It.Is<Expression<Func<ServerMember, bool>>>(expr => expr.Compile()(targetMember))))
                .ReturnsAsync(new List<ServerMember> { targetMember });

            // Act
            var result = await _service.KickMemberAsync(req);

            // Assert
            Assert.False(result);
            _memberRepoMock.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
