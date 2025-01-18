using AutoMapper;
using Belvoir.Bll.DTO.Tailor;
using Belvoir.Bll.DTO.User;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Admin;
using CloudinaryDotNet.Actions;
using Dapper;
using System.Data;
using System.Data.Common;

namespace Belvoir.Bll.Services.Admin
{
    public interface IAdminServices
    {
        public Task<Response<UserAndCount>> GetAllUsers(string role,UserQuery userQuery);
        public Task<Response<object>> GetUserById(Guid id);
        public Task<Response<object>> BlockOrUnblock(Guid id,string role);
        public Task<Response<TailorResponseDTO>> AddTailor(TailorDTO tailorDTO);
        public Task<Response<object>> DeleteTailor(Guid id);

        public Task<Response<IEnumerable<SalesReport>>> GetSalesReport();


    }
    public class AdminServices : IAdminServices
    {

        private readonly IAdminRepository _repo;
        private readonly IMapper _mapper;

        public AdminServices(IAdminRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<UserAndCount>> GetAllUsers(string role, UserQuery userQuery)
        {
            try
            {
                var users = await _repo.GetUsers(role,userQuery);
                var count = users.Count();
                return new Response<UserAndCount> { data = new UserAndCount { data =users, count = count }, statuscode = 200, message = "success" };
            }
            catch (Exception ex)
            {
                return new Response<UserAndCount>
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
                var user = await _repo.SingleUserwithId(id);
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
       
        public async Task<Response<object>> BlockOrUnblock(Guid id, string role)
        {
            var user = await _repo.SingleUserwithId(id);
            if (user == null)
            {
                return new Response<object>
                {
                    statuscode = 400,
                    message = "User not found"
                };
            }

            bool isBlocked = !user.IsBlocked;
            bool isrowAffected = await _repo.BlockAndUnblockUser(id, isBlocked);
            if (isrowAffected)
            {
                string message = isBlocked ? "User is blocked" : "User is unblocked";
                return new Response<object>
                {
                    statuscode = 201,
                    message = message,
                };
            }
            return new Response<object>
            {
                statuscode = 201,
                message = "some error ",
            };
        }
        public async Task<Response<TailorResponseDTO>>  AddTailor(TailorDTO tailorDTO)
        {
            // Check if the user already exists
            var userExists = await _repo.isUserExists(tailorDTO.Email);

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
            

            var newUser = _mapper.Map<User>(tailorDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tailorDTO.Password);

            bool isrowAffected = await _repo.AddTailor(newUser);

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
            var user = await _repo.SingleUserwithId(id);
            if (user == null)
            {
                return new Response<object>
                {
                    statuscode = 400,
                    message = "Tailor not found"
                };
            }
            bool isrowAffected = await _repo.Deleteuser(id);
            return new Response<object>
            {
                statuscode = 201,
                message = "successfully deleted",
            };
        }

        public async Task<Response<IEnumerable<SalesReport>>> GetSalesReport()
        {
            var response = await _repo.GetSales();
            return new Response <IEnumerable<SalesReport>>
            {
                statuscode = 200,
                message="success",
                data = response
            };

        } 
    }
}
