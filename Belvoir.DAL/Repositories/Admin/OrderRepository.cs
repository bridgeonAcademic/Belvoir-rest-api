using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IOrderRepository
    {
         public Task<bool> AddTailorProduct(TailorProduct tailorProduct);
         public Task<bool> IsClothExists(Guid Id);
         public Task<bool> IsDesignExists(Guid Id);
         public Task<bool> AddOrder(Order order);
         public Task<IEnumerable<OrderTailorGet>> orderTailorGets(); 
    }
    public class OrderRepository: IOrderRepository
    {
        private readonly IDbConnection _dbConnection;
        public OrderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AddTailorProduct(TailorProduct tailorProduct)
        {
            string query = "INSERT INTO `belvoir`.`tailor_products` (`product_id`,`customer_id`,`design_id`,`cloth_id`,`product_name`,`price`) VALUES (UUID(),'918eab05-0a0b-42a9-9ce6-2cc973c9eb3a',@designid,@clothid,@productname,@price)";
            return await _dbConnection.ExecuteAsync(query, new { clothid = tailorProduct.ClothId, userid = tailorProduct.UserId,designid = tailorProduct.DesignId,productname = tailorProduct.product_name,price = tailorProduct.price })>0;
        }
        public async Task<bool> IsClothExists(Guid Id)
        {
            string query = "SELECT Count(*) FROM Cloths WHERE Id = @Id";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { Id }) > 0;
        }
        public async Task<bool> IsDesignExists(Guid Id)
        {
            string query = "SELECT Count(*) FROM DressDesign WHERE Id = @Id";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { Id }) > 0;
        }
        public async Task<bool> AddOrder(Order order)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_customer_id", "918eab05-0a0b-42a9-9ce6-2cc973c9eb3a");
            parameters.Add("@p_order_date", order.orderDate);
            parameters.Add("@p_total_amount", order.totalAmount);
            parameters.Add("@p_payment_method", order.paymentMethod);
            parameters.Add("@p_shipping_address", order.shippingAddress);
            parameters.Add("@p_shipping_method", order.shippingMethod);
            parameters.Add("@p_shipping_cost", order.shippingCost);
            parameters.Add("@p_tracking_number", order.trackingNumber);
            parameters.Add("@p_updated_by", order.updatedBy);
            parameters.Add("@p_product_type", order.productType);
            parameters.Add("@p_tailor_product_id", order.tailorProductId);
            parameters.Add("@p_rental_product_id", order.rentalProductId);
            parameters.Add("@p_quantity", order.quantity);
            parameters.Add("@p_price", order.price);

            return await _dbConnection.ExecuteAsync("InsertOrderWithItems", parameters, commandType: CommandType.StoredProcedure)>0;

        }
        public async Task<IEnumerable<OrderTailorGet>> orderTailorGets()
        {
            string query = "SELECT orders.order_id,Name,order_date,order_status FROM orders join order_items on orders.order_id = order_items.order_id join User on User.Id = orders.customer_id";
            return await _dbConnection.QueryAsync<OrderTailorGet>(query);
        }


    }
}
