using AutoMapper;
using Belvoir.DTO.Rental;
using Belvoir.Helpers;
using Belvoir.Models;
using CloudinaryDotNet;
using Dapper;
using System.Data;

namespace Belvoir.Services.Rentals
{
    public interface IRentalService
    {
        public Task<Response<object>>AddRental(RentalSetDTO data);

    }

    public class RentalSevice:IRentalService
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;
        public RentalSevice(IDbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }
        public async Task<Response<object>> AddRental(RentalSetDTO data)
        {
            var fabric = await _connection.QueryFirstOrDefaultAsync("select * from FabricCategory where id=@id ", new { id = data.fabrictype });
            if (fabric == null)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "category doesnot exist"
                };

            }
            //var image = _cloudinaryService.UploadImageAsync(file);
            var query = @"
            INSERT INTO RentalProduct (Id,Title, Description, OfferPrice, Price, FabricType, Gender, GarmentType)
            VALUES (@Id,@Title, @Description, @OfferPrice, @Price, @FabricType, @Gender, @GarmentType)";
            var mapped = _mapper.Map<RentalProduct>(data);
            mapped.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(query,mapped);
            return new Response<object>
            {
                message = "Rental item addded successfully",
                statuscode = 200

            };

        }
       

    }
}
