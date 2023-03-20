using IP.Information.Application.Interfaces;
using IP.Information.Contract.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IP.Information.Application.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly Uri _baseUri;
        private IAppLogger<ConnectionService> _logger;
        private HttpClient _httpClient;
        private ICachingIPAddresses _cachingIPAddresses;
        private int _startTime;
        private IIPAddressStore _store;

        public ConnectionService(IConfiguration configuration, IAppLogger<ConnectionService> logger, ICachingIPAddresses cachingIPAddresses, IIPAddressStore store)
        {
            _baseUri = new Uri(configuration["ip2cUri"]);
            _logger = logger;
            _startTime = Environment.TickCount;
            _cachingIPAddresses = cachingIPAddresses;
            _store = store;
        }

        public async Task<IpAddressDto> GetIPAddress(string ip, bool addToStore)
        {
            IpAddressDto ipAddressDto = new IpAddressDto();
            try
            {
                string result = await CallRoutingRestService(ip);
                var vars = result.Split(';');

                if (vars != null)
                {
                    CountryDto country = new CountryDto()
                    {
                        Id = int.Parse(vars[0]),
                        TwoLetterCode = vars[1],
                        ThreeLetterCode = vars[2],
                        Name = vars[3],
                        CreatedAt = DateTime.Now
                    };

                    ipAddressDto.CreatedAt = DateTime.Now;
                    ipAddressDto.Country = country;
                    ipAddressDto.IP = ip;

                    if (addToStore)
                    {
                        _cachingIPAddresses.AddIPAddress(country, ip);
                        _store.AddIPAddress(country, ip, true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return ipAddressDto;
        }

        private async Task<string> CallRoutingRestService(string uriTemplate)
        {
            _httpClient = GetClient();
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(uriTemplate))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation(content);
                        return content;
                    }
                    else
                    {
                        _logger.LogError($"{response.StatusCode} {response.Content}");
                        throw new HttpRequestException($"BadRequest for app connection {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(_baseUri.AbsoluteUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            client.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

            return client;
        }
    }
}