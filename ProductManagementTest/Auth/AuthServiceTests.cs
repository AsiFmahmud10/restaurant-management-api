using System.Linq.Expressions;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Moq;
using ProductManagement.Auth;
using ProductManagement.Auth.Dto;
using ProductManagement.Cart;
using ProductManagement.Exception;
using ProductManagement.Role;
using ProductManagement.Token;
using ProductManagement.User;

namespace ProductManagementTest.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly Mock<ICartService> _cartService;
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
    private readonly Mock<IWebHostEnvironment> _envMock;

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _roleServiceMock = new Mock<IRoleService>();
        _cartService = new Mock<ICartService>();
        _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
        _envMock = new Mock<IWebHostEnvironment>();

        _authService = new AuthService(
            _userServiceMock.Object,
            _tokenServiceMock.Object,
            _roleServiceMock.Object,
            _cartService.Object,
            _backgroundJobClientMock.Object,
            _envMock.Object
        );
    }

    [Fact]
    public void Register_Success()
    {
        var request = new UserRegisterReq()
        {
            Email = "mhamudasif2@gmail.com",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "Asif",
            LastName = "Mahmud"
        };

        _userServiceMock
            .Setup(service => service.FindByEmail("mhamudasif2@gmail.com"))
            .Returns((User?)null);

        _roleServiceMock
            .Setup(service => service.FindByName("customer"))
            .Returns(new Role() { Name = "customer" });

        _authService.Register(request);


        _userServiceMock.Verify(
            s => s.Save(It.Is<User>(u =>
                u.Email == request.Email &&
                u.FirstName == request.FirstName &&
                u.LastName == request.LastName &&
                u.Roles.Any(r => r.Name == "customer")
            )),
            Times.Once
        );
    }

    [Fact]
    public void Register_WhenEmailAlreadyExists_ThrowsConflictException()
    {
        var request = new UserRegisterReq
        {
            Email = "existing@email.com",
            Password = "1234",
            FirstName = "Asif",
            LastName = "Mahmud"
        };

        _userServiceMock
            .Setup(s => s.FindByEmail(request.Email))
            .Returns(new User { Email = request.Email }); // simulate existing user


        var exception = Assert.Throws<ConflictException>(() => _authService.Register(request));

        Assert.Equal("Email address already exists", exception.Message);

        _userServiceMock.Verify(s => s.Save(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var loginReq = new LoginReq { Email = "test@example.com", Password = "password123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Email = loginReq.Email, Password = hashedPassword };

        _userServiceMock.Setup(s => s.FindByEmail(loginReq.Email,It.IsAny<Expression<Func<User,object>>[]>())).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns("access123");
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("refresh123");

        // Act
        var result = _authService.Login(loginReq);

        // Assert
        Assert.Equal("access123", result.AccessToken);
        Assert.Equal("refresh123", result.RefreshToken);
        _userServiceMock.Verify(s => s.Update(It.Is<User>(u => u.Tokens.Any(t => t.Value == "refresh123"))),
            Times.Once);
    }

    [Fact]
    public void Login_WhenUserNotFound_ThrowsResourceNotFoundException()
    {
        // Arrange
        var loginReq = new LoginReq { Email = "missing@example.com", Password = "password123" };
        _userServiceMock.Setup(s => s.FindByEmail(loginReq.Email)).Returns((User?)null);

        // Act & Assert
        Assert.Throws<ResourceNotFoundException>(() => _authService.Login(loginReq));
    }

    [Fact]
    public void Login_WhenWrongPassword_ThrowsUnAuthorizedException()
    {
        // Arrange
        var loginReq = new LoginReq { Email = "test@example.com", Password = "wrongpass" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Email = loginReq.Email, Password = hashedPassword };

        _userServiceMock.Setup(s => s.FindByEmail(loginReq.Email,It.IsAny<Expression<Func<User,object>>[]>())).Returns(user);

        // Act & Assert
        var exception = Assert.Throws<UnAuthorizedException>(() => _authService.Login(loginReq));
        Assert.Equal("Passwords do not match", exception.Message);
    }

    [Fact]
    public void Login_WhenNullAccessToken_ThrowsApplicationException()
    {
        // Arrange
        var loginReq = new LoginReq { Email = "test@example.com", Password = "password123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Email = loginReq.Email, Password = hashedPassword };

        _userServiceMock.Setup(s => s.FindByEmail(loginReq.Email,It.IsAny<Expression<Func<User,object>>[]>())).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns((string?)null);
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("refresh123");

        // Act & Assert
        Assert.Throws<ApplicationException>(() => _authService.Login(loginReq));
    }

    [Fact]
    public void Login_WhenNullRefreshToken_ThrowsApplicationException()
    {
        // Arrange
        var loginReq = new LoginReq { Email = "test@example.com", Password = "password123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Email = loginReq.Email, Password = hashedPassword };

        _userServiceMock.Setup(s => s.FindByEmail(loginReq.Email,It.IsAny<Expression<Func<User,object>>[]>())).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns("access123");
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns((string?)null);

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _authService.Login(loginReq));
        Assert.Equal("Refresh token was not created", exception.Message);
    }

    [Fact]
    public void Login_RefreshTokenPersistedInUser()
    {
        // Arrange
        var loginReq = new LoginReq { Email = "test@example.com", Password = "password123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Email = loginReq.Email, Password = hashedPassword };

        _userServiceMock.Setup(s => s.FindByEmail(loginReq.Email,It.IsAny<Expression<Func<User,object>>[]>())).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns("access123");
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("refresh123");

        // Act
        var result = _authService.Login(loginReq);

        // Assert
        Assert.Single(user.Tokens);
        Assert.Equal("refresh123", user.Tokens.First().Value);
        Assert.Equal(TokenType.Refresh, user.Tokens.First().TokenType);
    }
    
    [Fact]
    public void RefreshToken_InvalidToken_ThrowsUnAuthorized()
    {
        // Arrange
        var req = new RefreshTokenReq { RefreshToken = "bad-token" };
        _tokenServiceMock.Setup(s => s.ValidateToken(req.RefreshToken))
            .Returns(false);

        // Act & Assert
        var exception = Assert.Throws<UnAuthorizedException>(() => _authService.RefreshToken(req));
        Assert.Equal("Invalid refresh token", exception.Message);
    }

    [Fact]
    public void RefreshToken_TokenNotFound_ThrowsException()
    {
        // Arrange
        var req = new RefreshTokenReq { RefreshToken = "valid-token" };
        _tokenServiceMock.Setup(s => s.ValidateToken(req.RefreshToken)).Returns(true);
        _tokenServiceMock.Setup(s => s.GetByValueWithUser(req.RefreshToken)).Returns((Token?)null);

        // Act & Assert
        var exception = Assert.Throws<UnAuthorizedException>(() => _authService.RefreshToken(req));
        Assert.Equal("Refresh token not found", exception.Message);
    }

    [Fact]
    public void RefreshToken_WhenUserNotFound_ThrowsException()
    {
        // Arrange
        var req = new RefreshTokenReq { RefreshToken = "valid-token" };
        var tokenEntity = new Token { Value = "valid-token" };

        _tokenServiceMock.Setup(s => s.ValidateToken(req.RefreshToken)).Returns(true);
        _tokenServiceMock.Setup(s => s.GetByValueWithUser(req.RefreshToken)).Returns(tokenEntity);
        _userServiceMock.Setup(s => s.FindById(Guid.NewGuid())).Returns((User?)null);

        // Act & Assert
        var exception = Assert.Throws<UnAuthorizedException>(() => _authService.RefreshToken(req));
        Assert.Equal("user not found", exception.Message);
    }

    [Fact]
    public void RefreshToken_NullAccessToken_ThrowsException()
    {
        // Arrange
        var req = new RefreshTokenReq { RefreshToken = "valid-token" };
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
        var tokenEntity = new Token { Value = "valid-token", User = user };

        _tokenServiceMock.Setup(s => s.ValidateToken(req.RefreshToken)).Returns(true);
        _tokenServiceMock.Setup(s => s.GetByValueWithUser(req.RefreshToken)).Returns(tokenEntity);
        _userServiceMock.Setup(s => s.FindById(It.IsAny<Guid>())).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns((string?)null);
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("Refresh Token");

        // Act & Assert
        var exception = Assert.Throws<UnAuthorizedException>(() => _authService.RefreshToken(req));
        Assert.Equal("Token was not created", exception.Message);
    }

    [Fact]
    public void RefreshToken_NullRefreshToken_ThrowsException()
    {
        // Arrange
        var req = new RefreshTokenReq { RefreshToken = "valid-token" };
        var user = new User { Email = "test@example.com" };
        var tokenEntity = new Token { Value = "valid-token", UserId = Guid.NewGuid(), User = user };

        _tokenServiceMock.Setup(s => s.ValidateToken(req.RefreshToken)).Returns(true);
        _tokenServiceMock.Setup(s => s.GetByValueWithUser(req.RefreshToken)).Returns(tokenEntity);
        _userServiceMock.Setup(s => s.FindById(It.IsAny<Guid>())).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns("new-access-token");
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns((string?)null);

        // Act & Assert
        var exception = Assert.Throws<UnAuthorizedException>(() => _authService.RefreshToken(req));
        Assert.Equal("Token was not created", exception.Message);
    }

    [Fact]
    public void RefreshToken_ShouldPersistNewRefreshToken()
    {
        // Arrange
        var req = new RefreshTokenReq { RefreshToken = "old-token" };
        var user = new User { Id = Guid.NewGuid(), Email = "persist@test.com" };
        var tokenEntity = new Token { Value = "old-token", UserId = user.Id, User = user };

        _tokenServiceMock.Setup(s => s.ValidateToken(req.RefreshToken)).Returns(true);
        _tokenServiceMock.Setup(s => s.GetByValueWithUser(req.RefreshToken)).Returns(tokenEntity);
        _userServiceMock.Setup(s => s.FindById(user.Id)).Returns(user);
        _tokenServiceMock.Setup(s => s.GenerateAccessToken(user)).Returns("new-access");
        _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).Returns("new-refresh");

        // Act
        var result = _authService.RefreshToken(req);

        // Assert
        Assert.Single(user.Tokens);
        Assert.Equal("new-refresh", user.Tokens.First().Value);
        Assert.Equal(TokenType.Refresh, user.Tokens.First().TokenType);
        _userServiceMock.Verify(userService => userService.Update(user), Times.Once);
    }
}