using AutoMapper;
using Belvoir.DTO.Tailor;
using Belvoir.DTO.User;
using Belvoir.Models;
using CloudinaryDotNet.Actions;
using Dapper;
using System.Data;
using System.Data.Common;

namespace Belvoir.Services.Admin
{
    public interface IAdminServices
    {
        public Task<Response<object>> GetAllUsers(string role);
        public Task<Response<object>> GetUserById(Guid id);
        public Task<Response<object>> GetUserByName(string role ,string name);
        public Task<Response<object>> BlockOrUnblock(Guid id,string role);
        public Task<Response<TailorResponseDTO>> AddTailor(TailorDTO tailorDTO);
        public Task<Response<object>> DeleteTailor(Guid id);
    }
    public class AdminServices : IAdminServices
    {

        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public AdminServices(IDbConnection connection , IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<Response<object>> GetAllUsers(string role)
        {
            try
            {
                string query = "SELECT * FROM User WHERE Role = @Role ";
                var users = await _connection.QueryAsync<User>(query,new {Role = role});
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
        public async Task<Response<Object>> GetUserByName(string role, string name)
        {
            try
            {
                var users = await _connection.QueryAsync<User>("SELECT * FROM User WHERE Role = @Role AND Name LIKE @Name", new { Name = $"%{name}%", Role = role });
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

        public async Task<Response<object>> BlockOrUnblock(Guid id, string role)
        {
            var user = await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM User WHERE Role = @Role AND Id = @Id", new { Id = id , Role = role});
            if (user == null)
            {
                return new Response<object>
                {
                    statuscode = 400,
                    message = "User not found"
                };
            }

            bool isBlocked = !user.IsBlocked;
            await _connection.ExecuteAsync("UPDATE User SET IsBlocked = @IsBlocked WHERE Id = @Id", new { IsBlocked = isBlocked, Id = id });
            string message = isBlocked ? "User is blocked" : "User is unblocked";
            return new Response<object>
            {
                statuscode = 201,
                message = message,
            };
        }
        public async Task<Response<TailorResponseDTO>>  AddTailor(TailorDTO tailorDTO)
        {
            // Check if the user already exists
            var existingUserQuery = "SELECT COUNT(*) FROM User WHERE Email = @Email";
            var userExists = await _connection.ExecuteScalarAsync<int>(existingUserQuery, new { tailorDTO.Email }) > 0;

            if (userExists)
            {
                return new Response<TailorResponseDTO>
                {
                    statuscode = 400,
                    message = "User already exists",
                    error = "Email already registered",
                    data = null
                };
            }

            // Insert the user into the database
            var insertUserQuery = @"
                INSERT INTO User (Id, Name, Email, PasswordHash, Phone, Role, IsBlocked)
                VALUES (@Id, @Name, @Email, @PasswordHash, @Phone, 'Tailor', @IsBlocked)";

            var newUser = _mapper.Map<User>(tailorDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tailorDTO.Password);

            await _connection.ExecuteAsync(insertUserQuery, newUser);

            // Prepare the response
            var responseDTO = _mapper.Map<TailorResponseDTO>(newUser);

            return new Response<TailorResponseDTO>
            {
                statuscode = 201,
                message = "Tailor added successfully",
                data = responseDTO
            };
        }

        public async Task<Response<object>> DeleteTailor(Guid id)
        {
            var user = await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM User WHERE Role = @Role AND Id = @Id", new { Id = id, Role = "Tailor" });
            if (user == null)
            {
                return new Response<object>
                {
                    statuscode = 400,
                    message = "Tailor not found"
                };
            }

            await _connection.ExecuteAsync("DELETE FROM User  WHERE Id = @Id", new {  Id = id });
            return new Response<object>
            {
                statuscode = 201,
                message = "successfully deleted",
            };
        }
    }
}
