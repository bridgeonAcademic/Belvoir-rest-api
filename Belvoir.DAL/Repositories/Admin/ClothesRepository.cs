using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IClothesRepository
    {
        public Task<IEnumerable<Cloth>> GetClothes(ProductQuery query);
        public Task<int> AddWhishlist(Guid userid, Guid productid);
        public Task<IEnumerable<WhishList>> GetWishlist(Guid userId);
        public Task<int> ExistItem(Guid userid, Guid productid);


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

        public async Task<int> AddWhishlist(Guid userid, Guid productid)
        {
            return await _dbConnection.ExecuteAsync("insert into ClothesWishList (user_id,rental_id) values(@id,@usrid,@prid)", new { usrid = userid, prid = productid });
        }


        public async Task<IEnumerable<WhishList>> GetWishlist(Guid userId)
        {
            var query = @"select WhishList.Id,Title,Description,Price,ImageUrl from WhishList join Cloths on WhishList.clothes_id =Cloths.id";
            return await _dbConnection.QueryAsync<WhishList>(query, new { usrid = userId });
        }


        public async Task<int> ExistItem(Guid userid, Guid productid)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync("select count(*) from WhishList where @user_id=usrid and clothes_id=prid)", new { usrid = userid, prid = productid });
        }

    }
}
