using Dapper;
using System;
using System.Collections.Generic;
using Belvoir.Bll.DTO.Rental;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Rental
{
    public interface IRentalCartRepository
    {
        Task<RentalCart?> GetCartByUserId(Guid userId);

        Task AddToCartAsync(Guid userId, Guid productId, int quantity);
        Task UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity);

    }

    public class RentalCartRepository : IRentalCartRepository
    {
        private readonly IDbConnection _dbConnection;

        public RentalCartRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<RentalCart?> GetCartByUserId(Guid userId)
        {
            const string query = "SELECT * FROM CartView WHERE UserId = @UserId";

            var cartDictionary = new Dictionary<Guid, RentalCart>();

            var result = await _dbConnection.QueryAsync<RentalCart, RentalCartItem, RentalCart>(
                query,
                (cart, cartItem) =>
                {
                    if (!cartDictionary.TryGetValue(cart.Id, out var cartEntry))
                    {
                        cartEntry = cart;
                        cartDictionary.Add(cart.Id, cartEntry);
                    }

                    cartEntry.Items.Add(cartItem);

                    return cartEntry;
                },
                new { UserId = userId },
                splitOn: "CartItemId"
            );

            return cartDictionary.Values.FirstOrDefault();
        }


        public async Task AddToCartAsync(Guid userId, Guid productId, int quantity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_UserId", userId);
            parameters.Add("p_ProductId", productId.ToString());
            parameters.Add("p_Quantity", quantity);

            await _dbConnection.ExecuteAsync("sp_AddToCart", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_CartItemId", cartItemId);
            parameters.Add("p_NewQuantity", newQuantity);

            await _dbConnection.ExecuteAsync("sp_UpdateCartItemQuantity", parameters, commandType: CommandType.StoredProcedure);
        }

    }

}
