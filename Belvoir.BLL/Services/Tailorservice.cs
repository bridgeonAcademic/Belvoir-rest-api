using AutoMapper;
using Belvoir.Bll.DTO.Tailor;
using Belvoir.Bll.Mappings;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Tailors;
using System.Data;

namespace Belvoir.Bll.Services
{

    public interface ITailorservice
    {
        public Task<Response<IEnumerable<TailorTask>>> GET_ALL_TASK(Guid user);
        public Task<Response<object>> UpdateStatus(Guid taskId, string status);

        public Task<Response<Dashboard>> GetDashboardapi(Guid tailorid);

        public Task<Response<TailorGetDTO>> GetTailorprofile(Guid Tailorid);

        public Task<Response<object>> ResetPassword(Guid Tailorid, PasswordResetDTO data);

    }

    public class Tailorservice : ITailorservice
    {
        public readonly ITailorRepository _repo;
        public readonly IMapper _mapper;
        public Tailorservice(ITailorRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        public async Task<Response<IEnumerable<TailorTask>>> GET_ALL_TASK(Guid tailorid)
        {
          
            var response = await _repo.GetTailorTask(tailorid);
            return new Response<IEnumerable<TailorTask>>
            {
                StatusCode = 200,
                Message = "Success",
                Data = response
            };
        }






        public async Task<Response<object>> UpdateStatus(Guid taskId, string status)
        {
            try
            {

                int rowsAffected = await _repo.UpdateStatus(taskId, status);

                if (rowsAffected > 0)
                {

                    return new Response<object>
                    {
                        Message = "Task status updated successfully.",
                        StatusCode = 200,

                    };
                }
                else
                {
                    return new Response<object>
                    {
                        Message = "No task found with the given ID.",
                        StatusCode = 404
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    Message = "Error in updating",
                    Error = $"Error updating task status: {ex.Message}",
                    StatusCode = 500

                };
            }
        }



        public async Task<Response<Dashboard>> GetDashboardapi(Guid tailorid)
        {
            var dashboard = await _repo.dashboard(tailorid);
            
            return new Response<Dashboard>
            {
                StatusCode = 200,
                Message = "Success",
                Data = dashboard
            };
        }


        public async Task<Response<TailorGetDTO>> GetTailorprofile(Guid Tailorid)
        {
            var tailor = await _repo.SingleTailor(Tailorid);
            var response = _mapper.Map<TailorGetDTO>(tailor);
            return new Response<TailorGetDTO> { StatusCode = 200, Message = "success", Data = response };
        }


        public async Task<Response<object>> ResetPassword(Guid Tailorid, PasswordResetDTO data)
        {

            var response = await _repo.SingleUserwithId(Tailorid);
            if (response == null)
            {
                return new Response<object>
                {
                    Error = "User not found",
                    StatusCode = 404
                };
            }

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(data.OldPassword, response.PasswordHash);
            if (!isOldPasswordValid)
            {
                return new Response<object>
                {
                    Error = "Old password does not match",
                    StatusCode = 404
                };
            }

            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(data.NewPassword);

            bool updateResponse = await _repo.UpdatePassword(Tailorid, hashedNewPassword);
            if (!updateResponse)
            {
                return new Response<object>
                {
                    Error = "Failed to update password",
                    StatusCode = 500
                };
            }

            return new Response<object>
            {
                Message = "Password successfully updated",
                StatusCode = 200
            };
        }



    }
}
