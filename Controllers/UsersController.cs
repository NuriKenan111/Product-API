using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTO;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManeger;

    public UsersController(UserManager<AppUser> userManeger)
    {
        _userManeger = userManeger;
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
}