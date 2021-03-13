using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortner
{
    public class ShortnerContext:DbContext
    {
        public ShortnerContext(DbContextOptions options):base(options)
        {

        }

        public DbSet<Shortner> UrlShortner { get; set; }
    }
}
