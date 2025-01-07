﻿using Belvoir.Models;
using Dapper;
using System.Data;

namespace Belvoir.Services.Admin
{
    public interface IAdminServices
    {
        public Task<Response<object>> GetAllUsers();
        public Task<Response<object>> GetUserById(Guid id);
        public Task<Response<object>> GetUserByName(string name);
        public Task<Response<object>> BlockOrUnblock(Guid id);
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

        public async Task<Response<object>> BlockOrUnblock(Guid id)
        {
            var user = await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM User WHERE Id = @Id", new { Id = id });
            if (user == null)
            {
                return new Response<object>
                {
                    statuscode = 400,
                    message = "User not found"
                };
            }

            bool isBlocked = !user.IsBlocked;
            await _connection.ExecuteAsync("UPDATE TABLE User SET IsBlocked = @IsBlocked WHERE Id = @Id", new { IsBlocked = isBlocked, Id = id });
            string message = isBlocked ? "User is blocked" : "User is unblocked";
            return new Response<object>
            {
                statuscode = 201,
                message = message,
            };
        }
    }
}
