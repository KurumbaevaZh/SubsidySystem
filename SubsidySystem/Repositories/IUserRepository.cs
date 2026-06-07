using SubsidySystem.Models;
using System.Threading.Tasks;

namespace SubsidySystem.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginAsync(string login);
        Task UpdateLastLoginAsync(int userId);
        Task<bool> ValidateUserAsync(string login, string password);
    }
}