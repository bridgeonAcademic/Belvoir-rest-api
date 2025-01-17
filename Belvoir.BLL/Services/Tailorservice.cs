using AutoMapper;
using Belvoir.Bll.DTO.Tailor;
using Belvoir.Bll.Mappings;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Admin;
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
                statuscode = 200,
                message = "Success",
                data = response
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
                        message = "Task status updated successfully.",
                        statuscode = 200,

                    };
                }
                else
                {
                    return new Response<object>
                    {
                        message = "No task found with the given ID.",
                        statuscode = 404
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    message = "error in updating",
                    error = $"Error updating task status: {ex.Message}",
                    statuscode = 500

                };
            }
        }



        public async Task<Response<Dashboard>> GetDashboardapi(Guid tailorid)
        {
            var dashboard = await _repo.dashboard(tailorid);
            
            return new Response<Dashboard>
            {
                statuscode = 200,
                message = "Success",
                data = dashboard
            };
        }


        public async Task<Response<TailorGetDTO>> GetTailorprofile(Guid Tailorid)
        {
            var tailor = await _repo.SingleTailor(Tailorid);
            var response = _mapper.Map<TailorGetDTO>(tailor);
            return new Response<TailorGetDTO> { statuscode = 200, message = "success", data = response };
        }


        public async Task<Response<object>> ResetPassword(Guid Tailorid, PasswordResetDTO data)
        {

            var response = await _repo.SingleUserwithId(Tailorid);
            if (response == null)
            {
                return new Response<object>
                {
                    error = "User not found",
                    statuscode = 404
                };
            }

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(data.OldPassword, response.PasswordHash);
            if (!isOldPasswordValid)
            {
                return new Response<object>
                {
                    error = "Old password does not match",
                    statuscode = 404
                };
            }

            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(data.NewPassword);

            bool updateResponse = await _repo.UpdatePassword(Tailorid, hashedNewPassword);
            if (!updateResponse)
            {
                return new Response<object>
                {
                    error = "Failed to update password",
                    statuscode = 500
                };
            }

            return new Response<object>
            {
                message = "Password successfully updated",
                statuscode = 200
            };
        }



    }
}
