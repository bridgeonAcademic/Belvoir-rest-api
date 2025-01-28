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
    public interface IClothesRepository
    {
        public Task<IEnumerable<Cloth>> GetClothes(ProductQuery query);
    }
    public class ClothesRepository : IClothesRepository
    {
        private readonly IDbConnection _dbConnection;
        public ClothesRepository(IDbConnection dbConnection) {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<Cloth>> GetClothes(ProductQuery query)
        {
            var parameters = new DynamicParameters();
            parameters.Add("pTitle", query.SearchTerm, DbType.String);
            parameters.Add("pMaterial", query.Category, DbType.String);
            parameters.Add("pDesignPattern", null, DbType.String); // Add if needed
            parameters.Add("pMinPrice", query.MinPrice, DbType.Decimal);
            parameters.Add("pMaxPrice", query.MaxPrice, DbType.Decimal);
            parameters.Add("pSortBy", query.SortBy, DbType.String);
            parameters.Add("pIsDescending", query.IsDescending, DbType.Boolean);
            parameters.Add("pPageSize", query.PageSize, DbType.Int32);
            parameters.Add("pPageNo", query.PageNo, DbType.Int32);
            var clothes = await _dbConnection.QueryAsync<Cloth>("GetCloths", parameters, commandType: CommandType.StoredProcedure);

            return clothes;
        }


    }

    
}
