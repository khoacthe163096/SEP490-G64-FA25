namespace DAL.vn.fpt.edu.interfaces
{
    public interface IUserRepository
    {
        Task<(bool Success, string? UserId, string? RoleName)> VerifyCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
        Task<(bool Success, long UserId)> CreateUserAsync(string username, string hashedPassword, string? email, string phone, long? roleId, CancellationToken cancellationToken = default);
        Task<long?> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken = default);
    }
}


