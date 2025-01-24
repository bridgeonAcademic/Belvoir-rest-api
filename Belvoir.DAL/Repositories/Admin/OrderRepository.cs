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
            string query = "INSERT INTO `belvoir`.`tailor_products` (`product_id`,`customer_id`,`design_id`,`cloth_id`,`product_name`,`price`) VALUES (UUID(),@userid,@designid,@clothid,@productname,@price)";
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

    }
}
