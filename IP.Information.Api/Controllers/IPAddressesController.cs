using IP.Information.Application.Interfaces;
using IP.Information.Contract.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IP.Information.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IPAddressesController : ControllerBase
    {
        private readonly IConnectionService _connectionService;
        private ILogger<IPAddressesController> _logger;
        private readonly IIPAddressStore _store;
        private readonly ICachingIPAddresses _cachingIPAddresses;
        private readonly IConfiguration _configuration;

        public IPAddressesController(ILogger<IPAddressesController> logger,
            IConnectionService connectionService, IIPAddressStore store, ICachingIPAddresses cachingIPAddresses, IConfiguration configuration)
        {
            _connectionService = connectionService;
            _store = store;
            _cachingIPAddresses = cachingIPAddresses;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IpAddressDto> GetIPAddress(string ip)
        {
            IpAddressDto res = new IpAddressDto();

            bool found = false;
            try
            {
                if (_cachingIPAddresses.IPAddressesDto != null && _cachingIPAddresses.IPAddressesDto.Any(x => x.IP.Equals(ip)))
                {
                    res = _cachingIPAddresses.IPAddressesDto.Where(x => x.IP.Equals(ip)).First();
                    found = true;
                }
                else if (!found && _store.Store.ContainsKey(ip))
                {
                    res = _store.Store[ip];
                    _cachingIPAddresses.AddIPAddress(res.Country, ip);
                    found = true;
                }
                else if (!found)
                {
                    res = await _connectionService.GetIPAddress(ip, true);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return res;
            }

            return res;
        }

        [HttpGet]
        public IQueryable<ReportDto> GetCountriesReport()
        {
            List<ReportDto> res = new List<ReportDto>();
            try
            {
                string? connString = _configuration.GetConnectionString("ConnStr");

                if (!string.IsNullOrEmpty(connString))
                {
                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        connection.Open();

                        var sql = @"with
                        LastUpdateAt as (
                          select [IP], max(UpdatedAt) as LastUpdateAt
                          from dbo.ipaddresses I
                          Group by I.[IP]
                        )
                        select C.[Name], count(I.[IP]) AddressesCount, lua.LastUpdateAt
                            from dbo.ipaddresses I
                            inner join dbo.Countries C on C.Id = I.CountryId
                            inner join LastUpdateAt lua
                              on lua.[IP] = I.[IP]
                              and lua.LastUpdateAt = I.UpdatedAt
                              group by C.[Name], lua.LastUpdateAt";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ReportDto reportDto = new ReportDto()
                                    {
                                        Name = reader.GetString(0),
                                        AddressesCount = reader.GetInt32(1),
                                        LastUpdateAt = reader.GetDateTime(2),
                                    };

                                    res.Add(reportDto);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return res.AsQueryable();
        }
    }
}