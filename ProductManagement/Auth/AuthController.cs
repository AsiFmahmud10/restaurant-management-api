using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Auth.Dto;
using ProductManagement.Common.Annotation;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Auth;

public class AuthController(IAuthService authService) : Controller
{
    [Transaction]
    [AllowAnonymous]
    [HttpPost("/register")]
    [SwaggerOperation("Registers a new user",description:"Registers a new user")]
    public IActionResult Register([FromBody] UserRegisterReq registerReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }
        authService.Register(registerReq);
        return Ok("User registered successfully");
    }
    
    [AllowAnonymous]
    [HttpPost("/login")]
    [SwaggerOperation(summary:"Login user",description:"Login user")]
    public IActionResult Login([FromBody] LoginReq loginReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }
        return Ok(authService.Login(loginReq));
    }
    
    [HttpPost("/refresh-token")]
    [SwaggerOperation(summary:"Refresh Token",description:"Get Refresh Token")]
    [Transaction]
    public IActionResult RefreshToken([FromBody] RefreshTokenReq refreshTokenReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }
        return Ok(authService.RefreshToken(refreshTokenReq));
    }
    
    [HttpPost("/change-password")]
    [SwaggerOperation(summary:"Change user password",description:"Change user password")]
    public IActionResult ChangePassword([FromBody] ChangePasswordReq changePasswordReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }

        authService.ChangePassword(changePasswordReq);
        return Ok("Password changed successfully");
    }
    
    [HttpPost("/forget-password")]
    [SwaggerOperation(summary:"Forget password",description:"Forget password")]
    public IActionResult ForgetPassword([FromBody] ForgetPasswordReq forgetPasswordReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }

        authService.ForgetPassword(forgetPasswordReq);
        return Ok("token send successfully");
    }
    
    [HttpPost("/reset-password")]
    [SwaggerOperation(summary:"Reset Password",description:"Reset Password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordReq resetPasswordReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }

        authService.ResetPassword(resetPasswordReq.Token,resetPasswordReq);
        return Ok("Password changed successfully");
    }
    
    [Authorize]
    [HttpGet("/hello")]
    public IActionResult Hello()
    {
        return Ok("Hello");
    }
}