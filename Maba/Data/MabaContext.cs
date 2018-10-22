using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Maba.Models
{
    public class MabaContext : DbContext
    {
        public MabaContext (DbContextOptions<MabaContext> options)
            : base(options)
        {
        }

        public DbSet<Maba.Models.Users> User { get; set; }
		public DbSet<Maba.Models.TimeReport> TimeReport { get; set; }
    }
}
