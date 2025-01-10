using AutoMapper;
using Belvoir.DTO.Rental;
using Belvoir.Helpers;
using Belvoir.Models;
using CloudinaryDotNet;
using Dapper;
using Org.BouncyCastle.Bcpg;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Belvoir.Services.Rentals
{
    public interface IRentalService
    {
        public Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO data, Guid userid);

        public Task<Response<RentalViewDTO>> GetRentalById(Guid id);

        public Task<Response<IEnumerable<RentalViewDTO>>> SearchRental(string name);

        public Task<Response<IEnumerable<RentalViewDTO>>> PaginatedProduct(int pagenumber, int pagesize);

        public Task<Response<object>> DeleteRental(Guid rentalId, Guid userid);

        public Task<Response<object>> UpdateRental(Guid rentalId, IFormFile[] files, RentalSetDTO data, Guid userid);

        public Task<Response<IEnumerable<RentalViewDTO>>> GetRentalsByCategory(
        string gender,
        string garmentType,
        string fabricType);


    }

    public class RentalSevice : IRentalService
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        public RentalSevice(IDbConnection connection, IMapper mapper, ICloudinaryService cloudinary)
        {
            _connection = connection;
            _mapper = mapper;
            _cloudinaryService = cloudinary;
        }
        public async Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO data, Guid userid)
        {
            if (files.Length > 3)
            {
                return new Response<object>
                {
                    statuscode = 400,
                    error = "You can only upload a maximum of 3 images."
                };
            }

            var fabric = await _connection.QueryFirstOrDefaultAsync("select * from FabricCategory where id=@id ", new { id = data.fabrictype });
            if (fabric == null)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "category doesnot exist"
                };

            }

            var query = @"
            INSERT INTO RentalProduct (Id,Title, Description, OfferPrice, Price, FabricType, Gender, GarmentType,isDeleted,CreatedAt,CreatedBy)
            VALUES (@Id,@Title, @Description, @OfferPrice, @Price, @FabricType, @Gender, @GarmentType,@IsDeleted,@CreatedAt,@CreatedBy)";
            var mapped = _mapper.Map<RentalProduct>(data);
            mapped.CreatedAt = DateTime.UtcNow;
            mapped.CreatedBy = userid;
            mapped.IsDeleted = false;
            var rentalid = Guid.NewGuid();
            mapped.Id = rentalid;
            await _connection.ExecuteAsync(query, mapped);

            for (int i = 0; i < files.Length; i++)
            {

                var image = await _cloudinaryService.UploadImageAsync(files[i]);
                await _connection.ExecuteAsync(
                 "insert into RentalImage values (@id, @Imagepath, @productid, @isprimary)",
                 new
                 {
                     id = Guid.NewGuid(),
                     Imagepath = image,
                     productid = rentalid,
                     isprimary = (i == 0)
                 }
                 );


            }
            return new Response<object>
            {
                message = "Rental item addded successfully",
                statuscode = 200

            };

        }

        public async Task<Response<RentalViewDTO>> GetRentalById(Guid id)
        {
            var query = "SELECT * FROM RentalProduct WHERE Id = @Id and isdeleted=false";
            var rental = await _connection.QueryFirstOrDefaultAsync<RentalProduct>(query, new { Id = id });

            var imagesresponse = await _connection.QueryAsync<RentalImage>("select * from RentalImage where productid=@Id", new { Id = id });

            if (rental == null)
            {
                return new Response<RentalViewDTO>
                {
                    statuscode = 404,
                    error = "Rental item not found"
                };
            }
            var mapped = _mapper.Map<RentalViewDTO>(rental);
            mapped.images = imagesresponse.ToList();

            return new Response<RentalViewDTO>
            {
                message = "Rental item retrieved successfully",
                statuscode = 200,
                data = mapped
            };
        }

        public async Task<Response<IEnumerable<RentalViewDTO>>> SearchRental(string name)
        {
            var query = @"
        SELECT * FROM RentalProduct 
        JOIN RentalImage ON RentalProduct.id = RentalImage.productid
        WHERE (Title LIKE CONCAT('%', @name, '%') 
               OR Description LIKE CONCAT('%', @name, '%'))
              AND IsDeleted = false";


            var resultDict = new Dictionary<string, RentalViewDTO>();

            var results = await _connection.QueryAsync<RentalProduct, RentalImage, RentalViewDTO>(
                query,
                (rentalproduct, rentalimage) =>
                {
                    var mapped = _mapper.Map<RentalViewDTO>(rentalproduct);
                    if (!resultDict.ContainsKey(rentalproduct.Id.ToString()))
                    {
                        mapped.images = new List<RentalImage>();
                        resultDict[rentalproduct.Id.ToString()] = mapped;
                    }

                    resultDict[rentalproduct.Id.ToString()].images.Add(rentalimage);

                    return null;
                },
                new { name });

            var finalResults = resultDict.Values.ToList();

            if (!finalResults.Any())
            {
                return new Response<IEnumerable<RentalViewDTO>>
                {
                    statuscode = 404,
                    error = "No rental products found matching the search criteria."
                };
            }

            return new Response<IEnumerable<RentalViewDTO>>
            {
                message = "Rental products retrieved successfully.",
                statuscode = 200,
                data = finalResults
            };
        }

        public async Task<Response<IEnumerable<RentalViewDTO>>> PaginatedProduct(int pagenumber, int pagesize)
        {
            var resultDict = new Dictionary<string, RentalViewDTO>();
            var respose = await _connection.QueryAsync<RentalProduct, RentalImage, RentalSetDTO>("select * from RentalProduct join RentalImage on RentalProduct.id=RentalImage.productid where isdeleted=false  LIMIT @page_size OFFSET @offset_value ",
              (rentalproduct, rentalimage) =>
              {
                  var mapped = _mapper.Map<RentalViewDTO>(rentalproduct);
                  if (!resultDict.ContainsKey(rentalproduct.Id.ToString()))
                  {
                      mapped.images = new List<RentalImage>();
                      resultDict[rentalproduct.Id.ToString()] = mapped;
                  }

                  resultDict[rentalproduct.Id.ToString()].images.Add(rentalimage);

                  return null;
              }
            , new { page_size = pagesize, offset_value = (pagenumber - 1) * pagesize });
            var result = resultDict.Values.ToList();
            return new Response<IEnumerable<RentalViewDTO>>
            {
                statuscode = 200,
                data = result,
                message = "success"
            };
        }

        public async Task<Response<object>> DeleteRental(Guid rentalId, Guid userid)
        {
            var rentalProduct = await _connection.QueryFirstOrDefaultAsync<RentalProduct>(
                "SELECT * FROM RentalProduct WHERE Id = @Id", new { Id = rentalId });

            if (rentalProduct == null)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "Rental product not found"
                };
            }
            await _connection.ExecuteAsync(
                "Update  RentalProduct set  isdeleted=@status ,updatedat=@time,updatedby=@user  WHERE Id = @Id",
                new { Id = rentalId, status = true, time = DateTime.UtcNow, user = userid }
            );

            return new Response<object>
            {
                message = "Rental item deleted successfully",
                statuscode = 200
            };
        }


        public async Task<Response<object>> UpdateRental(Guid rentalId, IFormFile[] files, RentalSetDTO data, Guid userid)
        {
            // Check if rental product exists
            var rentalProduct = await _connection.QueryFirstOrDefaultAsync<RentalProduct>(
                "SELECT * FROM RentalProduct WHERE Id = @Id", new { Id = rentalId });

            if (rentalProduct == null)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "Rental product not found"
                };
            }

            // Check if the fabric category exists
            var fabric = await _connection.QueryFirstOrDefaultAsync(
                "SELECT * FROM FabricCategory WHERE id = @id",
                new { id = data.fabrictype });

            if (fabric == null)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "Fabric category does not exist"
                };
            }

            // Update rental product information
            var query = @"
            UPDATE RentalProduct 
            SET Title = @Title, Description = @Description, OfferPrice = @OfferPrice, 
                Price = @Price, FabricType = @FabricType, Gender = @Gender, GarmentType = @GarmentType,UpdatedBy=@UpdatedBy,UpdatedAt=@UpdatedAt
            WHERE Id = @Id";

            var mapped = _mapper.Map<RentalProduct>(data);
            mapped.Id = rentalId;
            mapped.UpdatedBy = userid;
            mapped.UpdatedAt = DateTime.UtcNow;
            await _connection.ExecuteAsync(query, mapped);

            // If new images are provided, handle them
            if (files != null && files.Length > 0)
            {
                // First, delete existing images for this rental product (optional, if you want to replace them)
                await _connection.ExecuteAsync(
                    "DELETE FROM RentalImage WHERE productid = @ProductId",
                    new { ProductId = rentalId });

                // Add new images
                for (int i = 0; i < files.Length; i++)
                {
                    var image = await _cloudinaryService.UploadImageAsync(files[i]);
                    await _connection.ExecuteAsync(
                        "INSERT INTO RentalImage (Id, Imagepath, productid, isprimary) VALUES (@Id, @Imagepath, @ProductId, @IsPrimary)",
                        new
                        {
                            Id = Guid.NewGuid(),
                            Imagepath = image,
                            ProductId = rentalId,
                            IsPrimary = (i == 0)
                        }
                    );
                }
            }

            return new Response<object>
            {
                message = "Rental item updated successfully",
                statuscode = 200
            };
        }


        public async Task<Response<IEnumerable<RentalViewDTO>>> GetRentalsByCategory(
        string gender,
        string garmentType,
        string fabricType)
        {

            var rentals = await _connection.ExecuteAsync(
                 "CALL SearchRentalsByCategory(@gender, @garmentType, @fabricType);",
                 new { gender, garmentType, fabricType });

            if (rentals == null)
            {
                return new Response<IEnumerable<RentalViewDTO>>
                {
                    statuscode = 404,
                    error = "No rentals found for the specified category"
                };
            }
            var rentalViewDTOs = _mapper.Map<IEnumerable<RentalViewDTO>>(rentals);

            return new Response<IEnumerable<RentalViewDTO>>
            {
                message = "Rental items retrieved successfully",
                statuscode = 200,
                data = rentalViewDTOs
            };

        }


    }


}
