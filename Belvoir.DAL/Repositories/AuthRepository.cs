using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories
{
    public interface IAuthRepository
    {
        public Task<bool> userExists(string email);
        public Task<bool> RegisterUser(User user);
        public Task<User> SingleUserWithEmail(string email);

        public Task<User> GetRefreshToken(string token);

        public Task<int> InsertRefreshToken(string token, Guid userid, DateTime expires);

        public Task<int> UpdateRefreshTokenAsync(string token ,Guid userid, DateTime expires);

        public Task<int> DeleteRefreshtoken(Guid userid);


    }
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _dbConnection;
        public AuthRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> userExists(string email)
        {
            var existingUserQuery = "SELECT COUNT(*) FROM User WHERE Email = @Email";
            return await _dbConnection.ExecuteScalarAsync<int>(existingUserQuery, new { email }) > 0;
        }
        public async Task<bool> RegisterUser(User user)
        {
            var insertUserQuery = @"
                INSERT INTO User (Id, Name, Email, PasswordHash, Phone, Role, IsBlocked)
                VALUES (@Id, @Name, @Email, @PasswordHash, @Phone, @Role, @IsBlocked)";
            return await _dbConnection.ExecuteAsync(insertUserQuery, user) > 0;
        }
        public async Task<User> SingleUserWithEmail(string email)
        {
            var getUserQuery = "SELECT * FROM User WHERE Email = @Email";
            var user = await _dbConnection.QuerySingleOrDefaultAsync<User>(getUserQuery, new { email });
            return user;
        }

        public async Task<User> GetRefreshToken(string token)
        {
            var query = "SELECT * FROM RefreshTokens WHERE Token = @Token";
            var response = await _dbConnection.QuerySingleOrDefaultAsync<RefreshToken>(query, new { Token = token });
            if (response.Expires < DateTime.Now) {
                return null;
            }
            return await _dbConnection.QueryFirstOrDefaultAsync<User>("Select * from User where Id=@id", new { id = response.UserId });
        }

        public async Task<int> InsertRefreshToken(string token, Guid userid, DateTime expires)
        {

            var query = @"
                INSERT INTO RefreshTokens (Token, UserId, Expires, Created)
                VALUES (@Token, @UserId, @Expires, @Created)";

            var parameters = new
            {
                Token = token,
                UserId = userid,
                Expires = expires,
                Created = DateTime.Now
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return result;
        }


        public async Task<int> UpdateRefreshTokenAsync(string token, Guid userId, DateTime expires)
        {
           
                var query = "UPDATE RefreshTokens SET Token = @Token, Created = @Created ,Expires=@exp WHERE UserId = @UserId";
                return await _dbConnection.ExecuteAsync(query, new { UserId = userId, Token = token, Created = DateTime.Now,exp=expires });
            
        }

        public async Task<int> DeleteRefreshtoken(Guid userid)
        {
            var query = "delete from RefreshTokens where UserId=@id";
            return await _dbConnection.ExecuteAsync(query, new { id = userid });
        }

    }



}
