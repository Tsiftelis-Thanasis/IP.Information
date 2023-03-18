using IP.Information.Contract.Dtos;
using System;
using System.Collections.Concurrent;

namespace IP.Information.Application.Interfaces
{
    public interface ICachingIPAddresses
    {
        ConcurrentQueue<IpAddressDto> IPAddressesDto { get; set; }
        DateTime LastUpdate { get; set; }

        public void AddIPAddress(CountryDto country, string Ip);
    }
}