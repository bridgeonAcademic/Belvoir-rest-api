using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Belvoir.DAL.Repositories.Rental
{
    public interface IRentalRepository
    {
        public Task<int> CetegoryExist(Guid id);

        public Task<int> AddRentalProductAsync(RentalProduct rentalProduct,Guid Userid);

        public Task<IEnumerable<(RentalProduct, RentalImage)>> GetRentalProductsAsync(int pageSize, int pageNumber);

        public  Task<RentalProduct> GetRentalProductById(Guid rentalId);

        public Task<IEnumerable<RentalImage>> GetRentalImagesByProductId(Guid id);


        public Task<int> RentalProductAsDeleted(Guid rentalId, Guid userId);

        public Task<int> UpdateRentalProduct(RentalProduct rentalProduct);

        public Task<int> DeleteRentalImages(Guid rentalId);

        public Task<int> AddRentalImage(Guid rentalId, string imagePath, bool isPrimary);

        public  Task<IEnumerable<(RentalProduct, RentalImage)>> GetRentalsByCategoryAsync(string gender, string garmentType, string fabricType);

        public Task<IEnumerable<(RentalProduct, RentalImage)>> SearchRentalsByName(string name);

    }
    public class RentalRepository:IRentalRepository
    {
        public readonly IDbConnection _connection;
        

        public RentalRepository(IDbConnection connection) { 
        _connection = connection;
        }
        public async Task<int> CetegoryExist(Guid id)
        {
            var fabric = await _connection.ExecuteScalarAsync<int>("select count(*) from FabricCategory where id=@id ", new { id = id });
            return fabric;

        }

        public async Task<int> AddRentalProductAsync(RentalProduct rentalProduct,Guid userid)
        {
            var query = @"
            INSERT INTO RentalProduct (Id, Title, Description, OfferPrice, Price, FabricType, Gender, GarmentType, isDeleted, CreatedAt, CreatedBy)
            VALUES (@Id, @Title, @Description, @OfferPrice, @Price, @FabricType, @Gender, @GarmentType, @IsDeleted, @CreatedAt, @CreatedBy)";

         
            var response =await _connection.ExecuteAsync(query, rentalProduct);

            return response ;
        }




        public async Task<IEnumerable<(RentalProduct, RentalImage)>> GetRentalProductsAsync(int pageSize, int pageNumber)
        {
            var offset = (pageNumber - 1) * pageSize;

            var query = @"
            SELECT * 
            FROM RentalProduct 
            JOIN RentalImage 
            ON RentalProduct.id = RentalImage.productid 
            WHERE RentalProduct.IsDeleted = false 
            LIMIT @page_size OFFSET @offset_value";

            return await _connection.QueryAsync<RentalProduct, RentalImage, (RentalProduct, RentalImage)>(
                query,
                (rentalProduct, rentalImage) => (rentalProduct, rentalImage),
                new { page_size = pageSize, offset_value = offset }
            );
        }


        public async Task<RentalProduct> GetRentalProductById(Guid rentalId)
        {
            return await _connection.QueryFirstOrDefaultAsync<RentalProduct>(
                "SELECT * FROM RentalProduct WHERE Id = @Id",
                new { Id = rentalId }
            );
        }

        public async Task<IEnumerable<RentalImage>> GetRentalImagesByProductId(Guid id)
        {
            var query = "SELECT * FROM RentalImage WHERE productid = @Id";
            return await _connection.QueryAsync<RentalImage>(query, new { Id = id });
        }

        public async Task<int> RentalProductAsDeleted(Guid rentalId, Guid userId)
        {
            return await _connection.ExecuteAsync(
                "UPDATE RentalProduct SET isdeleted = @status, updatedat = @time, updatedby = @user WHERE Id = @Id",
                new { Id = rentalId, status = true, time = DateTime.UtcNow, user = userId }
            );
        }


        public async Task<int> UpdateRentalProduct(RentalProduct rentalProduct)
        {
            var query = @"
            UPDATE RentalProduct 
            SET Title = @Title, Description = @Description, OfferPrice = @OfferPrice, 
                Price = @Price, FabricType = @FabricType, Gender = @Gender, 
                GarmentType = @GarmentType, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

            return await _connection.ExecuteAsync(query, rentalProduct);
        }

        public async Task<int> DeleteRentalImages(Guid rentalId)
        {
            return await _connection.ExecuteAsync(
                "DELETE FROM RentalImage WHERE productid = @ProductId",
                new { ProductId = rentalId }
            );
        }

        public async Task<int> AddRentalImage(Guid rentalId, string imagePath, bool isPrimary)
        {
            return await _connection.ExecuteAsync(
                "INSERT INTO RentalImage (Id, Imagepath, productid, isprimary) VALUES (@Id, @Imagepath, @ProductId, @IsPrimary)",
                new
                {
                    Id = Guid.NewGuid(),
                    Imagepath = imagePath,
                    ProductId = rentalId,
                    IsPrimary = isPrimary
                }
            );
        }


        public async Task<IEnumerable<(RentalProduct, RentalImage)>> GetRentalsByCategoryAsync(string gender, string garmentType, string fabricType)
        {
            var query = @"
            CALL SearchRentalsByCategory(@gender, @garmentType, @fabricType);";

            return await _connection.QueryAsync<RentalProduct, RentalImage, (RentalProduct, RentalImage)>(
                query,
                (rentalProduct, rentalImage) => (rentalProduct, rentalImage),
                new { gender, garmentType, fabricType },
                splitOn: "id"
            );
        }

        public async Task<IEnumerable<(RentalProduct, RentalImage)>> SearchRentalsByName(string name)
        {
        var query = @"
        SELECT * FROM RentalProduct 
        JOIN RentalImage ON RentalProduct.id = RentalImage.productid
        WHERE (Title LIKE CONCAT('%', @name, '%') 
               OR Description LIKE CONCAT('%', @name, '%'))
              AND RentalProduct.IsDeleted = false";

            var result = await _connection.QueryAsync<RentalProduct, RentalImage, (RentalProduct, RentalImage)>(
                query,
                (rentalproduct, rentalimage) => (rentalproduct, rentalimage),
                new { name }
            );

            Console.WriteLine("the result is" ,result);
            return result.ToList();
        }



    }
}
