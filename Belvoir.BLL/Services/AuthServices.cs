using System.Data;
using System.Security.Cryptography;
using AutoMapper;
using BCrypt.Net;
using Belvoir.Bll.DTO.User;
using Belvoir.Bll.Helpers;
using Belvoir.Bll.Services.Notification;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories;
using Microsoft.AspNetCore.SignalR;

//using Org.BouncyCastle.Crypto.Generators;

namespace Belvoir.Bll.Services
{
    public interface IAuthServices
    {
        Task<Response<RegisterResponseDTO>> RegisterUserAsync(RegisterDTO registerDTO);
        Task<Response<Token>> LoginAsync(LoginDTO loginDTO);
        public Task<Response<Token>> RefreshTokenAsync(string refreshToken);

    }
    public class AuthServices : IAuthServices
    {
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _repo;
        private readonly INotificationServiceSignal _signal;
        public AuthServices(IAuthRepository repo, IJwtHelper jwtHelper, IMapper mapper,INotificationServiceSignal notification_service)
        {
            _jwtHelper = jwtHelper;
            _mapper = mapper;
            _repo = repo;   
            _signal = notification_service;
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
                _signal.TriggerNotification(newUser.Id.ToString(), "User registered in the Belvoir");

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

        public async Task<Response<Token>> LoginAsync(LoginDTO loginDTO)
        {
            // Retrieve the user from the database
            var user = await _repo.SingleUserWithEmail(loginDTO.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return new Response<Token>
                {
                    statuscode = 401,
                    message = "Invalid credentials",
                    error = "Email or password is incorrect",
                    
                };
            }

            if (user.IsBlocked)
            {
                return new Response<Token>
                {
                    statuscode = 403,
                    message = "User is blocked",
                    error = "Access denied",
                };
            }

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user);
            var refreshtoken = _jwtHelper.GenerateRefreshToken();
            await _repo.DeleteRefreshtoken(user.Id);



            var response = new Token
            {
                AccessToken = token,
                RefreshToken = refreshtoken,
            };

            await _repo.InsertRefreshToken(refreshtoken,user.Id,DateTime.Now.AddDays(7));
            return new Response<Token>
            {
                statuscode = 200,
                message = "Login successful",
                data = response
            };
        }


        public async Task<User> ValidateRefreshTokenAsync(string token)
        {
            var user = await _repo.GetRefreshToken(token);
            if (user == null)
            {
                return null; 
            }
            
            return user; 
        }


        public async Task<Response<Token>> RefreshTokenAsync(string refreshToken)
        {
            // Validate the refresh token
            var Data = await ValidateRefreshTokenAsync(refreshToken);
            if (Data == null)
            {
                return new Response<Token>
                {
                    message = "failed",
                    statuscode = 401,
                };
            }

            // Generate new tokens
            var token = _jwtHelper.GenerateToken(Data);
            var refreshtoken = _jwtHelper.GenerateRefreshToken();

            // Update the refresh token in the database
            await _repo.UpdateRefreshTokenAsync(refreshToken,Data.Id,DateTime.Now.AddDays(7));

            var responsetokens = new Token
            {
                AccessToken = token,
                RefreshToken = refreshtoken,
            };
            // Return the new tokens
            return new Response<Token>
            {
                message ="success",
                data=responsetokens,
                statuscode = 200,
            };
        }

    }
}
