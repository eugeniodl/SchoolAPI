using SharedModels;

namespace School_API.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> ValidateUserAsync(string username, string password);
        Task RegisterUserAsync(User user, string password);
    }
}
