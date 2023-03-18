using System;

namespace IP.Information.Contract.Dtos
{
    public class IpAddressDto
    {
        public string IP { get; set; }
        public CountryDto Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}