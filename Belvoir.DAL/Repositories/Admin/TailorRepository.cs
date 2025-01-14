using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface ITailorRepository
    {
        public Task<IEnumerable<TailorTask>> GetTailorTask(Guid tailorid);
        public Task<int> UpdateStatus(Guid taskId, string status);
        public Task<Dashboard> dashboard(Guid tailorId);
        public Task<User> SingleUserwithId(Guid Tailorid);
        public Task<bool> UpdatePassword(Guid tailorid, string password);
        public Task<TailorViewDTO> SingleTailor(Guid Tailorid);



    }
    public class TailorRepository:ITailorRepository
    {
        private readonly IDbConnection _dbConnection;
        public TailorRepository(IDbConnection dbConnection) {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<TailorTask>> GetTailorTask(Guid tailorid)
        {
            var response = await _dbConnection.QueryAsync<TailorTask>("SELECT * FROM TailorTask where assaigned=@id", new { id = tailorid });
            return response;
        }
        public async Task<int> UpdateStatus(Guid taskId, string status)
        {
            const string query = "UPDATE TailorTask SET Status = @Status,updatedat=@update WHERE Id = @Id";
            return await _dbConnection.ExecuteAsync(query, new { Id = taskId, Status = status, update = DateTime.UtcNow });
        }
        public async Task<Dashboard> dashboard(Guid tailorId)
        {
            var dashboard = await _dbConnection.QueryMultipleAsync("TailorDashboard", new { inputTailorID = tailorId }, commandType : CommandType.StoredProcedure);
            return new Dashboard { averageRating = dashboard.ReadSingleOrDefault<decimal>(), completedorders = dashboard.ReadSingleOrDefault<int>(),pendingorders = dashboard.ReadSingleOrDefault<int>(),revenue = 500 };
        }
        public async Task<User> SingleUserwithId(Guid Tailorid)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync("SELECT PasswordHash FROM User WHERE Id = @tailorid", new { tailorid = Tailorid });
            
        }
        public async Task<bool> UpdatePassword(Guid Tailorid, string hashedNewPassword)
        {
            return await _dbConnection.ExecuteAsync("UPDATE User SET PasswordHash = @newpassword WHERE Id = @tailorid", new { newpassword = hashedNewPassword, tailorid = Tailorid }) == 1;

        }
        public async Task<TailorViewDTO> SingleTailor(Guid Tailorid)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<TailorViewDTO>("select * from User join TailorProfile on User.id=TailorProfile.Tailorid where User.id=@id", new { id = Tailorid });
            
        }
    }
}
