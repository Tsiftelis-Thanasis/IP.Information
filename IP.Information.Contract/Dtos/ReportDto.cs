using System;

namespace IP.Information.Contract.Dtos
{
    public class ReportDto
    {
        public string Name { get; set; }
        public int AddressesCount { get; set; }
        public DateTime LastUpdateAt { get; set; }
    }
}