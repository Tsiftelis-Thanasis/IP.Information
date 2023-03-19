using IP.Information.Application.Interfaces;
using IP.Information.Contract.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IP.Information.Api.Controllers
{
    public class TestIPAddressesController : Controller
    {
        private readonly IConnectionService _connectionService;
        private ILogger<IPAddressesController> _logger;
        private readonly IIPAddressStore _store;
        private readonly ICachingIPAddresses _cachingIPAddresses;
        private readonly IConfiguration _configuration;
        private readonly IPAddressesController _controller;

        public TestIPAddressesController(ILogger<IPAddressesController> logger,
            IConnectionService connectionService, IIPAddressStore store, ICachingIPAddresses cachingIPAddresses, IConfiguration configuration)
        {
            _connectionService = connectionService;
            _store = store;
            _cachingIPAddresses = cachingIPAddresses;
            _logger = logger;
            _configuration = configuration;
            _controller = new IPAddressesController(_logger, _connectionService, _store, _cachingIPAddresses, _configuration);
        }

        [InlineData("10.10.10.10")]
        [InlineData("10.11.10.11")]
        [InlineData("10.11.11.11")]
        [Theory]
        public void GetIpAddress(string ip)
        {
            var ipAddress = GetDemoIpAddressDto();

            var result = _controller.GetIPAddress(ip);

            Assert.Equal(ipAddress, result.Result);
        }

        [Fact]
        public void GetCountriesReport()
        {
            var result = _controller.GetCountriesReport();
            Assert.Single<ReportDto>(result);
            Assert.NotEmpty(result);
            Assert.IsAssignableFrom<IQueryable<ReportDto>>(result);
        }

        private IpAddressDto GetDemoIpAddressDto()
        {
            CountryDto countryDto = new CountryDto()
            {
                Id = 1,
                Name = "Greece",
                TwoLetterCode = "GR",
                ThreeLetterCode = "GRC",
                CreatedAt = DateTime.Now
            };

            return new IpAddressDto()
            {
                Country = countryDto,
                IP = "10.10.10.10",
                CreatedAt = DateTime.Now
            };
        }
    }
}