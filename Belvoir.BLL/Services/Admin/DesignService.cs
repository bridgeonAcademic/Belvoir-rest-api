using AutoMapper;
using Belvoir.Bll.DTO.Design;
using Belvoir.Bll.Helpers;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Admin;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Belvoir.Bll.Services.Admin
{
    public interface IDesignService
    {
        Task<Response<List<DesignDTO>>> GetDesignsAsync(DesignQueryParameters queryParams);
        Task<Response<DesignDTO>> GetDesignByIdAsync(Guid designId);

        Task<Response<string>> AddDesignAsync(Design design, List<IFormFile> imageFiles);

    }

    public class DesignService : IDesignService
    {
        public readonly IDesignRepository _designRepository;
        public readonly ICloudinaryService _cloudinaryService;
        public readonly IMapper _mapper;

        public DesignService(IDesignRepository designRepository, ICloudinaryService cloudinaryService, IMapper mapper)
        {
            _designRepository = designRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<Response<List<DesignDTO>>> GetDesignsAsync(DesignQueryParameters queryParams)
        {
            var designs = await _designRepository.GetDesignsAsync(queryParams);

            if (designs == null || designs.Count == 0)
            {
                return new Response<List<DesignDTO>>
                {
                    StatusCode = 404,
                    Message = "No designs found"
                };
            }
            var designDtos = _mapper.Map<List<DesignDTO>>(designs);

            return new Response<List<DesignDTO>>
            {
                StatusCode = 200,
                Message = "Designs retrieved successfully",
                Data = designDtos
            };
        }

        public async Task<Response<DesignDTO>> GetDesignByIdAsync(Guid designId)
        {
            var design = await _designRepository.GetDesignById(designId);

            if (design == null)
            {
                return new Response<DesignDTO>
                {
                    StatusCode = 404,
                    Message = "Design not found",
                    Data = null
                };
            }

            var designDto = _mapper.Map<DesignDTO>(design);

            return new Response<DesignDTO>
            {
                StatusCode = 200,
                Message = "Design retrieved successfully",
                Data = designDto
            };
        }



        public async Task<Response<string>> AddDesignAsync(Design design, List<IFormFile> imageFiles)
        {
            if (imageFiles == null || imageFiles.Count < 3)
            {
                return new Response<string>
                {
                    StatusCode = 400,
                    Message = "At least 3 images are required.",
                    Error = "Invalid number of images."
                };
            }

            design.Id = Guid.NewGuid();
            design.CreatedAt = DateTime.UtcNow;

            var imageList = new List<Image>();

            // Upload images to Cloudinary
            for (int i = 0; i < imageFiles.Count; i++)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(imageFiles[i]);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return new Response<string>
                    {
                        StatusCode = 500,
                        Message = "Image upload failed.",
                        Error = "Cloudinary service error."
                    };
                }

                imageList.Add(new Image
                {
                    Id = Guid.NewGuid(),
                    EntityId = design.Id,
                    ImageUrl = imageUrl,
                    IsPrimary = i == 0  // First image as primary
                });
            }

            design.Images = imageList;

            // Call repository to save design and images in a single transaction
            var result = await _designRepository.AddDesignWithImagesAsync(design);

            if (result > 0)
            {
                return new Response<string>
                {
                    StatusCode = 201,
                    Message = "Design added successfully.",
                    Data = design.Id.ToString()
                };
            }

            return new Response<string>
            {
                StatusCode = 500,
                Message = "Failed to add design.",
                Error = "Database insert error."
            };
        }

    }
}
