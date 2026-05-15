using Backend.Src.Models;
using Backend.Src.Repository;
using Backend.Src.Services;
using Infrastructure.Interfaces;
using Moq;
using Shared.Constants;
using Shared.DTOs.Requests;


namespace TF_Discord.Tests.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IRepository<User>> _mockUserRepo;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IRepository<User>>();
            _mockNotificationService = new Mock<INotificationService>();

            // Default — FindAsync retourne liste vide (username n'existe pas)
            _mockUserRepo
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User>());

            // InsertAsync ne fait rien
            _mockUserRepo
                .Setup(r => r.InsertAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Notification ne fait rien
            _mockNotificationService
                .Setup(n => n.SendLoginNotificationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _service = new UserService(_mockUserRepo.Object, _mockNotificationService.Object);
        }

        // ?? Register ????????????????????????????????????????????????????????

        [Fact]
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var req = new Shared.DTOs.Requests.RegisterRequest { Username = "haroune", Password = "secret123" };

            // Act
            var result = await _service.Register(req);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal(Messages.UserCreatedSuccess, result.Message);
        }

        [Fact]
        public async Task Register_WithExistingUsername_ReturnsFailure()
        {
            // Arrange — FindAsync retourne un user existant
            var existingUser = new User { Username = "haroune", PasswordHash = "xxx" };
            _mockUserRepo
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User> { existingUser });

            var req = new RegisterRequest { Username = "haroune", Password = "secret123" };

            // Act
            var result = await _service.Register(req);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(Messages.UserNameAlreadyExists, result.Message);
        }

        [Fact]
        public async Task Register_WithShortPassword_ReturnsFailure()
        {
            // Arrange — password de 3 chars
            var req = new RegisterRequest { Username = "haroune", Password = "abc" };

            // Act
            var result = await _service.Register(req);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(Messages.InvalidPasswordLength, result.Message);
        }

        // ?? Login ???????????????????????????????????????????????????????????

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsSuccess()
        {
            // Arrange — user existant avec hash correct
            var password = "secret123";
            var existingUser = new User
            {
                Id = "abc123",
                Username = "haroune",
                PasswordHash = Backend.Src.Services.CryptoService.Hash(password)
            };

            _mockUserRepo
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User> { existingUser });

            _mockUserRepo
                .Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var req = new LoginRequest { Username = "haroune", Password = password };

            // Act
            var result = await _service.Login(req);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal(Messages.LoginSuccess, result.Message);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsFailure()
        {
            // Arrange — user existant mais mauvais mot de passe
            var existingUser = new User
            {
                Id = "abc123",
                Username = "haroune",
                PasswordHash = Backend.Src.Services.CryptoService.Hash("correctPassword")
            };

            _mockUserRepo
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(new List<User> { existingUser });

            var req = new LoginRequest { Username = "haroune", Password = "wrongPassword" };

            // Act
            var result = await _service.Login(req);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(Messages.InvalidUsernameOrPassowrd, result.Message);
        }
    }
}