using AutoMapper;
using Belvoir.Bll.DTO.Rental;
using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
using CloudinaryDotNet;
using Dapper;
using Microsoft.AspNetCore.Http;
//using Org.BouncyCastle.Bcpg;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using CloudinaryDotNet;

namespace Belvoir.Bll.Services.Rentals
{
    public interface IRentalService
    {
        public Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO data, Guid userid);

        public Task<Response<RentalViewDTO>> GetRentalById(Guid id);

        public  Task<Response<IEnumerable<RentalViewDTO>>> SearchRental(string name);

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
        private readonly IRentalRepository _repo;
        public RentalSevice(IDbConnection connection, IMapper mapper, ICloudinaryService cloudinary, IRentalRepository repo)
        {
            _connection = connection;
            _mapper = mapper;
            _cloudinaryService = cloudinary;
            _repo = repo;
        }

        public async Task<Response<object>> AddRental(IFormFile[] files, RentalSetDTO data, Guid userId)
        {
            if (files.Length > 3)
            {
                return new Response<object>
                {
                    StatusCode = 400,
                    Error = "You can only upload a maximum of 3 images."
                };
            }

            var categoryExists = await _repo.CetegoryExist(data.fabrictype);
            if (categoryExists==0)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Category does not exist."
                };
            }

            var rentalProduct = _mapper.Map<RentalProduct>(data);
            rentalProduct.CreatedBy = userId;
            rentalProduct.Id = Guid.NewGuid();
            rentalProduct.CreatedAt = DateTime.UtcNow;
            rentalProduct.IsDeleted = false;
            var result = await _repo.AddRentalProductAsync(rentalProduct,userId);

            for (int i = 0; i < files.Length; i++)
            {
                var url = await _cloudinaryService.UploadImageAsync(files[i]);

                bool isPrimary = i == 0;
                await _repo.AddRentalImage(rentalProduct.Id, url, isPrimary);
               

            }


            return new Response<object>
            {
                StatusCode = 200,
                Message = "Rental item added successfully."
            };
        }


        public async Task<Response<RentalViewDTO>> GetRentalById(Guid id)
        {
            var rental = await _repo.GetRentalProductById(id);

            if (rental == null)
            {
                return new Response<RentalViewDTO>
                {
                    StatusCode = 404,
                    Error = "Rental item not found"
                };
            }

            var imagesresponse = await _repo.GetRentalImagesByProductId(id);

            var mapped = _mapper.Map<RentalViewDTO>(rental);
            mapped.images = imagesresponse.ToList();

            return new Response<RentalViewDTO>
            {
                Message = "Rental item retrieved successfully",
                StatusCode = 200,
                Data = mapped
            };
        }

       


        public async Task<Response<IEnumerable<RentalViewDTO>>> SearchRental(string name)
        {
            var rawData = await _repo.SearchRentalsByName(name);

            var resultDict = new Dictionary<Guid, RentalViewDTO>(); 

            foreach (var (rentalproduct, rentalimage) in rawData)
            {
                var mapped = _mapper.Map<RentalViewDTO>(rentalproduct);

                if (!resultDict.ContainsKey(rentalproduct.Id))
                {
                    mapped.images = new List<RentalImage>();
                    resultDict[rentalproduct.Id] = mapped;
                }

                resultDict[rentalproduct.Id].images.Add(rentalimage);
            }

            var finalResults = resultDict.Values.ToList();

            if (!finalResults.Any())
            {
                return new Response<IEnumerable<RentalViewDTO>>
                {
                    StatusCode = 404,
                    Error = "No rental products found matching the search criteria."
                };
            }

            return new Response<IEnumerable<RentalViewDTO>>
            {
                Message = "Rental products retrieved successfully.",
                StatusCode = 200,
                Data = finalResults
            };
        }



        public async Task<Response<IEnumerable<RentalViewDTO>>> PaginatedProduct(int pagenumber, int pagesize)
        {
            var rawData = await _repo.GetRentalProductsAsync(pagenumber, pagesize);
            Console.WriteLine("the Data is :", rawData);
            var resultDict = new Dictionary<string, RentalViewDTO>();

            foreach (var (rentalProduct, rentalImage) in rawData)
            {
                var mapped = _mapper.Map<RentalViewDTO>(rentalProduct);
                if (!resultDict.ContainsKey(rentalProduct.Id.ToString()))
                {
                    mapped.images = new List<RentalImage>();
                    resultDict[rentalProduct.Id.ToString()] = mapped;
                }
                resultDict[rentalProduct.Id.ToString()].images.Add(rentalImage);
            }
            var result= resultDict.Values.ToList();
            return new Response<IEnumerable<RentalViewDTO>>
            {
                StatusCode = 200,
                Data = result,
                Message = "success"
            };
        }

        public async Task<Response<object>> DeleteRental(Guid rentalId, Guid userid)
        {
            
                var rentalProduct = await _repo.GetRentalProductById(rentalId);

                if (rentalProduct == null)
                {
                    return new Response<object>
                    {
                        StatusCode = 404,
                        Error = "Rental product not found"
                    };
                }

                await _repo.RentalProductAsDeleted(rentalId, userid);

                return new Response<object>
                {
                    Message = "Rental item deleted successfully",
                    StatusCode = 200
                };
            }


        public async Task<Response<object>> UpdateRental(Guid rentalId, IFormFile[] files, RentalSetDTO data, Guid userId)
        {
            // Check if rental product exists
            var rentalProduct = await _repo.GetRentalProductById(rentalId);
            if (rentalProduct == null)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Rental product not found"
                };
            }

            var fabric = await _repo.CetegoryExist(data.fabrictype);
            if (fabric == null)
            {
                return new Response<object>
                {
                    StatusCode = 404,
                    Error = "Fabric category does not exist"
                };
            }

            var mappedProduct = _mapper.Map<RentalProduct>(data);
            mappedProduct.Id = rentalId;
            mappedProduct.UpdatedBy = userId;
            mappedProduct.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateRentalProduct(mappedProduct);

            if (files != null && files.Length > 0)
            {
                await _repo.DeleteRentalImages(rentalId);

                for (int i = 0; i < files.Length; i++)
                {
                    var imagePath = await _cloudinaryService.UploadImageAsync(files[i]);
                    await _repo.AddRentalImage(rentalId, imagePath, i == 0);
                  
                }
            }

            return new Response<object>
            {
                Message = "Rental item updated successfully",
                StatusCode = 200
            };
        }



        public async Task<Response<IEnumerable<RentalViewDTO>>> GetRentalsByCategory(string gender, string garmentType, string fabricType)
        {
            var rawData = await _repo.GetRentalsByCategoryAsync(gender, garmentType, fabricType);

            var resultDict = new Dictionary<string, RentalViewDTO>();

            foreach (var (rentalProduct, rentalImage) in rawData)
            {
                if (!resultDict.ContainsKey(rentalProduct.Id.ToString()))
                {
                    var mapped = _mapper.Map<RentalViewDTO>(rentalProduct);
                    mapped.images = new List<RentalImage>();
                    resultDict[rentalProduct.Id.ToString()] = mapped;
                }

                if (rentalImage != null)
                {
                    resultDict[rentalProduct.Id.ToString()].images.Add(rentalImage);
                }
            }

            var rentals = resultDict.Values.ToList();

            if (!rentals.Any())
            {
                return new Response<IEnumerable<RentalViewDTO>>
                {
                    StatusCode = 404,
                    Error = "No rentals found for the specified category"
                };
            }

            return new Response<IEnumerable<RentalViewDTO>>
            {
                Message = "Rental items retrieved successfully",
                StatusCode = 200,
                Data = rentals
            };
        }


    }
}
