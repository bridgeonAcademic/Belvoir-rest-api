using AutoMapper;
using Belvoir.Bll.DTO.Delivery;
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
   

        public Task<Response<IEnumerable<SalesReport>>> GetSalesReport();

        public  Task<Response<AdminDashboard>> GetDasboard();
        public Task<Response<object>> AddTailor(TailorDTO tailorDTO);
        public Task<Response<object>> AddDelivery(DeliveryDTO deliveryDTO);
        public Task<Response<object>> AddLaundry(RegisterDTO registerDTO);

        public Task<Response<object>> DeleteTailor(Guid id,string role);
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
                var totalusers = await _repo.GetCounts(role);
                var data = new UserAndCount { data = users, count = totalusers };
                return new Response<UserAndCount> { Data = data, StatusCode = 200, Message = "success" };
            }

            catch (Exception ex)
            {
                return new Response<UserAndCount>
                {
                    Error = ex.Message,
                    StatusCode = 500
                };
            }
        }
        public async Task<Response<Object>> GetUserById(Guid id)
        {
            try
            {
                var user = await _repo.SingleUserwithId(id);
                return new Response<object> { Data = user, StatusCode = 200, Message = "success" };

            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    Error = ex.Message,
                    StatusCode = 500
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
                    StatusCode = 400,
                    Message = "User not found"
                };
            }

            bool isBlocked = !user.IsBlocked;
            bool isrowAffected = await _repo.BlockAndUnblockUser(id, isBlocked);
            if (isrowAffected)
            {
                string message = isBlocked ? "User is blocked" : "User is unblocked";
                return new Response<object>
                {
                    StatusCode = 201,
                    Message = message,
                };
            }
            return new Response<object>
            {
                StatusCode = 500,
                Message = "some error ",
            };
        }
        public async Task<Response<object>>  AddTailor(TailorDTO tailorDTO)
        {
            // Check if the user already exists
            var userExists = await _repo.isUserExists(tailorDTO.Email);

            if (userExists)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "User already exists",
                    Error = "Email already registered",
                    Data = null
                };
            }

            // Insert the user into the database
            

            var newUser = _mapper.Map<Tailor>(tailorDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tailorDTO.Password);
            newUser.Tailorid = newUser.Id;
            newUser.tId = Guid.NewGuid();
            
            bool isrowAffected = await _repo.AddTailor(newUser);


            return new Response<object>
            {
                StatusCode = 201,
                Message = "Tailor added successfully",
                
            };
        }


        public async Task<Response<object>> AddDelivery(DeliveryDTO deliveryDTO)
        {
            // Check if the user already exists
            var userExists = await _repo.isUserExists(deliveryDTO.Email);

            if (userExists)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "User already exists",
                    Error = "Email already registered"
                };
            }

            // Insert the user into the database


            var newUser = _mapper.Map<Delivery>(deliveryDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(deliveryDTO.PasswordHash);
            newUser.dId = Guid.NewGuid();

            bool isrowAffected = await _repo.AddDelivery(newUser);

            // Prepare the response


            return new Response<object>
            {
                StatusCode = 201,
                Message = "Delivery Boy added successfully",

            };
        }
        public async Task<Response<object>> AddLaundry(RegisterDTO registerDTO)
        {
            // Check if the user already exists

            var userExists = await _repo.isUserExists(registerDTO.Email);
            if (userExists)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "User already exists",
                    Error = "Email already registered",
                    Data = null
                };
            }

            // Insert the user into the database


            var newUser = _mapper.Map<User>(registerDTO);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            if (await _repo.AddLaundry(newUser))
            {
                return new Response<object>
                {
                    StatusCode = 201,
                    Message = "User registered successfully",
                    
                };
            }
            return new Response<object>
            {
                StatusCode = 500,
                Message = "Some error occured",

            };
        }
        public async Task<Response<object>> DeleteTailor(Guid id,string role)
        {
            var user = await _repo.SingleUserwithId(id);
            if (user == null)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Message = "Tailor not found"
                };
            }
            bool isrowAffected = await _repo.Deleteuser(id,role);
            return new Response<object>
            {
                StatusCode = 201,
                Message = "successfully deleted",
            };
        }

        public async Task<Response<IEnumerable<SalesReport>>> GetSalesReport()
        {
            var response = await _repo.GetSales();
            return new Response <IEnumerable<SalesReport>>
            {
                StatusCode = 200,
                Message="success",
                Data = response
            };

        } 

        public async Task<Response<AdminDashboard>> GetDasboard()
        {

            var response = await _repo.Dashboard();
                return new Response<AdminDashboard>
                {
                    StatusCode = 200,
                    Message = "success",    
                    Data=response
                };
        }

    }
}
