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
        Task ExecuteAddToCartProcedure(string userId, Guid productId, int quantity);
    }

    public class RentalCartRepository : IRentalCartRepository
    {
        private readonly IDbConnection _dbConnection;

        public RentalCartRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<RentalCart?> GetCartViewByUserId(Guid userId)
        {
            const string storedProcedure = "GetCartViewByUserId";

            var cartDictionary = new Dictionary<Guid, RentalCart>();

            var result = await _dbConnection.QueryAsync<RentalCart, RentalCartItem, RentalCart>(
                storedProcedure,
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
                new { userIdParam = userId },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id"
            );

            return cartDictionary.Values.FirstOrDefault();
        }

        public async Task ExecuteAddToCartProcedure(string userId, Guid productId, int quantity)
        {
            // Define the parameters for the stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("p_UserId", userId);
            parameters.Add("p_ProductId", productId.ToString());
            parameters.Add("p_Quantity", quantity);

            // Execute the stored procedure
            await _dbConnection.ExecuteAsync(
                "sp_AddToCart",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }

}
