using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTO;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private UserManager<AppUser> _userManeger;
    private readonly SignInManager<AppUser> _signInManager;

    public UsersController(UserManager<AppUser> userManeger, SignInManager<AppUser> signInManager)
    {
        _userManeger = userManeger;
        _signInManager = signInManager;
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
            return Ok(new {token = "token"});
        }
        return Unauthorized();
    }
}