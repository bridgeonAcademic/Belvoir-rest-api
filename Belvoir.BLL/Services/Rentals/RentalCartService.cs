using Belvoir.Bll.DTO.Rental;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Rentals
{
    public interface IRentalCartService
    {
        Task<Response<string>> AddToCartAsync(string userId, AddToCartDTO cartDTO);
    }

    public class RentalCartService : IRentalCartService
    {
        private readonly IRentalCartRepository _repository;

        public RentalCartService(IRentalCartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<string>> AddToCartAsync(string userId, AddToCartDTO cartDTO)
        {
            try
            {
                await _repository.ExecuteAddToCartProcedure(userId, cartDTO.ProductId, cartDTO.Quantity);

                return new Response<string>
                {
                    statuscode = 200,
                    message = "Product added to cart successfully",
                    data = null
                };
            }
            catch (Exception ex)
            {
                // This will be caught by the global exception handler.
                throw new ApplicationException("Error adding product to cart", ex);
            }
        }
    }

}
