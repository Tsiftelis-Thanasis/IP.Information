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
        private IAppLogger<ConnectionService> _logger;
        public ConcurrentQueue<IpAddressDto> IPAddressesDto { get; set; }
        public DateTime LastUpdate { get; set; }

        private bool _timeHasPassed;
        private int _startTime;

        public CachingIPAddresses(IAppLogger<ConnectionService> logger)
        {
            _timeHasPassed = true;
            _startTime = Environment.TickCount;
            _timeHasPassed = (Environment.TickCount - _startTime > 10000);
            _logger = logger;
        }

        public void AddIPAddress(CountryDto country, string Ip)
        {
            try
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
            catch (Exception  ex)
            {
                _logger.LogError(ex.Message);
            }

        }
    }
}