using System.Threading.Tasks;
using EDI315Api.Models;

namespace EDI315Api.Services
{
    public interface IUserService
    {
        Task<string> Authenticate(User user);
        Task Register(User user);
        
    }
}
