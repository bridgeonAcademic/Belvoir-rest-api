using Belvoir.Bll.DTO.Rental;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.Rental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Rentals
{
    public interface IRentalCartService
    {
        Task<Response<RentalCart>> GetCartByUserId(Guid userId);

        Task<Response<string>> AddToCartAsync(Guid userId, AddToCartDTO cartDTO);
    }

    public class RentalCartService : IRentalCartService
    {
        private readonly IRentalCartRepository _repository;

        public RentalCartService(IRentalCartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<RentalCart>> GetCartByUserId(Guid userId)
        {
            var cart = await _repository.GetCartByUserId(userId);

            if (cart == null)
            {
                return new Response<RentalCart>
                {
                    StatusCode = 404,
                    Message = "Cart not found"
                };
            }

            return new Response<RentalCart>
            {
                StatusCode = 200,
                Message = "Cart retrieved successfully",
                Data = cart
            };
        }

        public async Task<Response<string>> AddToCartAsync(Guid userId, AddToCartDTO cartDTO)
        {
            try
            {
                // Fetch the current cart to check if the user has reached the item limit
                var currentCart = await _repository.GetCartByUserId(userId);
                if (currentCart == null)
                {
                    return new Response<string>
                    {
                        StatusCode = 404,
                        Message = "Cart not found for the user.",
                        Error = "No cart associated with the user.",
                        Data = null
                    };
                }

                // Check if the cart already has 20 items
                if (currentCart.Items.Count >= 20)
                {
                    return new Response<string>
                    {
                        StatusCode = 400,
                        Message = "Cart limit reached.",
                        Error = "You can only have a maximum of 20 items in your cart.",
                        Data = null
                    };
                }

                // Check if the product already exists in the cart
                var existingCartItem = currentCart.Items
                    .FirstOrDefault(item => item.ProductId == cartDTO.ProductId);

                // If the product already exists, update its quantity
                if (existingCartItem != null)
                {
                    var newQuantity = existingCartItem.Quantity + cartDTO.Quantity;
                    if (newQuantity > 10)
                    {
                        return new Response<string>
                        {
                            StatusCode = 400,
                            Message = "Quantity limit exceeded.",
                            Error = "You can only have a maximum of 10 of the same product in your cart.",
                            Data = null
                        };
                    }

                    // Update the cart item with the new quantity
                    await _repository.UpdateCartItemQuantityAsync(existingCartItem.ItemId, newQuantity);
                }
                else
                {
                    // If the product doesn't exist in the cart, add a new item
                    if (cartDTO.Quantity > 10)
                    {
                        return new Response<string>
                        {
                            StatusCode = 400,
                            Message = "Quantity limit exceeded.",
                            Error = "You can only add a maximum of 10 of the same product to your cart.",
                            Data = null
                        };
                    }

                    // Add a new cart item with the provided quantity
                    await _repository.AddToCartAsync(userId, cartDTO.ProductId, cartDTO.Quantity);
                }

                // After the add/update, return success
                return new Response<string>
                {
                    StatusCode = 200,
                    Message = "Product added to cart successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<string>
                {
                    StatusCode = 500,
                    Message = "An error occurred while adding the product to the cart.",
                    Error = ex.Message,
                    Data = null
                };
            }
        }

    }

}
