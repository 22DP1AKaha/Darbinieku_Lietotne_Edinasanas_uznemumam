using EIIOS.Data;
using EIIOS.Models;

namespace EIIOS.Services
{
    public class UserService
    {
        private readonly EIIOSDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(EIIOSDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsCurrentUserAdmin()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
                return false;

            var user = await _context.Users.FindAsync(userId.Value);
            return user?.Role == UserRole.Administrator && user.IsActive;
        }

        public async Task<bool> IsCurrentUserEmployee()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
                return false;

            var user = await _context.Users.FindAsync(userId.Value);
            return user?.Role == UserRole.Employee && user.IsActive;
        }

        public async Task<UserModel?> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
                return null;

            return await _context.Users.FindAsync(userId.Value);
        }
    }
}