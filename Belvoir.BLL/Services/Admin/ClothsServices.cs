using Belvoir.Bll.DTO;
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
        public Task<Response<Object>> UpdateCloths(Guid id,IFormFile file, ClothDTO cloth);
        public Task<Response<Object>> DeleteCloths(Guid id);
        public Task<Response<Object>> AddCloths(IFormFile file,ClothDTO cloth);

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
                return new Response<object> { Data = clothes, StatusCode = 200, Message = "success" };
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
        public async Task<Response<Object>> GetClothById(Guid id)
        {
            try
            {
                var query = @"SELECT 
                Description,Colors.name as color,MaterialTypes.name as materialtype,DesignTypes.name as designtype,Cloths.Id,Title,ImageUrl,Price
                FROM Cloths left join Colors on
                Cloths.Color=Colors.id
                left join DesignTypes on Cloths.DesignType=DesignTypes.id
                left join MaterialTypes on Cloths.MaterialType=MaterialTypes.id
                WHERE Cloths.id=@Id";
                var user = await _connection.QueryFirstOrDefaultAsync<Cloth>(query, new { Id = id });
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
      
        public async Task<Response<Object>> AddCloths(IFormFile file , ClothDTO cloth)
        {
            try
            {
                Guid id = Guid.NewGuid();
                string imageurl = await _cloudinary.UploadImageAsync(file);

                await _connection.ExecuteAsync(
                "INSERT INTO Cloths (Description, Id, MaterialType, Title, ImageUrl,CreatedBy,Color,DesignType,Price) VALUES (@Description, UUID(),@MaterialType, @Title ,@ImageUrl,'e2c7d233 - 3fd0 - 4527 - a79a - bfb45a762f1b',@Color,@DesignType,@Price)",
                new
                {
                    Price=cloth.Price,
                    Description = cloth.Description,
                    MaterialType = cloth.MaterialType,
                    Title = cloth.Title,
                    ImageUrl = imageurl,
                    Color=cloth.Color,
                    DesignType = cloth.DesignType
                }); 
                return new Response<object> {  StatusCode = 201, Message = "success" };
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
        public async Task<Response<Object>> UpdateCloths(Guid Id, IFormFile file, ClothDTO cloth)
        {
            try
            {
                await _connection.ExecuteAsync(
                    "UPDATE Cloths SET Description = @Description, MaterialType=@MaterialType, Title = @Title,Color=@Color,DesignType=@DesignType WHERE Id = @ClothId",
                    new
                    {
                        Description = cloth.Description,
                        MaterialType = cloth.MaterialType,
                        Title = cloth.Title,
                        Color = cloth.Color,
                        DesignType = cloth.DesignType,
                        ClothId= Id
                    });
                return new Response<object> { StatusCode = 200, Message = "success" };
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
        public async Task<Response<Object>> DeleteCloths(Guid id)
        {
            try
            {
                await _connection.ExecuteAsync("DELETE FROM Cloths WHERE Id = @Id", new { Id = id });
                return new Response<object> { StatusCode = 200, Message = "success" };
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

        public async Task<Response<object>> AddWishlist(Guid userId, Guid productId)
        {
            var itemexist = await _repo.ExistItem(userId, productId);
            if (itemexist > 0)
            {
                return new Response<object>
                {
                    Message = "item already exist",
                    StatusCode = 409
                };
            }
            await _repo.AddWhishlist(userId, productId);
            return new Response<object>
            {
                Message = "item added success",
                StatusCode = 200
            };
        }
        public async Task<Response<IEnumerable<WhishList>>> GetWishlist(Guid userId)
        {
            var response = await _repo.GetWishlist(userId);
            return new Response<IEnumerable<WhishList>>
            {
                Data = response,
                StatusCode = 200,
                Message = "Wishlist retrieved successfully."
            };

        }
    }
}
