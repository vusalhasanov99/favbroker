using Microsoft.AspNetCore.Identity;

namespace CybergAgency.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public List<MarketSubcatagory> MarketSubcatagories { get; set; }
    }
}
