using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using School_API.Data;
using School_API.Repository.IRepository;
using SharedModels;

namespace School_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SchoolContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserRepository(SchoolContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return 
                await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task RegisterUserAsync(User user, string password)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateUserAsync(string userName, string password)
        {
            var user = await GetUserByUserNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash,
                password);
            return verificationResult == PasswordVerificationResult.Success;
        }
    }
}
