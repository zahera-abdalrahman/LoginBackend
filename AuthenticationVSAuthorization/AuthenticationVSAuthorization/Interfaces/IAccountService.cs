using AuthenticationVSAuthorization.Data;
using AuthenticationVSAuthorization.DTO;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationVSAuthorization.Interfaces
{
    public interface IAccountService
    {
        Task<bool> SignUp(SignUpDto SignUpDto);

        Task<SignInResult> Login(LoginDto loginDto);

        Task<User> getloggedInUser(string username);
        Task<User> getUserInfo(string id);

        Task<List<UserRoleDto>> GetAllUsers();
        Task<bool> AssignRole(AssignRoleDto dto);
    }
}
