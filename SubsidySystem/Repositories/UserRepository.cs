using Microsoft.EntityFrameworkCore;
using SubsidySystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SubsidySystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;

                    // Явно указываем, что сущность изменена
                    _context.Entry(user).Property(x => x.LastLogin).IsModified = true;

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем авторизацию
                Console.WriteLine($"Ошибка при обновлении LastLogin: {ex.Message}");
            }
        }

        public async Task<bool> ValidateUserAsync(string login, string password)
        {
            var user = await GetByLoginAsync(login);
            if (user == null) return false;

            return user.PasswordHash == password;
        }
    }
}