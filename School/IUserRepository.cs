using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace School
{
    public interface IUserRepository
    {
        //Task<bool> AuthenticateUserAsync(string username, string password);
        Task<string> AuthenticateUserAsync(string username, string password);
    }
}
