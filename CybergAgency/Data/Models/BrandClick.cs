namespace CybergAgency.Data.Models
{
    public class BrandClick : BaseEntity
    {
        public WebSite WebSite { get; set; }
        public GClid GClid { get; set; }
        public Brand Brand { get; set; }
    }
}
