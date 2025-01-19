namespace api.Services
{
    public class TokenCleanUpService
    {
        private readonly DataContext _db;

        public TokenCleanUpService(DataContext db)
        {
            _db = db;
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _db.BannedTokens.Where(t => t.ExpiresAt <= now);

            if (expiredTokens.Any())
            {
                _db.BannedTokens.RemoveRange(expiredTokens);
                await _db.SaveChangesAsync();
            }
        }
    }

}
