using Microsoft.EntityFrameworkCore;
using SecurityAudit.Data;

namespace SecurityAudit.Services;

public static class UserQueries
{
    public static async Task<User?> GetUserSecureAsync(
        string username, AppDbContext db)
    {
        return await db.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
    }

    public static async Task<User?> GetUserRawAsync(
        string username, AppDbContext db)
    {
        return await db.Users
            .FromSqlInterpolated(
                $"SELECT * FROM Users WHERE Username = {username}")
            .FirstOrDefaultAsync();
    }
}
