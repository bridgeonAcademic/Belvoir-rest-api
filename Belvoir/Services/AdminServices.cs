
using Belvoir.Models;
using Dapper;
using System.Data;

namespace Belvoir.Services
{
    public class AdminServices
    {
        
        private readonly IDbConnection _connection;

        public AdminServices(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<User> GetData()
        {
            return _connection.Query<User>("SELECT * FROM User");
        }

    }
}
