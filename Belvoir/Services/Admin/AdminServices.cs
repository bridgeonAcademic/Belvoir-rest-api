using Belvoir.Models;
using Dapper;
using System.Data;

namespace Belvoir.Services.Admin
{
    public interface IAdminServices
    {
        public Task<Response<object>> GetAllUsers();
        public Task<Response<object>> GetUserById(Guid id);
        public Task<Response<object>> GetUserByName(string name);
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
                var users = await _connection.QueryAsync<User>("SELECT * FROM User WHERE Role = 'User' ");
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
        public async Task<Response<Object>> GetUserById(Guid id)
        {
            try
            {
                var user = await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM User WHERE Id = @Id", new { Id = id });
                return new Response<object> { data = user, statuscode = 200, message = "success" };

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
        public async Task<Response<Object>> GetUserByName(string name)
        {
            try
            {
                var users = await _connection.QueryAsync<User>("SELECT * FROM User WHERE Name LIKE @Name", new { Name = $"%{name}%" });
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
    }
}
