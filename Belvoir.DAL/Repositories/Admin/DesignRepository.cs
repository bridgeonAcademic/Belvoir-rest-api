using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.Admin
{
    public interface IDesignRepository
    {
        //Task<int> AddDressDesignAsync(DressDesign dressDesign);
        //Task<int> AddDesignImagesAsync(IEnumerable<DesignImage> designImages);
    }

    public class DesignRepository
    {
        //public async Task<IEnumerable<Design>> GetDesigns(DesignQuery query)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("pName", query.Name, DbType.String);
        //    parameters.Add("pCategory", query.Category, DbType.String);
        //    parameters.Add("pMinPrice", query.MinPrice, DbType.Decimal);
        //    parameters.Add("pMaxPrice", query.MaxPrice, DbType.Decimal);
        //    parameters.Add("pAvailable", query.Available, DbType.Boolean);
        //    parameters.Add("pSortBy", query.SortBy, DbType.String);
        //    parameters.Add("pIsDescending", query.IsDescending, DbType.Boolean);
        //    parameters.Add("pPageSize", query.PageSize, DbType.Int32);
        //    parameters.Add("pPageNo", query.PageNo, DbType.Int32);
        //    var designs = await _dbConnection.QueryAsync<Design>("SearchDressDesignsWithImages", parameters, commandType: CommandType.StoredProcedure);

        //    return designs;

        //}
    }
}
