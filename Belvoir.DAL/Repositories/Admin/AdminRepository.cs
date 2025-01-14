using Belvoir.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Globalization;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IAdminRepository
    {
        public Task<IEnumerable<User>> GetUsers(string role, UserQuery userQuery);
        public Task<User> SingleUserwithId(Guid userid);
        public Task<bool> BlockAndUnblockUser(Guid id, bool isBlocked);
        public Task<bool> isUserExists(string email);
        public Task<bool> AddTailor(User user);
        public Task<bool > Deleteuser(Guid id);


    }
    public class AdminRepository : IAdminRepository
    {
        private readonly IDbConnection _dbConnection;
        public AdminRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<User>> GetUsers(string role, UserQuery userQuery)
        {
            var ProcedureName = "GetUsers";
            var parameters = new DynamicParameters();
            parameters.Add("p_SearchTerm", userQuery.SearchTerm );
            parameters.Add("p_Role", role );
            parameters.Add("p_IsBlocked", userQuery.IsBlocked );
            parameters.Add("p_MincreatedAt", userQuery.MinCreatedDate );
            parameters.Add("p_MaxcreatedAt", userQuery.MaxCreatedDate );
            parameters.Add("p_SortBy", userQuery.SortBy );
            parameters.Add("p_IsDescending", userQuery.IsDescending);
            parameters.Add("p_PageSize", userQuery.PageSize);
            parameters.Add("p_PageNo", userQuery.pageNo );
            return await _dbConnection.QueryAsync<User>(ProcedureName, parameters,commandType:CommandType.StoredProcedure);
        }
        public async Task<User> SingleUserwithId(Guid id)
        {
            var user = await _dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM User WHERE Id = @Id", new { Id = id });
            return  user;

        }
       
        public async Task<bool> BlockAndUnblockUser(Guid id,bool isBlocked)
        {
            return await _dbConnection.ExecuteAsync("UPDATE User SET IsBlocked = @IsBlocked WHERE Id = @Id", new { IsBlocked = isBlocked, Id = id })>0;

        }
        public async Task<bool> isUserExists(string email)
        {
            var existingUserQuery = "SELECT COUNT(*) FROM User WHERE Email = @Email";
            return await _dbConnection.ExecuteScalarAsync<int>(existingUserQuery, new { email }) > 0;

        }
        public async Task<bool> AddTailor(User newUser)
        {
            var insertUserQuery = @"
                INSERT INTO User (Id, Name, Email, PasswordHash, Phone, Role, IsBlocked)
                VALUES (@Id, @Name, @Email, @PasswordHash, @Phone, 'Tailor', @IsBlocked)";
            return await _dbConnection.ExecuteAsync(insertUserQuery, newUser)>0;
        }
        public async Task<bool> Deleteuser(Guid id)
        {
            return await _dbConnection.ExecuteAsync("DELETE FROM User  WHERE Id = @Id", new { Id = id }) > 0;

        }
    }
}
