using AuthenticationVSAuthorization.Data;
using AuthenticationVSAuthorization.DTO;
using AuthenticationVSAuthorization.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AuthenticationVSAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly UserManager<User> userManager;
        private readonly IWebHostEnvironment host;

        public AccountController(IAccountService _accountService, UserManager<User> _userManager, IWebHostEnvironment _host)
        {
            userManager = _userManager;
            host = _host;
            accountService = _accountService;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(SignUpDto signUpDTO)
        {
            try
            {
                var result = await accountService.SignUp(signUpDTO);

                if (result)
                {
                    return Ok(new { message = "User created successfully" });
                }
                else
                {
                    return BadRequest(new { message = "This email is already registered" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }




        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto loginInDTO)
        {
            try
            {
                var result = await accountService.Login(loginInDTO);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(loginInDTO.Email);
                    if (user == null)
                    {
                        return Unauthorized(new { StatusCode = "UserNotFound", message = "User not found" });
                    }

                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, loginInDTO.Email),
                        new Claim("uniquevalue",Guid.NewGuid().ToString()),
                    };

                    var userRoles = await userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim("role", role));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMySuperSecureKeyWith32Characters"));

                    var token = new JwtSecurityToken(
                        issuer: "http://localhost",
                        audience: "User",
                        expires: DateTime.Now.AddDays(15),
                        claims: claims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                    });
                }
                else
                {
                    return Unauthorized(new { StatusCode = "Login_Failed", message = "Invalid email or password." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }

        }



        [HttpGet]
        [Route("finduser")]
        public async Task<ActionResult<User>> getloggedInUser(string username)
        {
            try
            {
                var userProfile = await accountService.getloggedInUser(username);
                if (userProfile == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }
        [HttpGet]
        [Route("userInfo")]
        public async Task<ActionResult<User>> GetUserProfile(string id)
        {
            try
            {
                var userProfile = await accountService.getUserInfo(id);
                if (userProfile == null)
                {
                    return NotFound(new { message = "User profile not found" });
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("AllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await accountService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("AssignRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(AssignRoleDto dto)
        {
            try
            {
                var result = await accountService.AssignRole(dto);
                if (result)
                {
                    return Ok(new { message = "Role updated successfully" });
                }
                return BadRequest(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }
    }
}
