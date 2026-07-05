using AuthenticationVSAuthorization.Data;
using AuthenticationVSAuthorization.DTO;
using AuthenticationVSAuthorization.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationVSAuthorization.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> identityRole;
        private readonly SignInManager<User> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountService(UserManager<User> _userManager, RoleManager<IdentityRole> _identityRole, SignInManager<User> _signInManager
            , IHttpContextAccessor _httpContextAccessor)
        {
            userManager = _userManager;
            identityRole = _identityRole;
            signInManager = _signInManager;
            httpContextAccessor = _httpContextAccessor;
        }

        public async Task<bool> SignUp(SignUpDto signUpDTO)
        {
            bool IsSuccess = true;


            var existingUser = await userManager.FindByNameAsync(signUpDTO.Email);
            if (existingUser != null)
            {
                IsSuccess = false;
                return IsSuccess;
            }


            User newUser = new User()
            {
                UserName = signUpDTO.Email,
                FirstName = signUpDTO.FirstName,
                LastName = signUpDTO.LastName,
                Email = signUpDTO.Email,
                Gender = signUpDTO.Gender,
            };

            var createUserResult = await userManager.CreateAsync(newUser, signUpDTO.Password);

            if (createUserResult.Succeeded)
            {

                var roleResult = await userManager.AddToRoleAsync(newUser, "User");

                if (!roleResult.Succeeded)
                {
                    await userManager.DeleteAsync(newUser);
                    IsSuccess = false;
                }

                else
                {
                    var updateResult = await userManager.UpdateAsync(newUser);
                    if (!updateResult.Succeeded)
                    {
                        IsSuccess = false;
                    }

                }

            }
            return IsSuccess;
        }

        public async Task<SignInResult> Login(LoginDto loginInDTO)
        {
            var result = await signInManager.PasswordSignInAsync(loginInDTO.Email, loginInDTO.Password, false, false);

            return result;
        }

        public async Task<User> getUserInfo(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User> getloggedInUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<List<UserRoleDto>> GetAllUsers()
        {
            var users = userManager.Users.ToList();
            var list = new List<UserRoleDto>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                list.Add(new UserRoleDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return list;
        }

        public async Task<bool> AssignRole(AssignRoleDto dto)
        {
            var user = await userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return false;
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, dto.Role);

            return true;
        }
    }
}
