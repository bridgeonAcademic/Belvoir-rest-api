using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Admin;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace Belvoir.Bll.Services.Admin
{
    public interface IClothsServices
    {
        public Task<Response<object>> GetAllCloths(ProductQuery pquery);
        public Task<Response<object>> GetClothById(Guid id);
        public Task<Response<Object>> UpdateCloths(Cloth cloth);
        public Task<Response<Object>> DeleteCloths(Guid id);
        public Task<Response<Object>> AddCloths(IFormFile file,Cloth cloth);

        public Task<Response<object>> AddWishlist(Guid userId, Guid productId);

        public Task<Response<IEnumerable<WhishList>>> GetWishlist(Guid userId);


    }
    public class ClothsServices : IClothsServices
    {
        private readonly IDbConnection _connection;
        private readonly ICloudinaryService _cloudinary;
        private readonly IClothesRepository _repo;

        public ClothsServices(IDbConnection connection, ICloudinaryService cloudinary,IClothesRepository clothesRepository)
        {
            _connection = connection;
            _cloudinary = cloudinary;
            _repo = clothesRepository;
        }

        public async Task<Response<object>> GetAllCloths(ProductQuery pquery)
        {
            try
            {
                var clothes = await _repo.GetClothes(pquery);
                return new Response<object> { data = clothes, statuscode = 200, message = "success" };
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
      
        public async Task<Response<Object>> AddCloths(IFormFile file , Cloth cloth)
        {
            try
            {
                Guid id = Guid.NewGuid();
                string imageurl = await _cloudinary.UploadImageAsync(file);
                await _connection.ExecuteAsync(
                "INSERT INTO Cloths (Description, DesignPattern, Clothid, Material, Title, ImageUrl) VALUES (@Description, @Design, @Id, @Material, @Title, @ImageUrl)",
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

        public async Task<Response<object>> AddWishlist(Guid userId, Guid productId)
        {
            var itemexist = await _repo.ExistItem(userId, productId);
            if (itemexist > 0)
            {
                return new Response<object>
                {
                    message = "item already exist",
                    statuscode = 409
                };
            }
            await _repo.AddWhishlist(userId, productId);
            return new Response<object>
            {
                message = "item added success",
                statuscode = 200
            };
        }
        public async Task<Response<IEnumerable<WhishList>>> GetWishlist(Guid userId)
        {
            var response = await _repo.GetWishlist(userId);
            return new Response<IEnumerable<WhishList>>
            {
                data = response,
                statuscode = 200,
                message = "Wishlist retrieved successfully."
            };

        }
    }
}
