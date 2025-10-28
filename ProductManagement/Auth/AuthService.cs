using Hangfire;
using ProductManagement.Auth.Dto;
using ProductManagement.Exception;
using ProductManagement.Services.Email;

namespace ProductManagement.Auth;

using User;
using Token;
using System;
using Role;

public class AuthService(
    IUserService userService,
    ITokenService tokenService,
    IRoleService roleService,
    IBackgroundJobClient backgroundJobClient,
    IWebHostEnvironment hostEnv) : IAuthService
{
    public void Register(UserRegisterReq request)
    {
        if (userService.FindByEmail(request.Email) != null)
        {
            throw new ConflictException("Email address already exists");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User user = new User()
        {
            Email = request.Email,
            Password = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        var role = roleService.FindByName("customer");
        if (role is null)
        {
            throw new ApplicationException("customer_permission can not be found");
        }
        user.Roles.Add(role);

        userService.Save(user);
    }

    public LoginResponse Login(LoginReq loginReq)
    {
        var user = userService.FindByEmail(loginReq.Email,user => user.Tokens,user => user.Roles);
       
        if (user == null)
        {
            throw new ResourceNotFoundException("Email address not found");
        }

        if (BCrypt.Net.BCrypt.Verify(loginReq.Password, user.Password) == false)
        {
            throw new UnAuthorizedException("Passwords do not match");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        if (accessToken == null || refreshToken == null)
        {
            throw new ApplicationException("Token was not created");
        }

        Token refreshTokenEntity = new Token()
        {
            Value = refreshToken,
            User = user,
            TokenType = TokenType.Refresh,
        };

        user.Tokens.Add(refreshTokenEntity);
        userService.Update(user);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public LoginResponse RefreshToken(RefreshTokenReq refreshTokenReq)
    {
            bool isValid = tokenService.ValidateToken(refreshTokenReq.RefreshToken);

            if (isValid is false)
            {
                throw new UnAuthorizedException("Invalid refresh token");
            }

            Token? token = tokenService.GetByValueWithUser(refreshTokenReq.RefreshToken);
            if (token is null)
            {
                throw new Exception("");
            }

            User? user = userService.FindById(token.UserId);

            if (user is null)
            {
                throw new Exception("");
            }

            string? accessToken = tokenService.GenerateAccessToken(user);
            string? refreshToken = tokenService.GenerateRefreshToken();

            if (accessToken == null || refreshToken == null)
            {
                throw new Exception("Token was not created");
            }

            Token refreshTokenEntity = new Token()
            {
                Value = refreshToken,
                User = user,
                TokenType = TokenType.Refresh,
            };

            tokenService.DeleteTokenByUserId(user.Id);

            user.Tokens.Add(refreshTokenEntity);
            userService.Update(user);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
    
    }

    public void ChangePassword(ChangePasswordReq changePasswordReq)
    {
        User? user = userService.FindByEmail(changePasswordReq.Email);
        if (user is null)
        {
            throw new ApplicationException("Provide valid Password");
        }
        
        var isPasswordMatched = BCrypt.Net.BCrypt.Verify(changePasswordReq.OldPassword, user.Password);

        if (!isPasswordMatched)
        {
            throw new UnAuthorizedException("Passwords do not match");
        }
        
        if (isPasswordMatched)
        {
            user.Password  = BCrypt.Net.BCrypt.HashPassword(changePasswordReq.NewPassword);
            
        }
        userService.Update(user);
    }

    public void ForgetPassword(ForgetPasswordReq forgetPasswordReq)
    {
        User? user = userService.FindByEmail(forgetPasswordReq.Email);

        if (user is null)
        {
            throw new ResourceNotFoundException("Invalid Email");
        }

        var resetPasswordToken = tokenService.GenerateResetPasswordToken();
        user.Tokens.Add(resetPasswordToken);
        resetPasswordToken.User = user;
        userService.Update(user);

        var emailBody = string.Format(GetEmailTemplate("reset-password.html"), resetPasswordToken.Value);
        
        //sendEmail 
        backgroundJobClient
            .Enqueue<IEmailService>(emailService =>
                emailService.SendEmail("mhamudasif1@gmail.com", "hello background job", emailBody));
    }

    private string GetEmailTemplate(string filename)
    {
        var emailPath = Path.Combine(hostEnv.WebRootPath + "/Email-template/" + filename);
        return File.ReadAllText(emailPath);
    }

    public void ResetPassword(string tokenValue, ResetPasswordReq resetPasswordReq)
    {
        Token? existedToken = tokenService.GetByValueWithUser(tokenValue);

        if (existedToken is null)
        {
            throw new UnAuthorizedException("Invalid Token");
        }

        if (DateTime.UtcNow > existedToken.ValidUntil)
        {
            throw new UnauthorizedAccessException("Token is expired");
        }

        var user = existedToken.User;
        user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordReq.Password);
        userService.Update(user);
        tokenService.DeleteTokenByTokenValue(existedToken.Value);
    }
}