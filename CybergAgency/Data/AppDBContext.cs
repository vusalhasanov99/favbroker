using CybergAgency.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CybergAgency.Data
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<WebSite> WebSites { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<MarketSubcatagory> MarketSubcatagories { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<GClid> GClids { get; set; }
        public DbSet<PostBack> PostBacks { get; set; }
        public DbSet<BrandClick> BrandClicks { get; set; }
        public DbSet<StaticData> StaticDatas { get; set; }
    }
}
