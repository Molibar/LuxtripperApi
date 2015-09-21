using System.ServiceModel;
using System.Threading.Tasks;

namespace Luxtripper.Api.WcfServices
{
    [ServiceContract]
    public interface IHotelService
    {
        [OperationContract]
        Task<LoginResponse> Login(LoginRequest request);
        [OperationContract]
        Task<HotelResponse> GetHotels(HotelRequest request);
    }
}
