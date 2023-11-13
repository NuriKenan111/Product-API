using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductApi.DTO;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private UserManager<AppUser> _userManeger;
    private readonly SignInManager<AppUser> _signInManager;
    private IConfiguration _configuration;

    public UsersController(UserManager<AppUser> userManeger, SignInManager<AppUser> signInManager, IConfiguration configuration)
    {
        _userManeger = userManeger;
        _signInManager = signInManager;
        _configuration = configuration;
    }
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(UserDTO model){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);

        }
        var user = new AppUser()
        {
            FullName = model.FullName,
            UserName = model.UserName,
            Email = model.Email,
            DateAdded = DateTime.Now
        };
        var result = await _userManeger.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            return StatusCode(201);
        }
            return BadRequest(result.Errors);
        
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var user = await _userManeger.FindByEmailAsync(model.Email);
        if(user == null){
            return BadRequest("Invalid Credentials");
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password,true);

        if(result.Succeeded){
            return Ok(new {token = GenerateJwt(user)});
        }
        return Unauthorized();
    }

    private object GenerateJwt(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes (_configuration.GetSection("AppSettings:Secret").Value ?? "");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            }),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "nurikenan.com",
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}