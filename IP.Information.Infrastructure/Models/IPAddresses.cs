using System;

namespace IP.Information.Application.Models
{
    public class IPAddresses
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int CountryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}