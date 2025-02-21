using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Tailors
{
    public interface ITailorRepository
    {
        public Task<IEnumerable<TailorTask>> GetTailorTask(Guid tailorid);
        public Task<int> UpdateStatus(Guid taskId, string status);
        public Task<Dashboard> dashboard(Guid tailorId);
        public Task<string> UserPassword(Guid Tailorid);
        public Task<bool> UpdatePassword(Guid tailorid, string password);
        public Task<Tailor> SingleTailor(Guid Tailorid);



    }
    public class TailorRepository : ITailorRepository
    {
        private readonly IDbConnection _dbConnection;
        public TailorRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<TailorTask>> GetTailorTask(Guid tailorid)
        {
            var response = await _dbConnection.QueryAsync<TailorTask>("SELECT * FROM TailorTask WHERE assaigned= NULL");
            return response;
        }
        public async Task<IEnumerable<TailorTask>> GetTailorTaskForTailor(Guid tailorid)
        {
            var response = await _dbConnection.QueryAsync<TailorTask>("SELECT * FROM TailorTask WHERE assaigned=@id", new { id = tailorid });
            return response;
        }
        public async Task<int> UpdateStatus(Guid taskId, string status)
        {
            const string query = "UPDATE TailorTask SET Status = @Status,updatedat=@update WHERE Id = @Id";
            return await _dbConnection.ExecuteAsync(query, new { Id = taskId, Status = status, update = DateTime.UtcNow });
        }
        public async Task<Dashboard> dashboard(Guid tailorId)
        {
            var dashboard = await _dbConnection.QuerySingleOrDefaultAsync<Dashboard>("TailorDashboard", new { inputTailorID = tailorId }, commandType: CommandType.StoredProcedure);
            
            return dashboard;
        }
        public async Task<string> UserPassword(Guid Tailorid)
        {
            var sql = "SELECT PasswordHash FROM User WHERE Id = @tailorid";
            return await _dbConnection.QuerySingleOrDefaultAsync<string>(sql, new { tailorid = Tailorid });
        }


        public async Task<bool> UpdatePassword(Guid Tailorid, string hashedNewPassword)
        {
            return await _dbConnection.ExecuteAsync("UPDATE User SET PasswordHash = @newpassword WHERE Id = @tailorid", new { newpassword = hashedNewPassword, tailorid = Tailorid }) == 1;

        }
        public async Task<Tailor> SingleTailor(Guid Tailorid)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<Tailor>("select * from User left join TailorProfile on User.id=TailorProfile.Tailorid where User.id=@id", new { id = Tailorid });

        }
    }
}
