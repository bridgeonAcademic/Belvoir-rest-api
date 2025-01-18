using System.Data;
using System.Security.Cryptography;
using AutoMapper;
using BCrypt.Net;
using Belvoir.Bll.DTO.User;
using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories;
//using Org.BouncyCastle.Crypto.Generators;

namespace Belvoir.Bll.Services
{
    public interface IAuthServices
    {
        Task<Response<RegisterResponseDTO>> RegisterUserAsync(RegisterDTO registerDTO);
        Task<Response<string>> LoginAsync(LoginDTO loginDTO);
    }
    public class AuthServices : IAuthServices
    {
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _repo;

        public AuthServices(IAuthRepository repo, IJwtHelper jwtHelper, IMapper mapper)
        {
            _jwtHelper = jwtHelper;
            _mapper = mapper;
            _repo = repo;   
        }

        public async Task<Response<RegisterResponseDTO>> RegisterUserAsync(RegisterDTO registerDTO)
        {
            // Check if the user already exists
            
            var userExists = await _repo.userExists(registerDTO.Email);
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
            

            var newUser = _mapper.Map<User>(registerDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            if (await _repo.RegisterUser(newUser))
            {

                // Prepare the response
                var responseDTO = _mapper.Map<RegisterResponseDTO>(newUser);

                return new Response<RegisterResponseDTO>
                {
                    statuscode = 201,
                    message = "User registered successfully",
                    data = responseDTO
                };
            }
            return new Response<RegisterResponseDTO>
            {
                statuscode = 500,
                message = "Some error occured",
                
            };
        }

        public async Task<Response<string>> LoginAsync(LoginDTO loginDTO)
        {
            // Retrieve the user from the database
            var user = await _repo.SingleUserWithEmail(loginDTO.Email);
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
