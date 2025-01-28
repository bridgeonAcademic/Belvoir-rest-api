using AutoMapper;
using Belvoir.Bll.DTO.Delivery;
using Belvoir.Bll.DTO.Tailor;
using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories.DeliveryRep;

namespace Belvoir.Bll.Services.DeliverySer
{
    public interface IDeliveryServices
    {
        public Task<Response<DeliveryResponseDTO>> GetDeliveryProfile(Guid id);

    }
    public class DeliveryServices : IDeliveryServices
    {
        private readonly IDeliveryRepository _repo;
        private readonly IMapper _mapper;
        public DeliveryServices(IDeliveryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<DeliveryResponseDTO>> GetDeliveryProfile(Guid id)
        {
            var response = await _repo.SingleProfile(id);
            var mapped = _mapper.Map<DeliveryResponseDTO>(response);
            if (response == null)
            {
                return new Response<DeliveryResponseDTO> { statuscode = 404, message = "the profile doesnot exist", };
            }
            return new Response<DeliveryResponseDTO> { statuscode = 200, message = "success", data = mapped };
        }
    }
}
