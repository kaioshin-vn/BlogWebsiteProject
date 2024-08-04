using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogWebsite.Data;

namespace Data.Database
{
    public class ApplicationFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=DATN;Trusted_Connection=True; TrustServerCertificate=true");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
