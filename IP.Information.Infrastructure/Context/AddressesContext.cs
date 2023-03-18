using IP.Information.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace IP.Information.Application.Context
{
    public class AddressesContext : DbContext
    {
        public AddressesContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<IPAddresses> IPAddresses { get; set; }

        public DbSet<Countries> Countries { get; set; }
    }
}