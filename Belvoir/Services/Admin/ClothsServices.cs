using Belvoir.Helpers;
using Belvoir.Models;
using Dapper;
using System.Data;

namespace Belvoir.Services.Admin
{
    public interface IClothsServices
    {
        public Task<Response<object>> GetAllCloths();
        public Task<Response<object>> GetClothById(Guid id);
        public Task<Response<object>> GetClothsByName(string name);
        public Task<Response<Object>> UpdateCloths(Cloth cloth);
        public Task<Response<Object>> DeleteCloths(Guid id);
        public Task<Response<Object>> AddCloths(IFormFile file,Cloth cloth);

    }
    public class ClothsServices : IClothsServices
    {
        private readonly IDbConnection _connection;
        private readonly ICloudinaryService _cloudinary;

        public ClothsServices(IDbConnection connection, ICloudinaryService cloudinary)
        {
            _connection = connection;
            _cloudinary = cloudinary;
        }

        public async Task<Response<object>> GetAllCloths()
        {
            try
            {
                var users = await _connection.QueryAsync<Cloth>("SELECT * FROM Cloths");
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
        public async Task<Response<Object>> GetClothById(Guid id)
        {
            try
            {
                var user = await _connection.QueryFirstOrDefaultAsync<Cloth>("SELECT * FROM Cloths WHERE ClothId = @Id", new { Id = id });
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
        public async Task<Response<Object>> GetClothsByName(string name)
        {
            try
            {
                var users = await _connection.QueryAsync<Cloth>("SELECT * FROM Cloths WHERE Title LIKE @Name", new { Name = $"%{name}%" });
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

        public async Task<Response<Object>> AddCloths(IFormFile file , Cloth cloth)
        {
            try
            {
                Guid id = Guid.NewGuid();
                string imageurl = await _cloudinary.UploadImageAsync(file);
                await _connection.ExecuteAsync(
                "INSERT INTO Cloths (Description, Design, id, Material, Title, ImageUrl) VALUES (@Description, @Design, @Id, @Material, @Title, @ImageUrl)",
                new
                {
                    Description = cloth.Description,
                    Design = cloth.DesignPattern,
                    Id = id,
                    Material = cloth.Material,
                    Title = cloth.Title,
                    ImageUrl = imageurl
                }); 
                return new Response<object> {  statuscode = 201, message = "success" };
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
        public async Task<Response<Object>> UpdateCloths(Cloth cloth)
        {
            try
            {
                await _connection.ExecuteAsync(
                    "UPDATE Cloths SET Description = @Description, DesignPattern = @Design, Material = @Materia, Title = @Title WHERE ClothId = @ClothId",
                    new
                    {
                        Description = cloth.Description,
                        Design = cloth.DesignPattern,
                        Materia = cloth.Material,
                        Title = cloth.Title,
                        ClothId = cloth.ClothId
                    });
                return new Response<object> { statuscode = 200, message = "success" };
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
        public async Task<Response<Object>> DeleteCloths(Guid id)
        {
            try
            {
                await _connection.ExecuteAsync("DELETE FROM Cloths WHERE ClothId = @Id", new { Id = id });
                return new Response<object> { statuscode = 200, message = "success" };
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
    }
}
