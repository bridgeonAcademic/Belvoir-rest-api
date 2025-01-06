using Belvoir.Models;
using Belvoir.Models.Generic_response;
using Dapper;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.Data;

namespace Belvoir.Services
{

    public interface ITailorservice {
        public  Task<Response<IEnumerable<TailorTask>>> GET_ALL_TASK(Guid user);
        public  Task<Response<object>> UpdateStatus(Guid taskId, string status);

        public Task<Response<Dashboard>> GetDashboardapi(Guid tailorid);

        public Task<Response<TailorProfile>> GetTailorprofile(Guid Tailorid);

    }

    public class Tailorservice:ITailorservice
    {
        public readonly IDbConnection connection;
        public Tailorservice(IDbConnection connection) { 
         this.connection = connection;
        }


        public async Task<Response<IEnumerable<TailorTask>>> GET_ALL_TASK(Guid tailorid)
        {
            var response = await connection.QueryAsync<TailorTask>("SELECT * FROM TailorTask where assaigned=@id",new {id=tailorid});

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
                const string query = "UPDATE TailorTask SET Status = @Status,updatedate=@update WHERE Id = @Id";

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = taskId, Status = status ,update=DateTime.UtcNow});

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
                        statuscode=404
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    message = "error in updating",
                    error = $"Error updating task status: {ex.Message}",
                    statuscode=500
                    
                };
            }
        }



        public async Task<Response<Dashboard>> GetDashboardapi(Guid tailorid)
        {
            var rating = await connection.QuerySingleAsync<int>("select avg(rating) from Rating group by tailorid having Tailorid=@Tailorid ",new { Tailorid = tailorid });
            var pending = await connection.QuerySingleAsync<int>("SELECT count(*) FROM TailorTask WHERE Status = @Status and assigned=@Tailorid", new { Status = "pending", Tailorid = tailorid });
            var completed = await connection.QuerySingleAsync<int>("SELECT count(*) FROM TailorTask WHERE Status = @Status and assigned=@Tailorid", new { Status = "completed", Tailorid = tailorid });
            var revenue = 500;
            return new Response<Dashboard>
            {
                statuscode = 200,
                message = "Success",
                data = new Dashboard
                {
                    rating = rating , 
                    pendingorders = pending,
                    completedorders = completed,
                    revenue = revenue
                }
            };
        }


        public async Task<Response<TailorProfile>> GetTailorprofile(Guid Tailorid)
        {
            var response = await connection.QuerySingleOrDefaultAsync<TailorProfile>("select * from TailorProfile where tailorid=@id",new {id=Tailorid});
            return new Response<TailorProfile> { statuscode = 200, message = "success", data = response };
        }


    }
}
 