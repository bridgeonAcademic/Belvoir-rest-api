
using Belvoir.Models;
using Dapper;
using System.Data;

namespace Belvoir.Services
{
    public interface IAdminServices
    {
        public Task<Response<object>> GetAllUsers();
        public Task<User> GetUserById(Guid id);
        public Task<IEnumerable<User>> GetUserByName(string name);
    }
    public class AdminServices : IAdminServices
    {

        private readonly IDbConnection _connection;

        public AdminServices(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Response<object>> GetAllUsers()
        {
            try
            {
                var users = await _connection.QueryAsync<User>("SELECT * FROM User");
                return new Response<object> { data = users, statuscode = 200, message = "success" };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    error = ex.Message,
                    statuscode = 500
                };
            }
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM User WHERE Id = @Id", new { Id = id });

        }
        public async Task<IEnumerable<User>> GetUserByName(string name)
        {
            return await _connection.QueryAsync<User>("SELECT * FROM User WHERE Name LIKE @Name", new { Name = $"%{name}%" });

        }
    }
}
