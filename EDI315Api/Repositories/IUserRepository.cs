using System.Threading.Tasks;
using EDI315Api.Models;

namespace EDI315Api.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
    }
}
