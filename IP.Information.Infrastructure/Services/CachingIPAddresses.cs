using IP.Information.Application.Context;
using IP.Information.Application.Interfaces;
using IP.Information.Application.Models;
using IP.Information.Contract.Dtos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IP.Information.Application.Services
{
    public class CachingIPAddresses : ICachingIPAddresses
    {
        public ConcurrentQueue<IpAddressDto> IPAddressesDto { get; set; }
        public DateTime LastUpdate { get; set; }

        private readonly IServiceScopeFactory _scopeFactory;
        private AddressesContext _context;
        private List<IPAddresses> DBIPAddresses;
        private List<Countries> DBCountries;
        private bool _timeHasPassed;
        private int _startTime;

        public CachingIPAddresses(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AddressesContext>();

            _timeHasPassed = true;
            _startTime = Environment.TickCount;
            _timeHasPassed = (Environment.TickCount - _startTime > 10000);
        }

        public void AddIPAddress(CountryDto country, string Ip)
        {
            IpAddressDto IPAddress = new IpAddressDto()
            {
                IP = Ip,
                Country = country,
                CreatedAt = DateTime.Now
            };

            if (IPAddressesDto == null)
            {
                IPAddressesDto = new ConcurrentQueue<IpAddressDto>();
                IPAddressesDto.Enqueue(IPAddress);
                LastUpdate = DateTime.Now;
            }
            else
            {
                IPAddressesDto.Enqueue(IPAddress);
                LastUpdate = DateTime.Now;
            }

    }
}