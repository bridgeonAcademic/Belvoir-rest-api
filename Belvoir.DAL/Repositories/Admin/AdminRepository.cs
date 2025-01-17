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
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IAdminRepository
    {
        public Task<IEnumerable<User>> GetUsers(string role, UserQuery userQuery);
        public Task<User> SingleUserwithId(Guid userid);
        public Task<bool> BlockAndUnblockUser(Guid id, bool isBlocked);
        public Task<bool> isUserExists(string email);
        public Task<bool> AddTailor(Tailor tailor);
        public Task<bool> AddDelivery(Delivery delivery);
        public Task<bool > Deleteuser(Guid id);
        public Task<CountUser> GetCounts(string role);
        

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
        public async Task<bool> AddTailor(Tailor tailor)
        {
            var spname = "InsertUserAndTailorProfile";
            var parameters = new
            {
                p_Id = tailor.Id,
                p_Name = tailor.Name,
                p_Email = tailor.Email,
                p_PasswordHash = tailor.PasswordHash,
                p_Phone = tailor.Phone,
                p_IsBlocked = tailor.IsBlocked,
                p_dId = tailor.tId,
                p_Experience = tailor.Experience,
                
            };
            return await _dbConnection.ExecuteAsync(spname, parameters,commandType:CommandType.StoredProcedure)>0;
        }

        public async Task<bool> AddDelivery(Delivery delivery)
        {
            var spname = "InsertUserAndDeliveryDetails";
            var parameters = new
            {
                p_Id = delivery.Id,
                p_Name = delivery.Name,
                p_Email = delivery.Email,
                p_PasswordHash = delivery.PasswordHash ,
                p_Phone = delivery.Phone,
                p_IsBlocked = delivery.IsBlocked,
                p_dId = delivery.dId,
                p_DeliveryId = delivery.Id,
                p_LicenceNo = delivery.LicenceNo,
                p_VehicleNo = delivery.VehicleNo
            };
            return await _dbConnection.ExecuteAsync(spname, parameters,commandType:CommandType.StoredProcedure) > 0;
        }
        public async Task<bool> Deleteuser(Guid id)
        {
            return await _dbConnection.ExecuteAsync("DELETE FROM User  WHERE Id = @Id", new { Id = id }) > 0;

        }
        public async Task<CountUser> GetCounts(string role)
        {
            var parameters = new DynamicParameters();
            parameters.Add("roleName", role, DbType.String, ParameterDirection.Input);
            parameters.Add("totalusers", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("blockedusers", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("unblockedusers", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync("GetUserCounts", parameters, commandType: CommandType.StoredProcedure);

            var values = new CountUser()
            {
                activeusercount = parameters.Get<int>("unblockedusers"),
                usercount = parameters.Get<int>("totalusers"),
                blockedusercount = parameters.Get<int>("blockedusers")
            };
            return values;
        }
    }
}
