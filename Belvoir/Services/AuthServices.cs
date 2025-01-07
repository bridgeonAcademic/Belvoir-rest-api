using System.Data;
using System.Security.Cryptography;
using AutoMapper;
using BCrypt.Net;
using Belvoir.DTO.User;
using Belvoir.Helpers;
using Belvoir.Models;
using Dapper;
using Org.BouncyCastle.Crypto.Generators;

namespace Belvoir.Services
{
    public interface IAuthServices
    {
        Task<Response<RegisterResponseDTO>> RegisterUserAsync(RegisterDTO registerDTO);
        Task<Response<string>> LoginAsync(LoginDTO loginDTO);
    }
    public class AuthServices : IAuthServices
    {
        private readonly IDbConnection _dbConnection;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;

        public AuthServices(IDbConnection dbConnection, IJwtHelper jwtHelper, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _jwtHelper = jwtHelper;
            _mapper = mapper;
        }

        public async Task<Response<RegisterResponseDTO>> RegisterUserAsync(RegisterDTO registerDTO)
        {
            // Check if the user already exists
            var existingUserQuery = "SELECT COUNT(*) FROM User WHERE Email = @Email";
            var userExists = await _dbConnection.ExecuteScalarAsync<int>(existingUserQuery, new { registerDTO.Email }) > 0;

            if (userExists)
            {
                return new Response<RegisterResponseDTO>
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
                VALUES (@Id, @Name, @Email, @PasswordHash, @Phone, @Role, @IsBlocked)";

            var newUser = _mapper.Map<User>(registerDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            await _dbConnection.ExecuteAsync(insertUserQuery, newUser);

            // Prepare the response
            var responseDTO = _mapper.Map<RegisterResponseDTO>(newUser);

            return new Response<RegisterResponseDTO>
            {
                statuscode = 201,
                message = "User registered successfully",
                data = responseDTO
            };
        }

        public async Task<Response<string>> LoginAsync(LoginDTO loginDTO)
        {
            // Retrieve the user from the database
            var getUserQuery = "SELECT * FROM User WHERE Email = @Email";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<User>(getUserQuery, new { loginDTO.Email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return new Response<string>
                {
                    statuscode = 401,
                    message = "Invalid credentials",
                    error = "Email or password is incorrect",
                    data = null
                };
            }

            if (user.IsBlocked)
            {
                return new Response<string>
                {
                    statuscode = 403,
                    message = "User is blocked",
                    error = "Access denied",
                    data = null
                };
            }

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user);

            return new Response<string>
            {
                statuscode = 200,
                message = "Login successful",
                data = token
            };
        }
    }
}
