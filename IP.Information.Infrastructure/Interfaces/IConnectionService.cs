using IP.Information.Contract.Dtos;
using System.Threading.Tasks;

namespace IP.Information.Application.Interfaces
{
    public interface IConnectionService
    {
        Task<IpAddressDto> GetIPAddress(string ip, bool addToStore);
    }
}