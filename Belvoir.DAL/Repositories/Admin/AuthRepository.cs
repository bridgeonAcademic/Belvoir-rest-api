using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IAuthRepository
    {
        public Task<bool> userExists(string  email);
        public Task<bool> RegisterUser(User user);
        public Task<User> SingleUserWithEmail(string email);

    }
    public class AuthRepository:IAuthRepository
    {
        private readonly IDbConnection _dbConnection;
        public AuthRepository(IDbConnection dbConnection) {
            _dbConnection = dbConnection;
        }

        public async Task<bool> userExists(string email) {
            var existingUserQuery = "SELECT COUNT(*) FROM User WHERE Email = @Email";
            return await _dbConnection.ExecuteScalarAsync<int>(existingUserQuery, new { email }) > 0;
        }
        public async Task<bool> RegisterUser(User user)
        {
            var insertUserQuery = @"
                INSERT INTO User (Id, Name, Email, PasswordHash, Phone, Role, IsBlocked)
                VALUES (@Id, @Name, @Email, @PasswordHash, @Phone, @Role, @IsBlocked)";
            return await _dbConnection.ExecuteAsync(insertUserQuery, user)>0;
        }
        public async Task<User> SingleUserWithEmail(string email)
        {
            var getUserQuery = "SELECT * FROM User WHERE Email = @Email";
            var user = await _dbConnection.QuerySingleOrDefaultAsync<User>(getUserQuery, new { email });
            return user;
        }
    }
}
