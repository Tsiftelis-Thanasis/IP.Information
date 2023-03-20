using IP.Information.Application.Context;
using IP.Information.Application.Interfaces;
using IP.Information.Application.Models;
using IP.Information.Contract.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IP.Information.Application.Services
{
    public class IPAddressStore : IIPAddressStore
    {
        private IAppLogger<IPAddressStore> _logger;
        public ConcurrentDictionary<string, IpAddressDto> Store { get; set; }
        private readonly IServiceScopeFactory _scopeFactory;
        private AddressesContext _context;
        private List<IPAddresses> IPAddresses;
        private List<Countries> Countries;
        private bool _timeHasPassed;
        private int _startTime;

        public IPAddressStore(IServiceScopeFactory scopeFactory, IAppLogger<IPAddressStore> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AddressesContext>();

            _timeHasPassed = true;
            _startTime = Environment.TickCount;
            _timeHasPassed = (Environment.TickCount - _startTime > 10000);

            if (IPAddresses == null || _timeHasPassed)
                IPAddresses = _context.IPAddresses.AsNoTracking().ToList();
            if (Countries == null || _timeHasPassed)
                Countries = _context.Countries.AsNoTracking().ToList();
            if (Store == null || _timeHasPassed)
                Store = GetStoreIPAddresses();
        }

        private ConcurrentDictionary<string, IpAddressDto> GetStoreIPAddresses()
        {
            ConcurrentDictionary<string, IpAddressDto> ids = new ConcurrentDictionary<string, IpAddressDto>();
            try
            {
                foreach (var ip in IPAddresses)
                {
                    Countries country = Countries.Where(x => x.Id.Equals(ip.CountryId)).FirstOrDefault();
                    if (country != null)
                    {
                        CountryDto countryDto = country.ToCountryDTO();
                        IpAddressDto ipAddressDto = new IpAddressDto()
                        {
                            IP = ip.IP,
                            Country = countryDto,
                            CreatedAt = ip.CreatedAt,
                            UpdatedAt = ip.UpdatedAt
                        };

                        ids.TryAdd(ip.IP, ipAddressDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return ids;
        }

        public bool AddIPAddress(CountryDto country, string Ip, bool forceUpdate)
        {
            bool res = false;
            int countryId = 0;
            int ipaddressId = 0;

            if (Store.Count > 0)
                if (Store.ContainsKey(Ip))
                    RemoveIPAddress(Ip);
            try
            {
                if (forceUpdate)
                {
                    IPAddresses = _context.IPAddresses.AsNoTracking().ToList();
                    Countries = _context.Countries.AsNoTracking().ToList();
                    Store = GetStoreIPAddresses();
                }

                if (!Countries.Any(x => x.Id.Equals(country.Id)))
                {
                    Countries newCountry = new Countries()
                    {
                        Name = country.Name,
                        TwoLetterCode = country.TwoLetterCode,
                        ThreeLetterCode = country.ThreeLetterCode,
                        CreatedAt = DateTime.Now
                    };
                    _context.Countries.Add(newCountry);
                    _context.SaveChanges();
                    countryId = newCountry.Id;
                }
                else
                {
                    countryId = country.Id;
                }

                if (countryId > 0)
                {
                    IPAddresses iPAddress = new IPAddresses()
                    {
                        CountryId = countryId,
                        IP = Ip,

                        CreatedAt = DateTime.Now
                    };
                    _context.IPAddresses.Add(iPAddress);
                    _context.SaveChanges();

                    ipaddressId = iPAddress.Id;
                }

                if (ipaddressId > 0)
                {
                    res = Store.TryAdd(Ip, new IpAddressDto
                    {
                        Country = country,
                        IP = Ip,
                        CreatedAt = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return res;
        }

        public void RemoveIPAddress(string ip)
        {
            try
            {
                if (Store.ContainsKey(ip))
                {
                    Store.Remove(ip, out _);
                    if (Store.ContainsKey(ip))
                    {
                        Store.Remove(ip, out _);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}