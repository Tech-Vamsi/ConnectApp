using API.DTO;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService tokenService;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> _signInManager,TokenService tokenService)
        {
            this._userManager = userManager;
            this._signInManager = _signInManager;
            this.tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDTO)
        {
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == loginDTO.Email);
              //  .FindByEmailAsync(loginDTO.Email);
            if(user==null)
            {
                return Unauthorized();
            }

            var result =await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (result.Succeeded)
            {
                return CreateUserDto(user); 
            }
            return Unauthorized();

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto regDTO)
        {
            
            if(await _userManager.Users.AnyAsync(x=>x.UserName==regDTO.UserName))
            {
                ModelState.AddModelError("username", "Username Taken");
                return ValidationProblem(ModelState);
            }
            if(await _userManager.Users.AnyAsync(x=>x.Email==regDTO.Email))
            {
                ModelState.AddModelError("email", "Email Taken");
                return ValidationProblem(ModelState);
            }
            

            var user = new AppUser
            {
                DisplayName = regDTO.DisplayName,
                Email = regDTO.Email,
                UserName = regDTO.UserName,
            };
            var result = await _userManager.CreateAsync(user, regDTO.Password);
            if(result.Succeeded)
            {
                return CreateUserDto(user); 
            }
            return BadRequest("Porblem in registering the user. Please try again");

            
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email)); 
            return CreateUserDto(user);
        }

        private UserDto CreateUserDto(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = user?.Photos?.FirstOrDefault(x=>x.IsMain)?.Url,
                Token = tokenService.CreateToken(user),
                UserName = user.UserName,
            };

        }
    }
}
