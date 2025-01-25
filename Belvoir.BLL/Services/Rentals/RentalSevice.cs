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

        public Task<Response<object>> AddWishlist(Guid userId, Guid productId);
        public Task<Response<IEnumerable<RentalWhishListviewDTO>>> GetWishlist(Guid userId);



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
                    statuscode = 400,
                    error = "You can only upload a maximum of 3 images."
                };
            }

            var categoryExists = await _repo.CetegoryExist(data.fabrictype);
            if (categoryExists==0)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "Category does not exist."
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
                statuscode = 200,
                message = "Rental item added successfully."
            };
        }


        public async Task<Response<RentalViewDTO>> GetRentalById(Guid id)
        {
            var rental = await _repo.GetRentalProductById(id);

            if (rental == null)
            {
                return new Response<RentalViewDTO>
                {
                    statuscode = 404,
                    error = "Rental item not found"
                };
            }

            var imagesresponse = await _repo.GetRentalImagesByProductId(id);

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
            var rawData = await _repo.GetRentalProductsAsync(pagenumber, pagesize);
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
                statuscode = 200,
                data = result,
                message = "success"
            };
        }

        public async Task<Response<object>> DeleteRental(Guid rentalId, Guid userid)
        {
            
                var rentalProduct = await _repo.GetRentalProductById(rentalId);

                if (rentalProduct == null)
                {
                    return new Response<object>
                    {
                        statuscode = 404,
                        error = "Rental product not found"
                    };
                }

                await _repo.RentalProductAsDeleted(rentalId, userid);

                return new Response<object>
                {
                    message = "Rental item deleted successfully",
                    statuscode = 200
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
                    statuscode = 404,
                    error = "Rental product not found"
                };
            }

            var fabric = await _repo.CetegoryExist(data.fabrictype);
            if (fabric == null)
            {
                return new Response<object>
                {
                    statuscode = 404,
                    error = "Fabric category does not exist"
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
                message = "Rental item updated successfully",
                statuscode = 200
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
                    statuscode = 404,
                    error = "No rentals found for the specified category"
                };
            }

            return new Response<IEnumerable<RentalViewDTO>>
            {
                message = "Rental items retrieved successfully",
                statuscode = 200,
                data = rentals
            };
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
        public async Task<Response<IEnumerable<RentalWhishListviewDTO>>> GetWishlist(Guid userId)
        {
            var rawData = await _repo.GetWishlist(userId);

            var resultDict = new Dictionary<string, RentalWhishListviewDTO>();

            foreach (var (rentalProduct, rentalImage) in rawData)
            {
                if (!resultDict.ContainsKey(rentalProduct.ProductId.ToString()))
                {
                    var mapped = _mapper.Map<RentalWhishListviewDTO>(rentalProduct);
                    mapped.images = new List<RentalImage>();
                    resultDict[rentalProduct.ProductId.ToString()] = mapped;
                }

                if (rentalImage != null)
                {
                    resultDict[rentalProduct.ProductId.ToString()].images.Add(rentalImage);
                }
            }

            var rentals = resultDict.Values.ToList();

            return new Response<IEnumerable<RentalWhishListviewDTO>>
            {
                data = rentals,
                statuscode = 200,
                message = "Wishlist retrieved successfully."
            };

        }


    }
}
