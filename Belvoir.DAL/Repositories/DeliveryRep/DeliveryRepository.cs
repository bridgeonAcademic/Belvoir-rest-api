using Belvoir.DAL.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.DAL.Repositories.DeliveryRep
{
    public interface IDeliveryRepository
    {
        public Task<Delivery> SingleProfile(Guid id);

    }
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly IDbConnection _dbConnection;

        public DeliveryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Delivery> SingleProfile(Guid userid)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Delivery>("select * from User left join DeliveryProfile on User.id=DeliveryProfile.Userid where User.id=@id", new { id = userid });

        }

    }
}
