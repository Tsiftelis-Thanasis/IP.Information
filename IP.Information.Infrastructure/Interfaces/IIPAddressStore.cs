using IP.Information.Contract.Dtos;
using System.Collections.Concurrent;

namespace IP.Information.Application.Interfaces
{
    public interface IIPAddressStore
    {
        public ConcurrentDictionary<string, IpAddressDto> Store { get; }

        bool AddIPAddress(CountryDto country, string Ip, bool forceUpdate);
    }
}