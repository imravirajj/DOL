using System.Security.Claims;
using DOL.Identity.Application.Commands.ChangePassword;
using DOL.Identity.Application.Commands.Login;
using DOL.Identity.Application.Commands.RefreshToken;
using DOL.Identity.Application.Commands.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOL.Identity.API.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var commandWithIp = command with { ClientIp = clientIp };

        var result = await Mediator.Send(commandWithIp);
        return HandleResult(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var commandWithIp = command with { ClientIp = clientIp };

        var result = await Mediator.Send(commandWithIp);
        return HandleResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] Application.Commands.ForgotPassword.ForgotPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] Application.Commands.ResetPassword.ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
