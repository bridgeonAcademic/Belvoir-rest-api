using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories
{
    public interface INotificationRepository
    {
        public Task<IEnumerable<NotificationModel>> GetNotification(Guid id);

    }
    public class NotificationRepository:INotificationRepository
    {
        private readonly IDbConnection _dbConnection;
        public NotificationRepository(IDbConnection connection) {
        _dbConnection = connection;
        } 
        public async Task<IEnumerable<NotificationModel>> GetNotification(Guid id)
        {
            return await _dbConnection.QueryAsync<NotificationModel>("SELECT * FROM notifications WHERE user_id = @userid ORDER BY is_read ASC, created_at DESC",
            new { userid = id });
        }
    }
}
