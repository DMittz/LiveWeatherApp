using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Data;
using Server.Supabase;
using Server.Services;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SupabaseUserService _supabase;
    private readonly IUserContextAccessor _userContextAccessor;

    public AuthController(SupabaseUserService supabase, IUserContextAccessor userContextAccessor)
    {
        _supabase = supabase;
        _userContextAccessor = userContextAccessor;
    }

    public class AuthRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest req)
    {
        var ok = await _supabase.SignUpAsync(req.Email, req.Password);
        return ok ? Ok() : BadRequest("Registration failed");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest req)
    {
        var ok = await _supabase.SignInAsync(req.Email, req.Password);
        return ok ? Ok() : Unauthorized("Invalid credentials");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _supabase.SignOutAsync();
        return Ok();
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userContext = _userContextAccessor.GetUserContext();
        
        return Ok(new AuthMeResponse
        {
            IsAuthenticated = userContext.IsAuthenticated,
            UserId = userContext.IsAuthenticated ? userContext.UserId : null,
            Email = userContext.IsAuthenticated ? userContext.Email : null
        });
    }
}
