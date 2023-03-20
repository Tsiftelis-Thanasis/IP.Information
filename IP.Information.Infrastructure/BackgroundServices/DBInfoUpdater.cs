using IP.Information.Application.Context;
using IP.Information.Application.Interfaces;
using IP.Information.Application.Models;
using IP.Information.Application.Services;
using IP.Information.Contract.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IP.Information.Application.BackgroundServices
{
    public class DBInfoUpdater : BackgroundService
    {
        private readonly IConnectionService _connectionService;
        private readonly IServiceScopeFactory _scopeFactory;
        private AddressesContext _context;
        private List<IPAddresses> IPAddresses;
        private List<Countries> Countries;
        private IAppLogger<IPAddressStore> _logger;

        private double _lasttick = 0;
        private double _lastUpdate = 0;
        private double _updateTick = 10000;
        private DateTime _LastUpdate;

        public DBInfoUpdater(IServiceScopeFactory scopeFactory, IConnectionService connectionService, IAppLogger<IPAddressStore> logger)
        {
            _connectionService = connectionService;
            _scopeFactory = scopeFactory;
            _context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AddressesContext>();
            _lasttick = 0;
            _LastUpdate = DateTime.Now;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _lasttick = Environment.TickCount;
                if (_lasttick >= _lastUpdate + _updateTick)
                {
                    _lastUpdate = _lasttick;
                    await AsyncUpdate();
                }
            }
        }

        protected async Task AsyncUpdate()
        {
            try
            {
                IPAddresses = _context.IPAddresses.ToList();
                Countries = _context.Countries.ToList();

                foreach (IPAddresses ip in IPAddresses)
                {
                    IpAddressDto res = new IpAddressDto();

                    res = await _connectionService.GetIPAddress(ip.IP, false);

                    if (res != null)
                    {
                        int newCountryId = 0;
                        var IpAddress = IPAddresses.Where(x => x.IP.Equals(ip.IP)).FirstOrDefault();
                        var country = Countries.Where(x => x.Name.Equals(res.Country.Name)).FirstOrDefault();

                        //from the requirements it's not clear
                        if (country != null)
                        {
                            country.TwoLetterCode = res.Country.TwoLetterCode;
                            country.ThreeLetterCode = res.Country.ThreeLetterCode;
                            country.UpdatedAt = _LastUpdate;
                            _context.SaveChanges();
                        }
                        else
                        {
                            Countries newCountry = new Countries()
                            {
                                Name = res.Country.Name,
                                TwoLetterCode = res.Country.TwoLetterCode,
                                ThreeLetterCode = res.Country.ThreeLetterCode,
                                CreatedAt = DateTime.Now
                            };
                            _context.Countries.Add(newCountry);
                            _context.SaveChanges();
                            newCountryId = newCountry.Id;
                        }

                        Countries = _context.Countries.ToList();

                        if (newCountryId > 0)
                        {
                            IpAddress.CountryId = newCountryId;
                            IpAddress.UpdatedAt = _LastUpdate;
                            _context.SaveChanges();
                        }
                        else if (ip.CountryId != country.Id)
                        {
                            IpAddress.CountryId = country.Id;
                            IpAddress.UpdatedAt = _LastUpdate;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await Task.Delay(1);
            }
        }
    }
}