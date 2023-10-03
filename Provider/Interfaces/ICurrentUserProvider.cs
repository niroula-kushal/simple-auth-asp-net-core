using SimpleAuth.Entity;

namespace SimpleAuth.Provider.Interfaces;

public interface ICurrentUserProvider
{
    bool IsLoggedIn();
    Task<User?> GetCurrentUser();
    long? GetCurrentUserId();
}