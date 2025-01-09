using AutoMapper;
using Belvoir.DTO.Rental;
using Belvoir.Helpers;
using Belvoir.Models;
using CloudinaryDotNet;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Belvoir.Services.Rentals
{
    public interface IRentalService
    {
        public Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO data);

        public Task<Response<RentalViewDTO>> GetRentalById(Guid id);

        public Task<Response<IEnumerable<RentalViewDTO>>> SearchRental(string name);

        public Task<Response<IEnumerable<RentalViewDTO>>> PaginatedProduct(int pagenumber, int pagesize);

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
        public async Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO data)
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
            INSERT INTO RentalProduct (Id,Title, Description, OfferPrice, Price, FabricType, Gender, GarmentType)
            VALUES (@Id,@Title, @Description, @OfferPrice, @Price, @FabricType, @Gender, @GarmentType)";
            var mapped = _mapper.Map<RentalProduct>(data);
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
            var query = "SELECT * FROM RentalProduct WHERE Id = @Id";
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
            WHERE Title LIKE CONCAT('%', @name, '%') 
            OR Description LIKE CONCAT('%', @name, '%')";

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
            var respose = await _connection.QueryAsync<RentalProduct,RentalImage,RentalSetDTO>("select * from RentalProduct join RentalImage on RentalProduct.id=RentalImage.productid   LIMIT @page_size OFFSET @offset_value ",
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
            ,new { page_size = pagesize, offset_value = (pagenumber - 1) * pagesize });
            var result = resultDict.Values.ToList();
            return new Response<IEnumerable<RentalViewDTO>>
            {
                statuscode = 200,
                data = result,
                message = "success"
            };
        }

    }


}
