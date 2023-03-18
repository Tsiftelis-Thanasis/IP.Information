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

            //if (forceUpdate)
            //{
            //    DBIPAddresses = _context.IPAddresses.AsNoTracking().ToList();
            //    DBCountries = _context.Countries.AsNoTracking().ToList();
            //}

            //if (Store.Count > 0)
            //    if (Store.ContainsKey(Ip))
            //        RemoveIPAddress(Ip);

            //bool res = Store.TryAdd(Ip, new IpAddressDto
            //{
            //    Country = country,
            //    IP = Ip,
            //    CreatedAt = DateTime.Now
            //});

            //if (res)
            //{
            //    int countryId = 0;
            //    if (!Countries.Any(x => x.Id.Equals(country.Id)))
            //    {
            //        Countries newCountry = new Countries()
            //        {
            //            Name = country.Name,
            //            TwoLetterCode = country.TwoLetterCode,
            //            ThreeLetterCode = country.ThreeLetterCode,
            //            CreatedAt = DateTime.Now
            //        };
            //        _context.Countries.Add(newCountry);
            //        _context.SaveChanges();
            //        countryId = newCountry.Id;
            //    }
            //    else
            //    {
            //        countryId = country.Id;

            //    }

            //    if (countryId > 0)
            //    {
            //        IPAddresses iPAddress = new IPAddresses()
            //        {
            //            CountryId = countryId,
            //            IP = Ip,

            //            CreatedAt = DateTime.Now
            //        };
            //        _context.IPAddresses.Add(iPAddress);
            //        _context.SaveChanges();
            //    }

            //}
        }

        //public void RemoveIPAddress(string ip)
        //{
        //    if (Store.ContainsKey(ip))
        //    {
        //        Store.Remove(ip, out _);
        //        if (Store.ContainsKey(ip))
        //        {
        //            Store.Remove(ip, out _);
        //        }
        //    }
        //}
    }
}