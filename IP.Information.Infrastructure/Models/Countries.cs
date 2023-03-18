using IP.Information.Contract.Dtos;
using System;

namespace IP.Information.Application.Models
{
    public class Countries
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CountryDto ToCountryDTO()
        {
            return new CountryDto
            {
                Id = Id,
                Name = Name,
                TwoLetterCode = TwoLetterCode,
                ThreeLetterCode = ThreeLetterCode,
                CreatedAt = CreatedAt
            };
        }
    }
}